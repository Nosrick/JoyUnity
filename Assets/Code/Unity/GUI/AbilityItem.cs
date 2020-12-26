using System.Globalization;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(Image))]
    public class AbilityItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected ValueContainer Parent;
        [SerializeField] protected Color32 SelectedColour;
        [SerializeField] protected Color32 IdleColour;
        [SerializeField] public int Delta = 1;
        public bool Selected { get; protected set; }
        
        public string Tooltip { get; set; }

        public string Name
        {
            get
            {
                return Text.text;
            }
            set
            {
                Text.text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
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
            
            Image = this.GetComponent<Image>();
            Image.color = IdleColour;
            Text = this.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        public void OnPointerEnter(PointerEventData data)
        {
            if (Tooltip is null)
            {
                return;
            }

            Tooltip tooltip = GUIManager.GetGUI(GUINames.TOOLTIP).GetComponent<Tooltip>();
            GUIManager.OpenGUI(GUINames.TOOLTIP);
            tooltip.Show(null, this.Tooltip);
        }

        public void OnPointerExit(PointerEventData data)
        {
            GUIManager.CloseGUI(GUINames.TOOLTIP);
        }

        public void ToggleMe()
        {
            if (Parent.Value < Delta && Selected == false)
            {
                return;
            }
            
            Selected = !Selected;
            if (Selected)
            {
                Image.color = SelectedColour;
            }
            else
            {
                Image.color = IdleColour;
            }
            OnSelect?.Invoke(this, new ValueChangedEventArgs()
            {
                Name = this.Name,
                NewValue = Selected ? Delta : -Delta,
                Delta = Selected ? Delta : -Delta 
            });
        }
    }
}