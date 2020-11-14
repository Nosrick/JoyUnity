using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class SexProposalProcessor : TopicData
    {
        public SexProposalProcessor()
            : base(
                new ITopicCondition[0],
                "SexProposal",
                new []{"SexDecision"},
                "words",
                0,
                new string[0],
                Speaker.INSTIGATOR)
        {}

        protected override ITopic[] FetchNextTopics()
        {
            Entity listener = ConversationEngine.Listener;
            Entity instigator = ConversationEngine.Instigator;
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

            string decision = "";
            bool happening = false;
            if (highestValue > listener.Sexuality.MatingThreshold && chosenRelationship is null == false)
            {
                decision = "Sounds good.";
                happening = true;
            }
            else
            {
                decision = "I'm sorry, no.";
            }

            return new ITopic[]
            {
                new SexDecision(decision, happening)
            };
        }
    }
}