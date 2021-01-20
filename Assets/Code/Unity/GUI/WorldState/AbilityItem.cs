using System.Globalization;
using Code.Unity.GUI.Managed_Assets;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(ManagedButton))]
    public class AbilityItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ValueContainer Parent { get; set; }
        [SerializeField] public int Delta = 1;
        public string Tooltip { get; set; }
        
        protected ManagedButton MyButton { get; set; }

        public bool Selected => this.MyButton.Toggled;

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
            this.MyButton = this.GetComponent<ManagedButton>();
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
            if (this.Parent.Value < this.Delta && this.MyButton.Toggled)
            {
                this.MyButton.Toggled = false;
                return;
            }

            this.OnSelect?.Invoke(this, new ValueChangedEventArgs
            {
                Name = this.Name,
                NewValue = this.MyButton.Toggled ? this.Delta : -this.Delta,
                Delta = this.MyButton.Toggled ? this.Delta : -this.Delta 
            });
        }
    }
}