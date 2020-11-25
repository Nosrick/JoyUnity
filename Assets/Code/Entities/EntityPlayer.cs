using System.Collections.Generic;
using DevionGames.InventorySystem;
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
            bool result = true;
            result &= m_Inventory.StackOrAdd(actor);
            return result && base.AddContents(actor);
        }

        public override bool AddContents(IEnumerable<ItemInstance> actors)
        {
            bool result = true;
            foreach (ItemInstance item in actors)
            {
                result &= m_Inventory.StackOrAdd(item);
            }
            
            return result && base.AddContents(actors);
        }

        public override bool RemoveContents(ItemInstance item)
        {
            bool result = true;
            result &= m_Inventory.RemoveItem(item);
            return result && base.RemoveContents(item);
        }

        public override void Clear()
        {
            m_Inventory.RemoveItems();
            base.Clear();
        }
    }
}