using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Unity.GUI;
using UnityEngine;

namespace JoyLib.Code.Entities.Abilities.Conversation.Processors
{
    public class TradeProcessor : TopicData
    {
        protected static TradeWindow TradeWindow { get; set; }
        protected static IGUIManager GUIManager { get; set; }

        public TradeProcessor()
            : base(
                new ITopicCondition[0],
                "TradeTopic",
                new string[0],
                "words",
                0,
                new string[0],
                Speaker.INSTIGATOR)
        {
            Initialise();
        }

        protected void Initialise()
        {
            if (TradeWindow is null)
            {
                TradeWindow = GameObject.Find("Trade").GetComponent<TradeWindow>();
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }
        }

        public override ITopic[] Interact(Entity instigator, Entity listener)
        {
            TradeWindow.SetActors(instigator, listener);
            
            GUIManager.OpenGUI("Trade");
            
            return base.Interact(instigator, listener);
        }
    }
}