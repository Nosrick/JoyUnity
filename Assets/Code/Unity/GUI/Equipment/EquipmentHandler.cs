using System.Collections.Generic;
using JoyLib.Code.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class EquipmentHandler : GUIData
    {
        private IEntity m_Player;

        [SerializeField] protected GameObject m_SlotPrefab;
        [SerializeField] protected LayoutGroup m_Container;
        [SerializeField] protected ItemContainer m_EquipmentContainer;
        
        public bool Initialised { get; protected set; }

        public void SetPlayer(IEntity player, bool clearSlots = false)
        {
            m_Player = player;
            CalculateSlots(clearSlots);
        }

        protected virtual void CalculateSlots(bool clearSlots = false)
        {
            if (clearSlots)
            {
                this.m_EquipmentContainer.RemoveAllSlots();
            }
            
            List<string> slots = this.m_Player.Slots;
            
            for(int i = 0; i < slots.Count; i++)
            {
                GameObject slotInstance = GameObject.Instantiate(
                    this.m_SlotPrefab,
                    this.m_Container.transform);
                slotInstance.SetActive(this.m_Container.enabled);
                TextMeshProUGUI slotName = slotInstance.GetComponentInChildren<TextMeshProUGUI>();
                slotName.text = slots[i];
                JoyItemSlot slotScript = slotInstance.GetComponent<JoyItemSlot>();
                slotScript.Container = this.m_EquipmentContainer;
                slotScript.m_Slot = slots[i];
            }

            this.Initialised = true;
        }
    }
}

