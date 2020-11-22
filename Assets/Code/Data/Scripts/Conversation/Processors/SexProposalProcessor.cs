using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class SexProposalProcessor : TopicData
    {
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

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            IRelationship[] relationships =
                RelationshipHandler.Get(new IJoyObject[] {instigator, listener}, new[] {"sexual"});
            int highestValue = int.MinValue;
            IRelationship chosenRelationship = null;

            foreach (IRelationship relationship in relationships)
            {
                int value = relationship.GetRelationshipValue(instigator.GUID, listener.GUID);
                if (value > highestValue)
                {
                    highestValue = value;
                    chosenRelationship = relationship;
                }
            }

            if (highestValue < listener.Sexuality.MatingThreshold || chosenRelationship is null)
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

            return FetchNextTopics();
        }
    }
}