using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Events;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity
{
    public class ItemContainer : GUIData
    {
        [SerializeField] protected KeyCode m_UseKey;
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

        public List<IItemInstance> Contents
        {
            get
            {
                if (this.Owner is IItemContainer container)
                {
                    return container.Contents;
                }

                return new List<IItemInstance>();
            }
        }

        public void OnEnable()
        {
            if (this.Owner is null)
            {
                return;
            }
            if (this.Owner is IItemContainer container)
            {
                if (this.Slots is null)
                {
                    this.Slots = this.GetComponentsInChildren<JoyItemSlot>().ToList();
                }

                if (this.Slots.Count < container.Contents.Count)
                {
                    for (int i = this.Slots.Count; i < container.Contents.Count; i++)
                    {
                        this.Slots.Add(Instantiate(this.m_SlotPrefab, this.m_SlotParent.transform, false).GetComponent<JoyItemSlot>());
                    }
                }

                foreach (JoyItemSlot slot in this.Slots)
                {
                    slot.Container = this;
                }
                
                for(int i = 0; i < container.Contents.Count; i++)
                {
                    this.Slots[i].Container = this;
                    this.Slots[i].Item = container.Contents[i];
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

        public List<JoyItemSlot> GetRequiredSlots(IItemInstance item, JoyItemSlot preferedSlot = null)
        {
            List<JoyItemSlot> slots = new List<JoyItemSlot>();
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
                if (this.Slots[i].m_Slot.IsNullOrEmpty() == false)
                {
                    foreach (KeyValuePair<string, int> pair in requiredSlots)
                    {
                        if (pair.Key.Equals(this.Slots[i].m_Slot, StringComparison.OrdinalIgnoreCase)
                            && this.Slots[i].IsEmpty
                            && copySlots[pair.Key] > 0)
                        {
                            copySlots[pair.Key] -= 1;
                            slots.Add(this.Slots[i]);
                        }
                    }
                }
            }

            return slots;
        }

        public bool AddSlot(JoyItemSlot slot, bool pool = true)
        {
            if (pool)
            {
                if (this.Slots.Any(itemSlot => itemSlot.isActiveAndEnabled == false))
                {
                    JoyItemSlot poolSlot = this.Slots.First(itemSlot => itemSlot.isActiveAndEnabled == false);
                    poolSlot.gameObject.SetActive(true);
                    poolSlot.Item = slot.Item;
                    return true;
                }

                return false;
            }
            else
            {
                this.Slots.Add(slot);
                slot.transform.parent = this.m_SlotParent.transform;
                return true;
            }
        }

        public bool RemoveSlot(JoyItemSlot slot, bool pool = true)
        {
            bool result = this.Slots.Any(itemSlot => itemSlot == slot);
            if (result)
            {
                if (pool)
                {
                    JoyItemSlot foundSlot = this.Slots.First(itemSlot => itemSlot == slot);
                    foundSlot.gameObject.SetActive(false);
                    result &= true;
                }
                else
                {
                    result &= this.Slots.Remove(slot);
                    DestroyImmediate(slot);
                }
            }

            return result;
        }

        public bool RemoveAllSlots(bool pool = true)
        {
            if (pool)
            {
                this.Slots.ForEach(slot => slot.gameObject.SetActive(false));
                return true;
            }
            else
            {
                this.Slots.ForEach(slot => Destroy(slot));
                return true;
            }
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
                    if(container.CanAddContents(instance) | container.Contains(instance))
                    {
                        result &= container.AddContents(instance);
                        slot.Item = instance;
                        this.OnAddItem?.Invoke(container, new ItemChangedEventArgs() { Item = item });
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
                    if (this.Slots.All(slot => slot.Item is null == false))
                    {
                        return result;
                    }
                    
                    result = container.CanAddContents(instance) | container.Contains(instance);
                    if (result)
                    {
                        result &= container.AddContents(instance);
                    }

                    if (result)
                    {
                        this.Slots.First(slot => slot.Item is null).Item = item;
                        this.OnAddItem?.Invoke(container, new ItemChangedEventArgs() { Item = item });
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
                        result &= this.RemoveItem(matches[i]);
                    }
                
                    if (result)
                    {
                        this.OnRemoveItem?.Invoke(container, new ItemChangedEventArgs() { Item = item });
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
                    if (container.Contains(item) == false || this.Slots.Any(slot => !(slot.Item is null) && slot.Item.GUID == item.GUID) == false)
                    {
                        return result;
                    }
                    
                    result = container.RemoveContents(item);
                
                    if (result)
                    {
                        this.Slots.First(slot => !(slot.Item is null) && slot.Item.GUID == item.GUID).Item = null;
                        this.OnRemoveItem?.Invoke(container, new ItemChangedEventArgs() { Item = item });
                    }
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
                if (result)
                {
                    slot.Item = null;
                    this.OnRemoveItem?.Invoke(container, new ItemChangedEventArgs() { Item = item });
                }
            }

            return result;
        }

        public bool StackOrSwap(JoyItemSlot s1, JoyItemSlot s2)
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

                    if (i1.AddContents(i2))
                    {
                        this.StackOrAdd(s1, i2);
                        this.OnAddItem?.Invoke(i1, new ItemChangedEventArgs() { Item = i2 });
                        return true;
                    }
                }
            }
            
            return false;
        }

        public event ItemAddedEventHandler OnAddItem;
        public event ItemRemovedEventHandler OnRemoveItem;
    }
}