using System.Collections.Generic;
using DevionGames.InventorySystem;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class EquipmentHandler : UIWidget
    {
        private IEntity m_Player;

        [SerializeField] protected GameObject m_SlotPrefab;
        [SerializeField] protected LayoutGroup m_Container;
        [SerializeField] protected MutableItemContainer m_EquipmentContainer;

        public void SetPlayer(IEntity player, bool clearSlots = false)
        {
            m_Player = player;
            CalculateSlots(clearSlots);
        }

        private void CalculateSlots(bool clearSlots = false)
        {
            if (clearSlots)
            {
                m_EquipmentContainer.Slots.Clear();
            }
            else if (!clearSlots && m_EquipmentContainer.Slots.Count > 0)
            {
                return;
            }
            
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in m_Container.transform)
            {
                children.Add(child.gameObject);
            }
            children.ForEach(child => Destroy(child));
            
            List<string> slots = m_Player.Slots;
            
            for(int i = 0; i < slots.Count; i++)
            {
                GameObject slotInstance = GameObject.Instantiate(
                    m_SlotPrefab,
                    m_Container.transform);
                slotInstance.SetActive(m_Container.enabled);
                TextMeshProUGUI slotName = slotInstance.GetComponentInChildren<TextMeshProUGUI>();
                slotName.text = slots[i];
                ItemSlot slotScript = slotInstance.GetComponent<ItemSlot>();
                slotScript.Container = m_EquipmentContainer;
                DevionGames.InventorySystem.Restrictions.EquipmentRegion region = slotInstance.GetComponent<DevionGames.InventorySystem.Restrictions.EquipmentRegion>();
                DevionGames.InventorySystem.EquipmentRegion regionSO =
                    ScriptableObject.CreateInstance<DevionGames.InventorySystem.EquipmentRegion>();
                regionSO.Name = slots[i];
                region.region = regionSO;
                slotScript.restrictions.Add(region);
            }

            m_EquipmentContainer.RefreshSlots();
        }
    }
}

