﻿using JoyLib.Code.Entities.Items;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI.Inventory
{
    public class JoyInventoryCell : MonoBehaviour, IDropHandler
    {
        private ItemInstance m_InventoryItem;
        private string m_SlotName;

        public bool SetItem(ItemInstance item)
        {
            if(item.ItemType.Slot == m_SlotName)
            {
                m_InventoryItem = item;
                m_SlotName = m_InventoryItem.ItemType.Slot;
                GetComponent<SpriteRenderer>().sprite = m_InventoryItem.Icon;
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
