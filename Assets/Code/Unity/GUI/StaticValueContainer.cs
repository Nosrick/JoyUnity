using JoyLib.Code.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class StaticValueContainer : MonoBehaviour, IValueContainer, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected TextMeshProUGUI m_ValueText;
        [SerializeField] protected TextMeshProUGUI m_NameText;
        
        protected string m_Tooltip;
        protected int m_Value;
        protected string m_Name;

        protected static IGUIManager GUIManager { get; set; }

        public void OnEnable()
        {
            if (GUIManager is null)
            {
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (this.Tooltip is null)
            {
                return;
            }

            GUIData tooltip = GUIManager.GetGUI(GUINames.TOOLTIP);
        }

        public void OnPointerExit(PointerEventData data)
        {
            InventoryManager.UI.tooltip.Close();
        }
        
        public int DecreaseValue(int delta = 1)
        {
            return Value;
        }

        public int IncreaseValue(int delta = 1)
        {
            return Value;
        }

        public int DirectValueSet(int newValue)
        {
            Value = newValue;
            return Value;
        }

        public int Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                m_ValueText.text = m_Value.ToString();
            }
        }

        public string Name
        {
            get => m_Name;
            set
            {
                m_Name = value;
                m_NameText.text = m_Name;
            }
        }
        
        public string Tooltip
        {
            get => m_Tooltip;
            set
            {
                m_Tooltip = value;
                if (InventoryManager.UI.tooltip.IsVisible)
                {
                    InventoryManager.UI.tooltip.Close();
                    InventoryManager.UI.tooltip.Show(m_Tooltip);
                }
            }
        }
        
        public event ValueChangedEventHandler ValueChanged;
    }
}