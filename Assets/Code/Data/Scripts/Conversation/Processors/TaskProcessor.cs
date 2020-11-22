using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Quests;
using UnityEngine;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class TaskProcessor : TopicData
    {
        protected static QuestProvider QuestProvider { get; set; }
        
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
                new string[0], 
                Speaker.LISTENER)
        {
            Initialise();
        }

        protected void Initialise()
        {
            if (!(QuestProvider is null))
            {
                return;
            }
            GameObject gameManager = GameObject.Find("GameManager");
            QuestProvider = gameManager.GetComponent<QuestProvider>();
        }

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            OfferedQuest = QuestProvider.MakeRandomQuest(
                instigator,
                listener,
                instigator.MyWorld.GetOverworld());

            return FetchNextTopics();
        }

        protected override ITopic[] FetchNextTopics()
        {
            return new ITopic[]
            {
                new TaskPresentationProcessor(OfferedQuest)
            };
        }
    }
}