using JoyLib.Code.Conversation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class ConversationMenu : MonoBehaviour, IPointerDownHandler
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

        protected TextMeshProUGUI Text
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
            if (GlobalConstants.GameManager.ConversationEngine is null == false 
                && (ConversationEngine is null
                    || ConversationEngine.GUID != GlobalConstants.GameManager.ConversationEngine?.GUID))
            {
                ConversationEngine = GlobalConstants.GameManager.ConversationEngine;
            }

            this.Text = this.transform.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ConversationEngine.Converse(this.Index);
        }

        public void SetText(string text)
        {
            if (this.Text is null)
            {
                this.Text = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            this.Text.text = text;
        }
    }
}