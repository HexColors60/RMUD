﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RMUD;

namespace ConversationModule
{
    public class TopicSource : IObjectSource
    {
        public String LocutorArgument;

        public TopicSource() { }
        public TopicSource(String LocutorArgument)
        {
            this.LocutorArgument = LocutorArgument;
        }

        public List<MudObject> GetObjects(PossibleMatch State, MatchContext Context)
        {
            NPC source = null;
            if (!String.IsNullOrEmpty(LocutorArgument))
                source = State[LocutorArgument] as NPC;
            else if (Context.ExecutingActor.HasProperty<NPC>("interlocutor"))
                source = Context.ExecutingActor.GetProperty<NPC>("interlocutor");

            if (source != null)
                if (source.HasProperty<List<MudObject>>("conversation-topics"))
                    return new List<MudObject>(source.GetProperty<List<MudObject>>("conversation-topics").Where(t => Core.GlobalRules.ConsiderCheckRuleSilently("topic available?", Context.ExecutingActor, source, t) == CheckResult.Allow));

            return new List<MudObject>();
        }
    }
}
