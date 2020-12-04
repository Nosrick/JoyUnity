using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames;
using DevionGames.InventorySystem;
using DevionGames.InventorySystem.Restrictions;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using EquipmentRegion = DevionGames.InventorySystem.EquipmentRegion;

namespace JoyLib.Code.Unity.GUI
{
    public class EquipmentHandler : UIWidget
    {
        private IEntity m_Player;

        [SerializeField] protected GameObject m_SlotPrefab;
        [SerializeField] protected LayoutGroup m_Container;
        [SerializeField] protected MutableItemContainer m_EquipmentContainer;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetPlayer(IEntity player, bool clearSlots = false)
        {
            m_Player = player;
            CalculateSlots();
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

