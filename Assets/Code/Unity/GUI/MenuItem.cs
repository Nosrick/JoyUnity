using Code.Unity.GUI.Managed_Assets;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class MenuItem : ManagedButton
    {
        [SerializeField] protected TextMeshProUGUI m_Text;

        public TextMeshProUGUI Text
        {
            get => this.m_Text;
            protected set => this.m_Text = value;
        }

        public void OnPointerClick (PointerEventData eventData)
        {
            if (!this.isActiveAndEnabled || !this.IsInteractable() || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            this.m_OnClick.Invoke();
        }


        public override void OnPointerEnter (PointerEventData eventData)
        {
            base.OnPointerEnter (eventData);
            this.DoStateTransition (SelectionState.Highlighted, false);
        }
    }
}