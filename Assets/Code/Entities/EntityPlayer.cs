﻿using DevionGames.InventorySystem;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Unity;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class EntityPlayer : Entity
    {
        protected static ItemContainer m_Inventory;

        public EntityPlayer(Entity baseEntity) :
            base(baseEntity)
        {
            if (m_Inventory is null)
            {
                m_Inventory = WidgetUtility.Find<MutableItemContainer>("Inventory");
            }
        }
        
        public override bool EquipItem(string slotRef, ItemInstance itemRef)
        {
            return base.EquipItem(slotRef, itemRef);
        }

        public override bool AddContents(JoyObject actor)
        {
            if(actor is ItemInstance item)
            {
                m_Inventory.StackOrAdd(item.Item);
                return base.AddContents(actor);
            }

            return false;
        }

        public override bool RemoveItemFromBackpack(ItemInstance item)
        {
            m_Inventory.RemoveItem(item.Item);
            return base.RemoveItemFromBackpack(item);
        }
    }
}