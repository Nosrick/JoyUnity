using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class RomanceYes : TopicData
    {
        protected string RelationshipName { get; set; }
        
        public RomanceYes(string relationshipName)
        : base(
            new ITopicCondition[0],
            "RomanceYes",
            new[] { "BaseTopics" },
            "It certainly is.",
            0,
            new string[0], 
            Speaker.LISTENER,
            new RNG())
        {
            this.RelationshipName = relationshipName;
        }

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            RelationshipHandler.CreateRelationship(
                new IJoyObject[]
                {
                    instigator,
                    listener
                },
                RelationshipName);
            
            return base.Interact(instigator, listener);
        }
    }
}