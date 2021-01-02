using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Relationships;

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
            IEntity listener = ConversationEngine.Listener;
            IEntity instigator = ConversationEngine.Instigator;
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(new IJoyObject[] {instigator, listener});
            if (listener.Romance.WillRomance(listener, instigator, relationships)
            && instigator.Romance.WillRomance(instigator, listener, relationships))
            {
                int cultureResult = this.Roller.Roll(0, listener.Cultures.Count);
                int relationshipTypeResult = this.Roller.Roll(0, listener.Cultures[cultureResult].RelationshipTypes.Length);
    
                string relationshipType = listener.Cultures[cultureResult].RelationshipTypes[relationshipTypeResult];
                IRelationship selectedRelationship = RelationshipHandler.RelationshipTypes.First(relationship =>
                    relationship.Name.Equals(relationshipType, StringComparison.OrdinalIgnoreCase));

                return new ITopic[]
                {
                    new RomancePresentation(selectedRelationship)
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