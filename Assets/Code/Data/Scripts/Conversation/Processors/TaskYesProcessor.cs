using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Quests;
using UnityEngine;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class TaskYesProcessor : TopicData
    {
        protected static QuestTracker QuestTracker { get; set; }
        protected static Entity Player { get; set; }
        
        protected IQuest OfferedQuest { get; set; }
        
        public TaskYesProcessor(IQuest offeredQuest) 
            : base(
                new ITopicCondition[0], 
                "TaskYes", 
                new []{ "ListenerThanks" }, 
                "I can do that.", 
                0, 
                new string[0], 
                Speaker.INSTIGATOR)
        {
            Initialise();
            this.OfferedQuest = offeredQuest;
        }

        protected void Initialise()
        {
            if (!(QuestTracker is null))
            {
                return;
            }
            GameObject gameManager = GameObject.Find("GameManager");
            QuestTracker = gameManager.GetComponent<QuestTracker>();
            Player = gameManager.GetComponent<LiveEntityHandler>().GetPlayer();
        }

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            base.Interact(instigator, listener);
            QuestTracker.AddQuest(
                instigator.GUID,
                this.OfferedQuest);

            foreach (string next in NextTopics)
            {
                Debug.Log(next);
            }
            
            OfferedQuest.StartQuest(instigator);

            this.Words = this.OfferedQuest.ToString();

            return FetchNextTopics();
        }
    }
}