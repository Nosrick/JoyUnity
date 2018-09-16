using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Inventory
{
    public class JoyInventoryListItem : MonoBehaviour
    {
        private ItemInstance m_InventoryItem;

        public ItemInstance Item
        {
            get
            {
                return m_InventoryItem;
            }
            set
            {
                m_InventoryItem = value;
                ItemName = m_InventoryItem.DisplayName;
                GetComponent<SpriteRenderer>().sprite = m_InventoryItem.Icon;
            }
        }

        public string ItemName
        {
            get;
            private set;
        }
    }
}
