using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Quests;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class TaskProcessor : TopicData
    {
        protected static IQuestProvider QuestProvider { get; set; }
        
        protected IQuest OfferedQuest { get; set; }
        
        public TaskProcessor() 
            : base(
                new ITopicCondition[0], 
                "TaskTopic", 
                new []
                {
                    "TaskPresentation"
                }, 
                "", 
                0, 
                null, 
                Speaker.LISTENER)
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (!(QuestProvider is null))
            {
                return;
            }

            IGameManager gameManager = GlobalConstants.GameManager;
            QuestProvider = gameManager.QuestProvider;
        }

        public override ITopic[] Interact(IEntity instigator, IEntity listener)
        {
            this.OfferedQuest = QuestProvider.MakeRandomQuest(
                instigator,
                listener,
                instigator.MyWorld.GetOverworld());

            return this.FetchNextTopics();
        }

        protected override ITopic[] FetchNextTopics()
        {
            return new ITopic[]
            {
                new TaskPresentationProcessor(this.OfferedQuest)
            };
        }
    }
}