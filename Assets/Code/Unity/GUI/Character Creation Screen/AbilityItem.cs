using JoyLib.Code.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(Image))]
    public class AbilityItem : MonoBehaviour
    {
        [SerializeField] protected ValueContainer Parent;
        [SerializeField] protected Color32 SelectedColour;
        [SerializeField] protected Color32 IdleColour;
        public bool Selected { get; protected set; }

        public string Name
        {
            get
            {
                return Text.text;
            }
            set
            {
                Text.text = value;
            }
        }
        protected Image Image { get; set; }
        protected TextMeshProUGUI Text { get; set; }

        public event ValueChangedEventHandler OnSelect;

        public void Awake()
        {
            Image = this.GetComponent<Image>();
            Image.color = IdleColour;
            Text = this.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void ToggleMe()
        {
            if (Parent.Value <= 0 && Selected == false)
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
                NewValue = Selected ? 1 : -1,
                Delta = Selected ? 1 : -1 
            });
        }
    }
}