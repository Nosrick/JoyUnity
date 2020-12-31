using System;
using JoyLib.Code.Entities;
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
            
            var slots = this.Player.Equipment.Slots;
            
            foreach ((string slot, var item) in slots)
            {
                JoyItemSlot slotInstance = Instantiate(
                    this.m_SlotPrefab,
                    this.m_SlotParent.transform);
                slotInstance.gameObject.SetActive(true);
                TextMeshProUGUI slotName = slotInstance.GetComponentInChildren<TextMeshProUGUI>();
                slotName.text = slot;
                slotInstance.Container = this;
                slotInstance.m_Slot = slot;
                slotInstance.Item = item;
                this.Slots.Add(slotInstance);
            }
        }
    }
}

