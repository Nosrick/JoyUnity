using System;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    public class MutableItemContainer : ItemContainer
    {
        [SerializeField] protected bool ClearAndFillOnEnable = false;
        public IJoyObject Owner { get; set; }

        public void OnEnable()
        {
            if (this.Owner is null || this.ClearAndFillOnEnable == false)
            {
                return;
            }

            if (this.Owner is IItemContainer container)
            {
                this.RemoveItems();
                foreach (IItemInstance item in container.Contents)
                {
                    if (item is ItemInstance instance)
                    {
                        base.StackOrAdd(instance);
                    }
                }
            }
        }

        public override List<Slot> GetRequiredSlots(Item item, Slot preferedSlot = null)
        {
            List<Slot> slots = new List<Slot>();
            if (item == null || !(item is EquipmentItem))
            {
                return slots;
            }

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
                    foreach (KeyValuePair<string, int> pair in requiredSlots)
                    {
                        if (pair.Key.Equals(restriction.region.Name, StringComparison.OrdinalIgnoreCase)
                            && this.Slots[i].IsEmpty
                            && copySlots[pair.Key] > 0)
                        {
                            copySlots[pair.Key] -= 1;
                            slots.Add(this.m_Slots[i]);
                        }
                    }
                }
            }

            return slots;
        }

        public override bool StackOrAdd(Slot slot, Item item)
        {
            if (this.Owner is null)
            {
                return false;
            }

            if (item is ItemInstance instance && instance.GUID != this.Owner.GUID)
            {
                bool result = false;
                if (this.Owner is IItemContainer container)
                {
                    result = base.StackOrAdd(item);
                    if (result && this.UseReferences == false)
                    {
                        result &= container.AddContents(instance);
                    }
                    
                }

                return result;
            }

            return false;
        }

        public override bool StackOrAdd(Item item)
        {
            if (this.Owner is null)
            {
                return false;
            }

            if (item is ItemInstance instance && instance.GUID != this.Owner.GUID)
            {
                bool result = false;
                if (this.Owner is IItemContainer container)
                {
                    result = base.StackOrAdd(item);
                    if (result && this.UseReferences == false)
                    {
                        result &= container.AddContents(instance);
                    }
                }

                return result;
            }

            return false;
        }

        public override bool RemoveItem(Item item, int amount)
        {
            if (this.Owner is null)
            {
                return false;
            }

            bool result = false;
            if (this.Owner is IItemContainer container
                && item is ItemInstance instance)
            {
                if (container.Contents.Any(i =>
                    i.IdentifiedName.Equals(instance.IdentifiedName, StringComparison.OrdinalIgnoreCase)))
                {
                    List<IItemInstance> matches = container.Contents.Where(itemInstance =>
                        itemInstance.IdentifiedName.Equals(instance.IdentifiedName,
                            StringComparison.OrdinalIgnoreCase)).ToList();

                    result = true;
                    if (this.UseReferences == false)
                    {
                        for (int i = 0; i < amount; i++)
                        {
                            result &= container.RemoveContents(matches[i]);
                        }
                    }

                    result &= base.RemoveItem(item, amount);
                }
            }

            return result;
        }

        public override bool RemoveItem(Item item)
        {
            if (this.Owner is null)
            {
                return false;
            }

            if (item is ItemInstance instance && instance.GUID != this.Owner.GUID)
            {
                bool result = false;
                if (this.Owner is IItemContainer container)
                {
                    result = base.RemoveItem(item);
                    result &= container.RemoveContents(instance);
                }

                return result;
            }

            return false;
        }

        public override bool RemoveItem(int index)
        {
            if (this.Owner is null)
            {
                return false;
            }

            bool result = false;
            if (index < this.m_Slots.Count
                && this.Owner is IItemContainer container)
            {
                Slot slot = this.m_Slots[index];
                Item item = slot.ObservedItem;
                if (item is null)
                {
                    return result;
                }

                if (UseReferences)
                {
                    slot.ObservedItem = null;
                    result = item.ReferencedSlots.Remove(slot);
                    return result;
                }

                if (item is ItemInstance instance)
                {
                    result = base.RemoveItem(instance);
                    result &= container.RemoveContents(instance);
                }
            }

            return result;
        }

        public new ItemCollection Collection
        {
            get { return m_Collection; }

            set
            {
                if (value == null)
                {
                    return;
                }

                RemoveItems(true);
                value.Initialize();
                this.m_Collection = value;

                CurrencySlot[] currencySlots = GetSlots<CurrencySlot>();

                for (int i = 0; i < currencySlots.Length; i++)
                {
                    Currency defaultCurrency = currencySlots[i].GetDefaultCurrency();
                    Currency currency =
                        m_Collection.Where(x =>
                                typeof(Currency).IsAssignableFrom(x.GetType()) && x.Id == defaultCurrency.Id)
                            .FirstOrDefault() as Currency;
                    if (currency == null)
                    {
                        ReplaceItem(currencySlots[i].Index, defaultCurrency);
                    }
                    else
                    {
                        currencySlots[i].ObservedItem = currency;
                        currency.Slots.Add(currencySlots[i]);
                    }
                }

                for (int i = 0; i < this.m_Collection.Count; i++)
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

                    if (this.m_DynamicContainer)
                    {
                        Slot slot = CreateSlot();
                        slot.ObservedItem = item;
                        item.Slots.Add(slot);
                    }
                    else
                    {
                        Slot slot;
                        if (CanAddItem(item, out slot))
                        {
                            ReplaceItem(slot.Index, item);
                        }
                    }
                }
            }
        }

        public new List<Slot> Slots => this.m_Slots;
    }
}