using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Quests;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class TaskYesProcessor : TopicData
    {
        protected static IQuestTracker QuestTracker { get; set; }
        protected static IEntity Player { get; set; }
        
        protected IQuest OfferedQuest { get; set; }
        
        public TaskYesProcessor(IQuest offeredQuest) 
            : base(
                new ITopicCondition[0], 
                "TaskYes", 
                new []{ "ListenerThanks" }, 
                "I can do that.", 
                0, 
                null, 
                Speaker.INSTIGATOR)
        {
            this.Initialise();
            this.OfferedQuest = offeredQuest;
        }

        protected void Initialise()
        {
            if (!(QuestTracker is null))
            {
                return;
            }

            IGameManager gameManager = GlobalConstants.GameManager;
            QuestTracker = gameManager.QuestTracker;
            Player = gameManager.EntityHandler.GetPlayer();
        }

        public override ITopic[] Interact(IEntity instigator, IEntity listener)
        {
            base.Interact(instigator, listener);
            QuestTracker.AddQuest(
                instigator.Guid,
                this.OfferedQuest);

            this.OfferedQuest.StartQuest(instigator);

            this.Words = this.OfferedQuest.ToString();

            return this.FetchNextTopics();
        }
    }
}