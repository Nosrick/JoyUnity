using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class RomanticProposalProcessor : TopicData
    {
        public RomanticProposalProcessor() 
            : base(
                new ITopicCondition[0], 
                "RomanticProposal", 
                new []
                {
                    "RomancePresentation"
                }, 
                "words", 
                0, 
                new string[0], 
                Speaker.INSTIGATOR)
        {
        }

        protected override ITopic[] FetchNextTopics()
        {
            Entity listener = ConversationEngine.Listener;
            Entity instigator = ConversationEngine.Instigator;
            int highestValue = RelationshipHandler.GetHighestRelationshipValue(instigator, listener);
            if (highestValue > listener.Sexuality.MatingThreshold)
            {
                int cultureResult = Roller.Roll(0, listener.Cultures.Count);
                int relationshipTypeResult = Roller.Roll(0, listener.Cultures[cultureResult].RelationshipTypes.Length);
    
                string relationshipType = listener.Cultures[cultureResult].RelationshipTypes[relationshipTypeResult];
    
                return new ITopic[]
                {
                    new RomancePresentation(relationshipType)
                };
            }
            else
            {
                return new ITopic[]
                {
                    new TopicData(
                        new ITopicCondition[0],
                        "RomanceTurnDown",
                        new[] {"BaseTopics"},
                        "Uh, no thanks.",
                        0,
                        new string[0],
                        Speaker.LISTENER)
                };
            }
        }
    }
}