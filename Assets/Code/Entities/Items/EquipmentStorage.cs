using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Events;

namespace JoyLib.Code.Entities.Items
{
    public class EquipmentStorage : JoyObject, IItemContainer
    {
        protected List<Tuple<string, IItemInstance>> m_Slots;
        public IReadOnlyList<Tuple<string, IItemInstance>> Slots => this.m_Slots.AsReadOnly();

        public IEnumerable<IItemInstance> Contents =>
            this.m_Slots.Where(tuple => tuple.Item2 is null == false)
                .Select(tuple => tuple.Item2);

        public EquipmentStorage()
        {
            this.m_Slots = new List<Tuple<string, IItemInstance>>();
        }

        public EquipmentStorage(IEnumerable<string> slots)
        {
            this.m_Slots = new List<Tuple<string, IItemInstance>>();
            foreach (string slot in slots)
            {
                this.m_Slots.Add(new Tuple<string, IItemInstance>(slot, null));
            }
        }
        
        public bool Contains(IItemInstance actor)
        {
            return this.m_Slots.Any(tuple => actor.Equals(tuple.Item2));
        }

        public IEnumerable<IItemInstance> GetSlotContents(string slot)
        {
            return this.m_Slots.Where(tuple => tuple.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase))
                .Select(tuple => tuple.Item2);
        }

        public bool CanAddContents(IItemInstance actor)
        {
            return !this.Contains(actor) 
                   && this.m_Slots.Any(slot => 
                       actor.ItemType.Slots.Any(s => s.Equals(slot.Item1, StringComparison.OrdinalIgnoreCase))
                       && slot.Item2 is null);
        }

        public bool AddContents(IItemInstance actor)
        {
            if (!this.CanAddContents(actor))
            {
                return false;
            }

            string slot = this.m_Slots.First(s =>
                actor.ItemType.Slots.Any(i => i.Equals(s.Item1, StringComparison.OrdinalIgnoreCase))
                && s.Item2 is null).Item1;

            return this.AddContents(actor, slot);
        }

        public bool AddContents(IItemInstance actor, string slot)
        {
            if (!this.CanAddContents(actor))
            {
                return false;
            }

            int index = this.m_Slots.FindIndex(tuple =>
                tuple.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase)
                && tuple.Item2 is null);

            if (index == -1)
            {
                return false;
            }
            
            this.m_Slots.RemoveAt(index);
            this.m_Slots.Insert(index, new Tuple<string, IItemInstance>(slot, actor));
            
            this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
            return true;
        }

        public bool AddContents(IEnumerable<IItemInstance> actors)
        {
            bool result = actors.Aggregate(true, (current, actor) => current & this.AddContents(actor));

            return result;
        }

        public bool RemoveContents(IItemInstance actor)
        {
            if (this.m_Slots.All(s => actor.Equals(s.Item2) == false))
            {
                return false;
            }
            int index = this.m_Slots.FindIndex(s => actor.Equals(s.Item2));
            if (index == -1)
            {
                return false;
            }

            string slot = this.m_Slots[index].Item1;
            this.m_Slots.RemoveAt(index);
            this.m_Slots.Insert(index, new Tuple<string, IItemInstance>(slot, null));
            this.ItemRemoved?.Invoke(this, new ItemChangedEventArgs() { Item = actor });

            return true;
        }

        public bool RemoveContents(string slot, IItemInstance actor)
        {
            if (this.m_Slots.All(s => s.Item2.GUID != actor.GUID
                                    && s.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase) == false))
            {
                return false;
            }

            int index = this.m_Slots.FindIndex(tuple =>
                                                tuple.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase)
                                                && tuple.Item2.Equals(actor));
            if (index == -1)
            {
                return false;
            }
            this.m_Slots.RemoveAt(index);
            this.ItemRemoved?.Invoke(this, new ItemChangedEventArgs { Item = actor });
            return true;
        }

        public void Clear()
        {
            List<IItemInstance> copy = new List<IItemInstance>(this.Contents);
            foreach (IItemInstance item in copy)
            {
                this.RemoveContents(item);
            }
        }

        public int AddSlot(string slot)
        {
            this.m_Slots.Add(new Tuple<string, IItemInstance>(slot, null));

            return this.m_Slots.Count(s => s.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase));
        }

        public string ContentString { get; }
        public event ItemRemovedEventHandler ItemRemoved;
        public event ItemAddedEventHandler ItemAdded;
    }
}