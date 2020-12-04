using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class RomancePresentation : TopicData
    {
        protected IRelationship SelectedRelationship { get; set; }
        
        public RomancePresentation(IRelationship relationship)
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
            this.SelectedRelationship = relationship;
            Words = Words.Replace("<1>", SelectedRelationship.Name);
        }

        protected override ITopic[] FetchNextTopics()
        {
            return new ITopic[]
            {
                new RomanceYes(SelectedRelationship),
                new RomanceNo()
            };
        }
    }
}