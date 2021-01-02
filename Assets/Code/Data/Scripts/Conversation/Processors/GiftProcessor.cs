using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Rollers;
using JoyLib.Code.Unity.GUI;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class GiftProcessor : TopicData
    {
        protected IGUIManager GUIManager { get; set; }
        
        public GiftProcessor() 
            : base(
                new ITopicCondition[0], 
                "GiftTopic", 
                new string[] { "ListenerThanks" }, 
                "", 
                0, 
                new string[] { "giveitemaction" }, 
                Speaker.INSTIGATOR,
                new RNG())
        {
            if (this.GUIManager is null)
            {
                this.GUIManager = GlobalConstants.GameManager.GUIManager;
            }
        }

        public override ITopic[] Interact(IEntity instigator, IEntity listener)
        {
            this.GUIManager.OpenGUI("Inventory");
            this.GUIManager.BringToFront("Inventory");
            
            return base.Interact(instigator, listener);
        }
    }
}