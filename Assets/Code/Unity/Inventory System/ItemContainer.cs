using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Unity.GUI;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity
{
    public class ItemContainer : GUIData
    {
        [SerializeField] protected KeyCode m_UseKey;
        [SerializeField] protected bool m_ClearAndFillOnEnable = false;
        [SerializeField] protected LayoutGroup m_SlotParent;
        [SerializeField] protected JoyItemSlot m_SlotPrefab;
        
        [SerializeField]
        protected bool m_CanDrag = false;
        public bool CanDrag => this.m_CanDrag;

        [SerializeField]
        protected bool m_CanDropItems = false;
        public bool CanDropItems => this.m_CanDropItems;

        [SerializeField]
        protected bool m_CanUseItems = false;
        public bool CanUseItems => this.m_CanUseItems;
        
        [SerializeField]
        protected bool m_UseContextMenu = false;
        public bool UseContextMenu => this.m_UseContextMenu;

        [SerializeField]
        protected bool m_ShowTooltips = false;
        public bool ShowTooltips => this.m_ShowTooltips;
        
        [SerializeField]
        protected bool m_MoveUsedItem = false;
        public bool MoveUsedItem => this.m_MoveUsedItem;
        
        protected List<JoyItemSlot> Slots { get; set; }

        public IJoyObject Owner { get; set; }

        public void OnEnable()
        {
            if (this.Owner is null || this.m_ClearAndFillOnEnable == false)
            {
                return;
            }
            if (this.Owner is IItemContainer container)
            {
                foreach (IItemInstance item in container.Contents)
                {
                    if (item is ItemInstance instance)
                    {
                        StackOrAdd(instance);
                    }
                }
            }
        }

        public void RemoveAllItems()
        {
            foreach (JoyItemSlot slot in this.Slots)
            {
                slot.Item = null;
            }
        }

        public List<Slot> GetRequiredSlots(IItemInstance item, JoyItemSlot preferedSlot = null)
        {
            List<Slot> slots = new List<Slot>();
            if (item == null)
            {
                return slots;
            }

            Dictionary<string, int> requiredSlots = new Dictionary<string, int>();

            foreach (string slot in item.ItemType.Slots)
            {
                if (requiredSlots.ContainsKey(slot))
                {
                    requiredSlots[slot] += 1;
                }
                else
                {
                    requiredSlots.Add(slot, 1);
                }
            }

            Dictionary<string, int> copySlots = new Dictionary<string, int>(requiredSlots);

            for (int i = 0; i < this.Slots.Count; i++)
            {
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

        public bool StackOrAdd(JoyItemSlot slot, IItemInstance item)
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
                    result = container.CanAddContents(instance) | container.Contains(instance);
                    if (result)
                    {
                        result &= container.AddContents(instance);
                    }
                }

                return result;
            }

            return false;
        }

        public bool StackOrAdd(IItemInstance item)
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
                    result = container.CanAddContents(instance) | container.Contains(instance);
                    if (result)
                    {
                        result &= container.AddContents(instance);
                    }
                }

                return result;
            }

            return false;
        }

        public bool RemoveItem(IItemInstance item, int amount)
        {
            if (this.Owner is null)
            {
                return false;
            }

            bool result = false;
            if (this.Owner is IItemContainer container)
            {
                if (container.Contents.Any(i =>
                    i.IdentifiedName.Equals(item.IdentifiedName, StringComparison.OrdinalIgnoreCase)))
                {
                    List<IItemInstance> matches = container.Contents.Where(itemInstance =>
                        itemInstance.IdentifiedName.Equals(item.IdentifiedName,
                            StringComparison.OrdinalIgnoreCase)).ToList();

                    result = true;
                    for (int i = 0; i < amount; i++)
                    {
                        result &= container.RemoveContents(matches[i]);
                    }
                }
            }

            return result;
        }

        public bool RemoveItem(IItemInstance item)
        {
            if (this.Owner is null)
            {
                return false;
            }

            if (item.GUID != this.Owner.GUID)
            {
                bool result = false;
                if (this.Owner is IItemContainer container)
                {
                    result = container.RemoveContents(item);
                }

                return result;
            }

            return false;
        }

        public bool RemoveItem(int index)
        {
            if (this.Owner is null)
            {
                return false;
            }

            bool result = false;
            if (index < this.Slots.Count
                && this.Owner is IItemContainer container)
            {
                JoyItemSlot slot = this.Slots[index];
                IItemInstance item = slot.Item;
                if (item is null)
                {
                    return result;
                }

                result = container.RemoveContents(item);
            }

            return result;
        }

        public new bool StackOrSwap(JoyItemSlot s1, JoyItemSlot s2)
        {
            IItemInstance i2 = s2.Item as IItemInstance;

            if (i2.GUID != s1.Container.Owner.GUID)
            {
                if (s1.Container.Owner is IItemContainer i1)
                {
                    if (i1.CanAddContents(i2) == false)
                    {
                        return false;
                    }

                    i1.AddContents(i2);
                }
            }
            
            return false;
        }
    }
}