using System;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Events;
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
        
        public event JoyObjectMouseOverHandler OnMouseOverEvent;
        public event JoyObjectMouseExitHandler OnMouseExitEvent;

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
            this.OnMouseExitEvent = null;
            this.OnMouseOverEvent = null;
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
            this.OnMouseOverEvent?.Invoke(this, new JoyObjectMouseOverEventArgs() { Actor = this.MyJoyObject });
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.OnMouseExitEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
