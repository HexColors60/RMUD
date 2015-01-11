﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD.Modules.Quests
{
    public class QuestProceduralRules 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            GlobalRules.Perform<PossibleMatch, Actor>("after acting")
                .Do((match, actor) =>
                {
                    var player = actor as Player;
                    if (player != null && player.ActiveQuest != null)
                    {
                        var quest = player.ActiveQuest;

                        if (GlobalRules.ConsiderValueRule<bool>("quest complete?", player, quest))
                        {
                            player.ActiveQuest = null;
                            GlobalRules.ConsiderPerformRule("quest completed", player, quest);
                        }
                        else if (GlobalRules.ConsiderValueRule<bool>("quest failed?", player, quest))
                        {
                            player.ActiveQuest = null;
                            GlobalRules.ConsiderPerformRule("quest failed", player, quest);
                        }
                    }

                    return PerformResult.Continue;
                })
                .Name("Check quest status after acting rule.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest reset", "[quest, thing] : The quest is being reset. Quests can call this on objects they interact with.");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest accepted", "[actor, quest] : Handle accepting a quest.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest completed", "[actor, quest] : Handle when a quest is completed.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest failed", "[actor, quest] : Handle when a quest is failed.");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("quest abandoned", "[actor, quest] : Handle when a quest is abandoned.");

            GlobalRules.Perform<MudObject, MudObject>("quest abandoned")
                .Last
                .Do((actor, quest) =>
                {
                    return GlobalRules.ConsiderPerformRule("quest failed", actor, quest);
                })
                .Name("Abandoning a quest is failure rule.");

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, bool>("quest available?", "[actor, quest -> bool] : Is the quest available to this actor?");

            GlobalRules.Value<MudObject, MudObject, bool>("quest available?")
                .Do((Actor, quest) => false)
                .Name("Quests unavailable by default rule.");

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, bool>("quest complete?", "[actor, quest -> bool] : Has this actor completed this quest?");

            GlobalRules.Value<MudObject, MudObject, bool>("quest complete?")
                .Do((actor, quest) => false)
                .Name("Quests incomplete by default rule.");

            GlobalRules.DeclareValueRuleBook<MudObject, MudObject, bool>("quest failed?", "[actor, quest -> bool] : Has this actor failed this quest?");

            GlobalRules.Value<MudObject, MudObject, bool>("quest failed?")
                .Do((actor, quest) => false)
                .Name("Quests can't fail by default rule.");

        }
    }
}
