using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Events;

namespace JoyLib.Code.Entities.Items
{
    public class EquipmentStorage : JoyObject, IItemContainer
    {
        protected List<Tuple<string, IItemInstance>> Slots { get; set; }

        public IEnumerable<IItemInstance> Contents =>
            this.Slots.Where(tuple => tuple.Item2 is null == false)
                .Select(tuple => tuple.Item2);

        public EquipmentStorage()
        {
            this.Slots = new List<Tuple<string, IItemInstance>>();
        }
        
        public bool Contains(IItemInstance actor)
        {
            return this.Slots.Any(tuple => actor.Equals(tuple.Item2));
        }

        public IEnumerable<IItemInstance> GetSlotContents(string slot)
        {
            return this.Slots.Where(tuple => tuple.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase))
                .Select(tuple => tuple.Item2);
        }

        public bool CanAddContents(IItemInstance actor)
        {
            return !this.Contains(actor) 
                   && this.Slots.Any(slot => 
                       actor.ItemType.Slots.Any(s => s.Equals(slot.Item1, StringComparison.OrdinalIgnoreCase))
                       && slot.Item2 is null);
        }

        public bool AddContents(IItemInstance actor)
        {
            if (!this.CanAddContents(actor))
            {
                return false;
            }

            string slot = this.Slots.First(s =>
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

            int index = this.Slots.FindIndex(tuple =>
                tuple.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase)
                && tuple.Item2 is null);

            if (index == -1)
            {
                return false;
            }
            
            this.Slots.RemoveAt(index);
            this.Slots.Insert(index, new Tuple<string, IItemInstance>(slot, actor));
            
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
            if (this.Slots.All(s => s.Item2.GUID != actor.GUID))
            {
                return false;
            }
            int index = this.Slots.FindIndex(s => s.Item2.GUID == actor.GUID);
            if (index == -1)
            {
                return false;
            }
            
            this.Slots.RemoveAt(index);
            this.ItemRemoved?.Invoke(this, new ItemChangedEventArgs() { Item = actor });

            return true;
        }

        public bool RemoveContents(string slot, IItemInstance actor)
        {
            if (this.Slots.All(s => s.Item2.GUID != actor.GUID
                                    && s.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase) == false))
            {
                return false;
            }

            int index = this.Slots.FindIndex(tuple =>
                                                tuple.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase)
                                                && tuple.Item2.Equals(actor));
            if (index == -1)
            {
                return false;
            }
            this.Slots.RemoveAt(index);
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
            this.Slots.Add(new Tuple<string, IItemInstance>(slot, null));

            return this.Slots.Count(s => s.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase));
        }

        public string ContentString { get; }
        public event ItemRemovedEventHandler ItemRemoved;
        public event ItemAddedEventHandler ItemAdded;
    }
}