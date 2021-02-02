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
        protected List<Tuple<string, long>> m_Slots;

        public IEnumerable<Tuple<string, IItemInstance>> Slots
        {
            get
            {
                return this.m_Slots
                    .Where(tuple => tuple.Item2 > 0)
                    .Select(tuple =>
                    new Tuple<string, IItemInstance>(tuple.Item1,
                        GlobalConstants.GameManager.ItemHandler.GetItem(tuple.Item2)));
            }
        }

        public override int HitPointsRemaining => 1;
        public override int HitPoints => 1;

        public IEnumerable<IItemInstance> Contents =>
            this.Slots
                .Select(tuple => tuple.Item2)
                .Distinct();

        public EquipmentStorage()
        {
            this.m_Slots = new List<Tuple<string, long>>();
            this.JoyName = "Equipment";
        }

        public EquipmentStorage(IEnumerable<string> slots)
        {
            this.m_Slots = new List<Tuple<string, long>>();
            foreach (string slot in slots)
            {
                this.m_Slots.Add(new Tuple<string, long>(slot, -1));
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

            foreach (Tuple<string, long> tuple in this.m_Slots)
            {
                foreach (KeyValuePair<string, int> pair in requiredSlots)
                {
                    if (pair.Key.Equals(tuple.Item1, StringComparison.OrdinalIgnoreCase)
                        && tuple.Item2 == -1
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

        public virtual IItemInstance GetSlotContents(string slot)
        {
            Tuple<string, long> slotTuple =
                this.m_Slots.FirstOrDefault(tuple => tuple.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase));
            if (slotTuple is null == false && slotTuple.Item2 > 0)
            {
                return GlobalConstants.GameManager.ItemHandler.GetItem(slotTuple.Item2);
            }

            return null;
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
                         && s.Item2 == -1);
                this.m_Slots[index] = new Tuple<string, long>(slot, actor.GUID);
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
            if (this.m_Slots.All(s => actor.GUID.Equals(s.Item2) == false))
            {
                return false;
            }
            List<Tuple<string, long>> slots = this.m_Slots.Where(s => actor.GUID.Equals(s.Item2)).ToList();
            if (slots.Any() == false)
            {
                return false;
            }

            foreach (Tuple<string, long> slot in slots)
            {
                int index = this.m_Slots.IndexOf(slot);
                this.m_Slots[index] = new Tuple<string, long>(slot.Item1, -1);
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
            this.m_Slots.Add(new Tuple<string, long>(slot, -1));

            return this.m_Slots.Count(s => s.Item1.Equals(slot, StringComparison.OrdinalIgnoreCase));
        }

        public virtual string ContentString { get; }
        public virtual event ItemRemovedEventHandler ItemRemoved;
        public virtual event ItemAddedEventHandler ItemAdded;
    }
}