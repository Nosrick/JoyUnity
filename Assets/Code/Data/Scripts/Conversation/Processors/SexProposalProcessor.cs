using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class SexProposalProcessor : TopicData
    {
        protected bool Happening { get; set; }
        
        public SexProposalProcessor()
            : base(
                new ITopicCondition[0],
                "SexProposal",
                new string[0],
                "words",
                0,
                new string[0],
                Speaker.INSTIGATOR)
        {}

        public override ITopic[] Interact(IEntity instigator, IEntity listener)
        {
            Happening = false;

            IJoyObject[] participants = new[] {instigator, listener};
            
            List<IRelationship> relationships =
                RelationshipHandler.Get(participants, new[] {"sexual"}, false).ToList();

            if (relationships.IsNullOrEmpty()
                && listener.Sexuality.Compatible(listener, instigator)
                && instigator.Sexuality.Compatible(instigator, listener))
            {
                relationships.Add(RelationshipHandler.CreateRelationship(participants, new string[] {"sexual"}));
            }
            
            if (listener.Sexuality.WillMateWith(listener, instigator, relationships) == false
            || instigator.Sexuality.WillMateWith(instigator, listener, relationships) == false)
            {
                return base.Interact(instigator, listener);
            }
            
            base.Interact(instigator, listener);
            
            IJoyAction fulfillNeed = instigator.FetchAction("fulfillneedaction");

            int listenerSatisfaction = (
                instigator.Statistics[EntityStatistic.INTELLECT].Value
                + instigator.Statistics[EntityStatistic.ENDURANCE].Value
                + instigator.Statistics[EntityStatistic.PERSONALITY].Value) / 3;
            
            int instigatorSatisfaction = (
                listener.Statistics[EntityStatistic.INTELLECT].Value
                + listener.Statistics[EntityStatistic.ENDURANCE].Value
                + listener.Statistics[EntityStatistic.PERSONALITY].Value) / 3;
            
            fulfillNeed.Execute(
                new IJoyObject[] {instigator},
                new[] {"sex", "need"},
                new object[] {"sex", instigatorSatisfaction, 5});
            fulfillNeed.Execute(
                new IJoyObject[] {listener},
                new[] {"sex", "need"},
                new object[] {"sex", listenerSatisfaction, 5});

            Happening = true;

            return FetchNextTopics();
        }

        protected override ITopic[] FetchNextTopics()
        {
            if (!Happening)
            {
                return new ITopic[]
                {
                    new TopicData(
                        new ITopicCondition[0],
                        "SexRejection",
                        new string[] {"BaseTopics"},
                        "No thank you.",
                        0,
                        new string[0],
                        Speaker.LISTENER)
                };
            }
            else
            {
                return new ITopic[]
                {
                    new TopicData(
                        new ITopicCondition[0],
                        "SexAcceptance",
                        new string[0],
                        "words",
                        0,
                        new string[0],
                        Speaker.LISTENER)
                };
            }
        }
    }
}