using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Inventory
{
    public class JoyInventoryListItem : MonoBehaviour
    {
        private JoyInventoryItem m_InventoryItem;

        public JoyInventoryItem Item
        {
            get
            {
                return m_InventoryItem;
            }
            set
            {
                m_InventoryItem = value;
                ItemName = m_InventoryItem.name;
            }
        }

        public string ItemName
        {
            get;
            private set;
        }
    }
}
