﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RMUD.Modules.ClientLogin
{
	public class LoginCommandHandler : ClientCommandHandler
	{
		internal CommandParser Parser;

        public static void LogPlayerIn(Client Client, Account Account)
        {
            Client.Account = Account;
            Client.CommandHandler = Core.ParserCommandHandler;
            Client.Rank = 500;

            if (Account.LoggedInCharacter != null)
            {
                //Connect to the existing session
                if (Account.LoggedInCharacter.ConnectedClient != null)
                {
                    Account.LoggedInCharacter.ConnectedClient.Player = null;
                    Account.LoggedInCharacter.ConnectedClient.Send("You are being disconnected because you have logged into this account from another connection.\r\n");
                    Account.LoggedInCharacter.ConnectedClient.Disconnect();
                }
                Client.Send("You were already logged in. You are being connected to that session.\r\n");
                Client.Player = Account.LoggedInCharacter;
            }
            else
            {
                //Start a new session
                Client.Player = Accounts.GetAccountCharacter(Account);
                Client.Player.Rank = Client.Rank;
                MudObject.Move(Client.Player, MudObject.GetObject(Core.SettingsObject.NewPlayerStartRoom));
                Core.EnqueuClientCommand(Client, "look");
            }


            Client.Player.ConnectedClient = Client;
            Account.LoggedInCharacter = Client.Player;

            GlobalRules.ConsiderPerformRule("player joined", Client.Player);
        }

		public LoginCommandHandler()
		{
			Parser = new CommandParser();

            CommandFactory.CreateCommandFactory(typeof(Modules.ClientLogin.Login)).Create(Parser);
            CommandFactory.CreateCommandFactory(typeof(Modules.ClientLogin.Register)).Create(Parser);
            CommandFactory.CreateCommandFactory(typeof(Modules.ClientLogin.Quit)).Create(Parser);

		}

		public void HandleCommand(Client Client, String Command)
		{
            try
			{
				var matchedCommand = Parser.ParseCommand(Command, null);
                if (matchedCommand != null)
                {
                    matchedCommand.Matches[0].Upsert("CLIENT", Client);
                    Core.ProcessPlayerCommand(matchedCommand.Command, matchedCommand.Matches[0], null);
                }
                else
                    MudObject.SendMessage(Client, "I do not understand.");
			}
			catch (Exception e)
			{
				Core.ClearPendingMessages();
                MudObject.SendMessage(Client, e.Message);
			}
		}
	}
}
