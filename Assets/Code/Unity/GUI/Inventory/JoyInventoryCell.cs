using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI.Inventory
{
    public class JoyInventoryCell : MonoBehaviour, IDropHandler
    {
        private JoyInventoryItem m_InventoryItem;
        private string m_SlotName;

        public bool SetItem(JoyInventoryItem item)
        {
            if(item.Item.ItemType.Slot == m_SlotName)
            {
                m_InventoryItem = item;
                item.transform.position = m_InventoryItem.transform.position;
                return true;
            }

            return false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
