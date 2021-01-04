using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Quests;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class TaskPresentationProcessor : TopicData
    {
        protected IQuest OfferedQuest { get; set; }
        
        public TaskPresentationProcessor(IQuest offeredQuest) 
            : base(
                new ITopicCondition[0], 
                "TaskPresentation",
                new []
                {
                    "TaskYes",
                    "TaskNo"
                }, 
                offeredQuest.ToString(), 
                0,
                new string[0], 
                Speaker.LISTENER)
        {
            this.OfferedQuest = offeredQuest;
        }

        protected override ITopic[] FetchNextTopics()
        {
            return new ITopic[]
            {
                new TaskYesProcessor(this.OfferedQuest),
                new TaskNoProcessor()
            };
        }
    }
}