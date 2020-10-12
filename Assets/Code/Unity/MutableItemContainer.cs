using System;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using DevionGames.InventorySystem.Restrictions;
using EquipmentRegion = DevionGames.InventorySystem.Restrictions.EquipmentRegion;

namespace JoyLib.Code.Unity
{
    public class MutableItemContainer : ItemContainer
    {
        public override List<Slot> GetRequiredSlots(Item item)
        {
            List<Slot> slots = new List<Slot>();
            if (item == null || !(item is EquipmentItem)) { return slots; }
            EquipmentItem equipmentItem = item as EquipmentItem;
            
            Dictionary<string, int> requiredSlots = new Dictionary<string, int>();

            foreach (DevionGames.InventorySystem.EquipmentRegion region in equipmentItem.Region)
            {
                if (requiredSlots.ContainsKey(region.Name))
                {
                    requiredSlots[region.Name] += 1;
                }
                else
                {
                    requiredSlots.Add(region.Name, 1);
                }
            }
            
            Dictionary<string, int> copySlots = new Dictionary<string, int>(requiredSlots);

            for (int i = 0; i < this.m_Slots.Count; i++)
            {
                DevionGames.InventorySystem.Restrictions.EquipmentRegion restriction = 
                    this.m_Slots[i].GetComponent<DevionGames.InventorySystem.Restrictions.EquipmentRegion>();
                
                if (restriction != null)
                {
                    foreach(KeyValuePair<string, int> pair in requiredSlots)
                    {
                        if (pair.Key.Equals(restriction.region.Name, StringComparison.OrdinalIgnoreCase) && copySlots[pair.Key] > 0)
                        {
                            copySlots[pair.Key] -= 1;
                            slots.Add(this.m_Slots[i]);
                        }
                    }
                }
            }

            return slots;
        }

        public new List<Slot> Slots => this.m_Slots;
    }
}