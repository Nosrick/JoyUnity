using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class EquipmentHandler : ItemContainer
    {
        private IEntity m_Player;

        public void Awake()
        {
            this.Owner = new VirtualStorage();
        }

        public void SetPlayer(IEntity player, bool clearSlots = false)
        {
            m_Player = player;
            CalculateSlots(clearSlots);
        }

        protected virtual void CalculateSlots(bool clearSlots = false)
        {
            if (clearSlots)
            {
                this.RemoveAllSlots();
            }
            
            List<string> slots = this.m_Player.Slots;
            
            for(int i = 0; i < slots.Count; i++)
            {
                JoyItemSlot slotInstance = GameObject.Instantiate(
                    this.m_SlotPrefab,
                    this.transform);
                slotInstance.gameObject.SetActive(this.enabled);
                TextMeshProUGUI slotName = slotInstance.GetComponentInChildren<TextMeshProUGUI>();
                slotName.text = slots[i];
                JoyItemSlot slotScript = slotInstance.GetComponent<JoyItemSlot>();
                slotScript.Container = this;
                slotScript.m_Slot = slots[i];
            }
        }
    }
}

