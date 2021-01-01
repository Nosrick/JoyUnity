using JoyLib.Code.Entities.Items;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity
{
    public class MonoBehaviourHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected IJoyObject m_JoyObject;
        protected SpriteRenderer m_SpriteRenderer;
        protected SpriteRenderer SpeechBubble { get; set; }

        public IJoyObject MyJoyObject => this.m_JoyObject;
        
        protected static IGUIManager GUIManager { get; set; }

        public void Update()
        {
            if (this.m_JoyObject == null)
            {
                return;
            }
            
            this.m_JoyObject.Update();
            this.m_SpriteRenderer.sprite = this.m_JoyObject.Sprite;
            this.transform.position = new Vector3(this.m_JoyObject.WorldPosition.x, this.m_JoyObject.WorldPosition.y);
        }

        public void Start()
        {
        }

        public virtual void AttachJoyObject(IJoyObject joyObject)
        {
            if (GUIManager is null)
            {
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }
            
            this.m_JoyObject = joyObject;
            this.m_JoyObject.AttachMonoBehaviourHandler(this);
            this.m_SpriteRenderer = this.GetComponent<SpriteRenderer>();
            Transform transform = this.transform.Find("Speech Bubble");
            if (transform is null == false)
            {
                this.SpeechBubble = transform.GetComponent<SpriteRenderer>();
                this.SpeechBubble.gameObject.SetActive(false);
            }

            if(this.m_JoyObject.JoyName.StartsWith("Downstairs") || m_JoyObject.JoyName.StartsWith("Upstairs"))
            {
                this.m_SpriteRenderer.sortingLayerName = "Walls";
            }
            else if (this.m_JoyObject.GetType() == typeof(JoyObject))
            {
                if(this.m_JoyObject.IsWall)
                {
                    this.m_SpriteRenderer.sortingLayerName = "Walls";
                }
                else
                {
                    this.m_SpriteRenderer.sortingLayerName = "Terrain";
                }
            }
            else
            {
                if(this.m_JoyObject is ItemInstance)
                {
                    this.m_SpriteRenderer.sortingLayerName = "Objects";
                }
                else
                {
                    this.m_SpriteRenderer.sortingLayerName = "Entities";
                }
            }
            this.name = this.m_JoyObject.JoyName + ":" + this.m_JoyObject.GUID;
            this.transform.position = new Vector3(this.m_JoyObject.WorldPosition.x, this.m_JoyObject.WorldPosition.y, 0.0f);
            this.m_SpriteRenderer.sprite = joyObject.Sprite;
        }

        public void SetSpeechBubble(bool on, Sprite need = null)
        {
            this.SpeechBubble.gameObject.SetActive(on);
            if (on)
            {
                this.SpeechBubble.sprite = need;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (GUIManager.IsActive(GUINames.CONTEXT_MENU) == false)
            {
                GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>().Show(
                    this.m_JoyObject.JoyName,
                    null,
                    this.m_JoyObject.Sprite,
                    this.m_JoyObject.Tooltip);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GUIManager.CloseGUI(GUINames.TOOLTIP);
        }
    }
}
