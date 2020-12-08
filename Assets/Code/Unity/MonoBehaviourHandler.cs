using System;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Events;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    public class MonoBehaviourHandler : MonoBehaviour
    {
        protected IJoyObject m_JoyObject;
        protected SpriteRenderer m_SpriteRenderer;
        protected SpriteRenderer SpeechBubble { get; set; }

        public IJoyObject MyJoyObject => m_JoyObject;
        
        public event JoyObjectMouseOverHandler OnMouseOverEvent;
        public event JoyObjectMouseExitHandler OnMouseExitEvent;

        public void Update()
        {
            if (m_JoyObject == null)
            {
                return;
            }
            
            m_JoyObject.Update();
            m_SpriteRenderer.sprite = m_JoyObject.Sprite;
            this.transform.position = new Vector3(m_JoyObject.WorldPosition.x, m_JoyObject.WorldPosition.y);
        }

        public void Start()
        {
        }

        public virtual void AttachJoyObject(IJoyObject joyObject)
        {
            m_JoyObject = joyObject;
            m_JoyObject.AttachMonoBehaviourHandler(this);
            m_SpriteRenderer = this.GetComponent<SpriteRenderer>();
            Transform transform = this.transform.Find("Speech Bubble");
            if (transform is null == false)
            {
                SpeechBubble = transform.GetComponent<SpriteRenderer>();
                SpeechBubble.gameObject.SetActive(false);
            }

            if(m_JoyObject.JoyName.StartsWith("Downstairs") || m_JoyObject.JoyName.StartsWith("Upstairs"))
            {
                m_SpriteRenderer.sortingLayerName = "Walls";
            }
            else if (m_JoyObject.GetType() == typeof(JoyObject))
            {
                if(m_JoyObject.IsWall)
                {
                    m_SpriteRenderer.sortingLayerName = "Walls";
                }
                else
                {
                    m_SpriteRenderer.sortingLayerName = "Terrain";
                }
            }
            else
            {
                if(m_JoyObject is ItemInstance)
                {
                    m_SpriteRenderer.sortingLayerName = "Objects";
                }
                else
                {
                    m_SpriteRenderer.sortingLayerName = "Entities";
                }
            }
            this.name = m_JoyObject.JoyName + ":" + m_JoyObject.GUID;
            this.transform.position = new Vector3(m_JoyObject.WorldPosition.x, m_JoyObject.WorldPosition.y, 0.0f);
            m_SpriteRenderer.sprite = joyObject.Sprite;
        }

        public void SetSpeechBubble(bool on, Sprite need = null)
        {
            SpeechBubble.gameObject.SetActive(on);
            if (on)
            {
                SpeechBubble.sprite = need;
            }
        }

        public void OnMouseEnter()
        {
            OnMouseOverEvent?.Invoke(this, new JoyObjectMouseOverEventArgs() { Actor = MyJoyObject});
        }

        public void OnMouseExit()
        {
            OnMouseExitEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
