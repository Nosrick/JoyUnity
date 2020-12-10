using DevionGames.InventorySystem;
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
        
        public void OnPointerEnter(PointerEventData data)
        {
            if (Tooltip is null)
            {
                return;
            }
            InventoryManager.UI.tooltip.Show(Tooltip);
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