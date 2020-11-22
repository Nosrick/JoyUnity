using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Unity.GUI;
using UnityEngine;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class GiftProcessor : TopicData
    {
        protected GUIManager GUIManager { get; set; }
        
        public GiftProcessor() 
            : base(
                new ITopicCondition[0], 
                "GiftTopic", 
                new string[] { "ListenerThanks" }, 
                "", 
                0, 
                new string[] { "giveitemaction" }, 
                Speaker.INSTIGATOR)
        {
            if (GUIManager is null)
            {
                GUIManager = GameObject.Find("GameManager").GetComponent<GUIManager>();
            }
        }

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            GUIManager.OpenGUI("Inventory");
            GUIManager.BringToFront("Inventory");
            
            return base.Interact(instigator, listener);
        }
    }
}