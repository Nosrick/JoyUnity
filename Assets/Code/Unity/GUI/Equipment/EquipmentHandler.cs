using System.Collections.Generic;
using Castle.Core.Internal;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using TMPro;

namespace JoyLib.Code.Unity.GUI
{
    public class EquipmentHandler : ItemContainer
    {
        protected IEntity Player { get; set; }

        public void SetPlayer(IEntity player, bool clearSlots = false)
        {
            this.Player = player;
            this.Owner = this.Player.Equipment;
            this.CalculateSlots(clearSlots);
        }

        protected virtual void CalculateSlots(bool clearSlots = false)
        {
            if (clearSlots)
            {
                this.RemoveAllSlots();
            }

            foreach (JoyItemSlot slot in this.Slots)
            {
                slot.gameObject.SetActive(false);
            }
            
            var slots = this.Player.Equipment.Slots;

            for (int i = this.Slots.Count; i < slots.Count; i++)
            {
                JoyItemSlot slotInstance = Instantiate(
                    this.m_SlotPrefab,
                    this.m_SlotParent.transform);
                this.Slots.Add(slotInstance);
            }
            
            for (int i = 0; i < slots.Count; i++)
            {
                this.Slots[i].gameObject.SetActive(true);
                TextMeshProUGUI slotName = this.Slots[i].GetComponentInChildren<TextMeshProUGUI>();
                slotName.text = slots[i].Item1;
                this.Slots[i].name = slots[i].Item1;
                this.Slots[i].Container = this;
                this.Slots[i].m_Slot = slots[i].Item1;
                this.Slots[i].Item = slots[i].Item2;
            }
        }

        protected override bool StackOrAdd(IEnumerable<JoyItemSlot> slots, IItemInstance item)
        {
            return !slots.IsNullOrEmpty() && base.StackOrAdd(slots, item);
        }
    }
}

