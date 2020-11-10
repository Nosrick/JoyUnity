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
                    "RomanceYes",
                    "RomanceNo"
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
            
            int cultureResult = RNG.instance.Roll(0, listener.Cultures.Length);
            int relationshipTypeResult = RNG.instance.Roll(0, listener.Cultures[cultureResult].RelationshipTypes.Length);

            string relationshipType = listener.Cultures[cultureResult].RelationshipTypes[relationshipTypeResult];

            return new ITopic[]
            {
                new RomancePresentation(relationshipType)
            };
        }
    }
}