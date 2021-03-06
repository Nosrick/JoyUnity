﻿using JoyLib.Code.Entities.Items;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI.Inventory
{
    public class JoyInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private ItemInstance m_Item;

        public ItemInstance Item
        {
            get
            {
                return m_Item;
            }
            set
            {
                m_Item = value;
                GetComponent<SpriteRenderer>().sprite = m_Item.Icon;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
