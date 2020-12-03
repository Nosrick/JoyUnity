using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Conversation.Subengines.Rumours;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class ChatterProcessor : TopicData
    {
        protected static IRumourMill RumourMill
        {
            get;
            set;
        }
        
        public ChatterProcessor() 
            : base(
                new ITopicCondition[0], 
                "ChatterTopic", 
                new[] { "Thanks" }, 
                "", 
                0, 
                new string[0], 
                Speaker.LISTENER,
                new RNG())
        {
            Initialise();
        }

        protected void Initialise()
        {
            if (RumourMill is null)
            {
                RumourMill = new ConcreteRumourMill();
            }
        }

        protected override ITopic[] FetchNextTopics()
        {
            return new ITopic[]
            {
                new TopicData(
                    new ITopicCondition[0],
                    "ChatterTopic",
                    new string[] {"Thanks"},
                    RumourMill.GetRandom(ConversationEngine.Listener.MyWorld.GetOverworld()).Words,
                    0,
                    new string[0],
                    Speaker.LISTENER,
                    new RNG())
            };
        }
    }
}