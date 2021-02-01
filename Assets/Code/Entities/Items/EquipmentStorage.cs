using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Events;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Items
{
    [Serializable]
    public class EquipmentStorage : JoyObject, IItemContainer
    {
        [OdinSerialize]
        protected List<Tuple<string, IItemInstance>> m_Slots;
        
        public IReadOnlyList<Tuple<string, IItemInstance>> Slots => this.m_Slots.AsReadOnly();

        public override int HitPointsRemaining => 1;
        public override int HitPoints => 1;

        public IEnumerable<IItemInstance> Contents =>
            this.m_Slots.Where(tuple => tuple.Item2 is null == false)
                .Select(tuple => tuple.Item2)
                .Distinct();

        public EquipmentStorage()
        {
            this.m_Slots = new List<Tuple<string, IItemInstance>>();
            this.JoyName = "Equipment";
        }

        public EquipmentStorage(IEnumerable<string> slots)
        {
            this.m_Slots = new List<Tuple<string, IItemInstance>>();
            foreach (string slot in slots)
            {
                this.m_Slots.Add(new Tuple<string, IItemInstance>(slot, null));
            }
            this.JoyName = "Equipment";
        }
        
        protected virtual IEnumerable<string> GetRequiredSlots(IItemInstance item)
        {
            List<string> slots = new List<string>();
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

            foreach (Tuple<string, IItemInstance> tuple in this.Slots)
            {
                foreach (KeyValuePair<string, int> pair in requiredSlots)
                {
                    if (pair.Key.Equals(tuple.Item1, StringComparison.OrdinalIgnoreCase)
                        && tuple.Item2 is null
                        && copySlots[pair.Key] > 0)
                    {
                        copySlots[pair.Key] -= 1;
                        slots.Add(tuple.Item1);
                    }
                }
            }

            return slots;
        }
        
        public virtual bool Contains(IItemInstance actor)
        {
            return this.m_Slots.Any(tuple => actor.Equals(tuple.Item2));
        }

        public virtual IEnumerable<IItemInstance> GetSlotContents(string slot)
        {
            return this.m_Slots.Where(tuple => tuple.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase))
                .Select(tuple => tuple.Item2);
        }

        public virtual bool CanAddContents(IItemInstance actor)
        {
            int slots = this.GetRequiredSlots(actor).Count();
            return !this.Contains(actor)
                   && slots > 0
                   && slots == actor.ItemType.Slots.Length;
        }

        public virtual bool AddContents(IItemInstance actor)
        {
            if (!this.CanAddContents(actor))
            {
                return false;
            }

            IEnumerable<string> slots = this.GetRequiredSlots(actor);

            return this.AddContents(actor, slots);
        }

        public virtual bool AddContents(IItemInstance actor, IEnumerable<string> slots)
        {
            if (!this.CanAddContents(actor))
            {
                return false;
            }

            List<string> openSlots = this.GetRequiredSlots(actor).ToList();
            List<string> slotList = slots.ToList();
            int matches = 0;
            foreach (string slot in slotList)
            {
                int index = openSlots.FindIndex(s => s.Equals(slot, StringComparison.OrdinalIgnoreCase));
                if (index > -1)
                {
                    openSlots.RemoveAt(index);
                    matches += 1;
                }

                if (matches == slotList.Count)
                {
                    break;
                }
            }

            if (matches != slotList.Count)
            {
                return false;
            }

            foreach (string slot in slotList)
            {
                int index = this.m_Slots.FindIndex(
                    s => s.Item1.Equals(slot, StringComparison.InvariantCulture)
                         && s.Item2 is null);
                this.m_Slots[index] = new Tuple<string, IItemInstance>(slot, actor);
            }
            
            this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
            return true;
        }

        public virtual bool AddContents(IEnumerable<IItemInstance> actors)
        {
            bool result = actors.Aggregate(true, (current, actor) => current & this.AddContents(actor));

            return result;
        }

        public virtual bool RemoveContents(IItemInstance actor)
        {
            if (this.m_Slots.All(s => actor.Equals(s.Item2) == false))
            {
                return false;
            }
            List<Tuple<string, IItemInstance>> slots = this.m_Slots.Where(s => actor.Equals(s.Item2)).ToList();
            if (slots.Any() == false)
            {
                return false;
            }

            foreach (Tuple<string, IItemInstance> slot in slots)
            {
                int index = this.m_Slots.IndexOf(slot);
                this.m_Slots[index] = new Tuple<string, IItemInstance>(slot.Item1, null);
            }
            this.ItemRemoved?.Invoke(this, new ItemChangedEventArgs { Item = actor });

            return true;
        }
        
        public virtual bool RemoveContents(IEnumerable<IItemInstance> actors)
        {
            return actors.Aggregate(true, (current, actor) => current & this.RemoveContents(actor));
        }

        public virtual void Clear()
        {
            List<IItemInstance> copy = new List<IItemInstance>(this.Contents);
            foreach (IItemInstance item in copy)
            {
                this.RemoveContents(item);
            }
        }

        public virtual int AddSlot(string slot)
        {
            this.m_Slots.Add(new Tuple<string, IItemInstance>(slot, null));

            return this.m_Slots.Count(s => s.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase));
        }

        public virtual string ContentString { get; }
        public virtual event ItemRemovedEventHandler ItemRemoved;
        public virtual event ItemAddedEventHandler ItemAdded;
    }
}