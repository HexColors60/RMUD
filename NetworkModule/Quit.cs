﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace NetworkModule
{
	internal class Quit : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                KeyWord("QUIT"))
                .Manual("Disconnect from the game immediately.")
                .ProceduralRule((match, actor) =>
                {
                    if (actor != null && actor.ConnectedClient != null)
                        match.Upsert("CLIENT", actor.ConnectedClient);
                    if (match.ContainsKey("CLIENT"))
                    {
                        (match["CLIENT"] as Client).Send("Goodbye...\r\n");
                        (match["CLIENT"] as Client).Disconnect();
                    }
                    return PerformResult.Continue;
                });
        }
	}
}
