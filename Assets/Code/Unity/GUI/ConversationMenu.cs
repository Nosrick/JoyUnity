using JoyLib.Code.Conversation;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class ConversationMenu : MonoBehaviour
    {
        public string TopicID
        {
            get;
            set;
        }
        
        protected static ConversationEngine ConversationEngine
        {
            get;
            set;
        }

        protected Text Text
        {
            get;
            set;
        }

        public void Awake()
        {
            if (ConversationEngine is null)
            {
                ConversationEngine = GameObject.Find("GameManager").GetComponent<ConversationEngine>();

                Text = this.transform.GetChild(0).GetComponent<Text>();
            }
        }

        public void OnMouseDown()
        {
            ConversationEngine.Converse(TopicID);
        }

        public void SetText(string text)
        {
            if (Text is null)
            {
                Text = this.transform.GetChild(0).GetComponent<Text>();
            }
            
            Text.text = text;
        }
    }
}