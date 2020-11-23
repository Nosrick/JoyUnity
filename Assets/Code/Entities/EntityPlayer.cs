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

        public override bool AddContents(ItemInstance actor)
        {
            m_Inventory.StackOrAdd(actor);
            return base.AddContents(actor);
        }

        public override bool RemoveContents(ItemInstance item)
        {
            m_Inventory.RemoveItem(item);
            return base.RemoveContents(item);
        }
    }
}