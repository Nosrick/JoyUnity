using System.Globalization;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class AbilityItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected ValueContainer m_Parent;
        [SerializeField] public int Delta = 1;
        public bool Selected { get; protected set; }

        public string Tooltip { get; set; }

        public string Name
        {
            get
            {
                return this.Text.text;
            }
            set
            {
                this.Text.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
            }
        }
        protected Image Image { get; set; }
        protected TextMeshProUGUI Text { get; set; }
        
        protected static IGUIManager GUIManager { get; set; }

        public event ValueChangedEventHandler OnSelect;

        public void Awake()
        {
            if (GUIManager is null)
            {
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }

            this.Image = this.GetComponent<Image>();
            this.Text = this.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        public void OnPointerEnter(PointerEventData data)
        {
            if (this.Tooltip is null)
            {
                return;
            }

            Tooltip tooltip = GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>();
            tooltip.Show(null, this.Tooltip);
        }

        public void OnPointerExit(PointerEventData data)
        {
            GUIManager.CloseGUI(GUINames.TOOLTIP);
        }

        public void ToggleMe()
        {
            if (this.m_Parent.Value < this.Delta && this.Selected == false)
            {
                return;
            }

            this.Selected = !this.Selected;

            this.OnSelect?.Invoke(this, new ValueChangedEventArgs
            {
                Name = this.Name,
                NewValue = this.Selected ? this.Delta : -this.Delta,
                Delta = this.Selected ? this.Delta : -this.Delta 
            });
        }
    }
}