using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class RomancePresentation : TopicData
    {
        protected string RelationshipType { get; set; }
        
        public RomancePresentation(string relationshipType)
        : base(
            new ITopicCondition[0], 
            "RomancePresentation",
            new []
            {
                "RomanceYes",
                "RomanceNo"
            },
            "Is a <1> relationship okay?",
            0,
            new string[0],
            Speaker.LISTENER,
            new RNG())
        {
            this.RelationshipType = relationshipType;
            Words = Words.Replace("<1>", RelationshipType);
        }

        protected override ITopic[] FetchNextTopics()
        {
            return new ITopic[]
            {
                new RomanceYes(RelationshipType),
                new RomanceNo()
            };
        }
    }
}