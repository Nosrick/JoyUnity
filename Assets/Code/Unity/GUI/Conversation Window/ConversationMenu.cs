using System;
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

        public int Index
        {
            get;
            set;
        }

        protected Text Text
        {
            get;
            set;
        }
        
        protected static IConversationEngine ConversationEngine
        {
            get;
            set;
        }

        public void Awake()
        {
            if (ConversationEngine is null)
            {
                ConversationEngine = GlobalConstants.GameManager.ConversationEngine;

                Text = this.transform.GetChild(0).GetComponent<Text>();
            }
        }

        public void OnMouseDown()
        {
            ConversationEngine.Converse(Index);
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