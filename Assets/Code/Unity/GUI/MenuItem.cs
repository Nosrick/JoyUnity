using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class MenuItem : Selectable, IPointerClickHandler
    {
        protected UnityEvent m_Trigger = new UnityEvent();
        public UnityEvent Trigger {
            get 
            {
                if (this.m_Trigger == null) 
                {
                    this.m_Trigger = new UnityEvent ();
                }
                return this.m_Trigger;
            }
            set => this.m_Trigger = value;
        }

        [SerializeField] protected TextMeshProUGUI m_Text;

        public TextMeshProUGUI Text
        {
            get => this.m_Text;
            protected set => this.m_Text = value;
        }

        public void OnPointerClick (PointerEventData eventData)
        {
            if (!this.IsActive() || !this.IsInteractable())
            {
                return;
            }
            this.Trigger.Invoke();
        }


        public override void OnPointerEnter (PointerEventData eventData)
        {
            base.OnPointerEnter (eventData);
            this.DoStateTransition (SelectionState.Highlighted, false);
        }
    }
}