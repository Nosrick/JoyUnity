using System;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using DevionGames.InventorySystem.Restrictions;
using EquipmentRegion = DevionGames.InventorySystem.Restrictions.EquipmentRegion;

namespace JoyLib.Code.Unity
{
    public class MutableItemContainer : ItemContainer
    {
        public override List<Slot> GetRequiredSlots(Item item, Slot preferedSlot = null)
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
        
        public new ItemCollection Collection {

            get
            {
                return m_Collection;
            }
            
            set {
                if (value == null) {
                    return;
                }
                RemoveItems(true);
                value.Initialize();
                this.m_Collection = value;
              
                CurrencySlot[] currencySlots = GetSlots<CurrencySlot>();

                for (int i = 0; i < currencySlots.Length; i++) {
                    Currency defaultCurrency = currencySlots[i].GetDefaultCurrency();
                    Currency currency = m_Collection.Where(x => typeof(Currency).IsAssignableFrom(x.GetType()) && x.Id == defaultCurrency.Id).FirstOrDefault() as Currency;
                    if (currency == null) {
                        ReplaceItem(currencySlots[i].Index, defaultCurrency);
                    } else {
                        currencySlots[i].ObservedItem = currency;
                        currency.Slots.Add(currencySlots[i]);
                    }
                }

                for(int i=0; i < this.m_Collection.Count; i++)
                {
                    Item item = this.m_Collection[i];
                    if (item is Currency)
                        continue;

                    item.Slots.RemoveAll(x => x == null);
                    if (item.Slots.Count > 0)
                    {
                        for (int j = 0; j < item.Slots.Count; j++)
                        {
                            item.Slots[j].ObservedItem = item;
                        }
                        continue;
                    }
                    if (this.m_DynamicContainer) {
                        Slot slot = CreateSlot();
                        slot.ObservedItem = item;
                        item.Slots.Add(slot);
                    } else {
                        Slot slot;
                        if (CanAddItem(item, out slot)) {
                            ReplaceItem(slot.Index, item);
                        }
                    }

                }
            }
        }

        public new List<Slot> Slots => this.m_Slots;
    }
}