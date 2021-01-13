using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Helpers;
using JoyLib.Code.Unity.GUI;

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
            this.Initialise();
        }

        protected void Initialise()
        {
            if (TradeWindow is null || GUIManager is null)
            {
                try
                {
                    GUIManager = GlobalConstants.GameManager.GUIManager;
                    TradeWindow = GUIManager.GetGUI(GUINames.TRADE).GetComponent<TradeWindow>();
                }
                catch
                {
                    GlobalConstants.ActionLog.AddText("Could not load TradeProcessor bits. Trying again later.", LogLevel.Warning);
                }
            }
        }

        public override ITopic[] Interact(IEntity instigator, IEntity listener)
        {
            this.Initialise();
            TradeWindow.SetActors(instigator, listener);
            
            GUIManager.OpenGUI("Trade");
            
            return base.Interact(instigator, listener);
        }
    }
}