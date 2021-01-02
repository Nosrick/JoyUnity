using System;
using System.Linq;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class RomanceNo : TopicData
    {
        public RomanceNo()
            : base(
                new ITopicCondition[0], 
                "RomanceNo",
                new []{ "BaseTopics" },
                "No thanks.",
                0,
                new string[0], 
                Speaker.INSTIGATOR,
                new RNG())
        {}

        public override ITopic[] Interact(IEntity instigator, IEntity listener)
        {
            IJoyAction influence = this.CachedActions.First(action =>
                action.Name.Equals("modifyrelationshippointsaction", StringComparison.OrdinalIgnoreCase));

            influence.Execute(
                new IJoyObject[]
                {
                    listener,
                    instigator
                },
                new[] {"friendship"},
                new object[] {-instigator.Statistics[EntityStatistic.PERSONALITY].Value});
            
            return this.FetchNextTopics();
        }
    }
}