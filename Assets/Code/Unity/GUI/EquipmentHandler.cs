using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames;
using DevionGames.InventorySystem;
using DevionGames.InventorySystem.Restrictions;
using JoyLib.Code.Entities;
using Lean.Gui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EquipmentRegion = DevionGames.InventorySystem.EquipmentRegion;

namespace JoyLib.Code.Unity.GUI
{
    public class EquipmentHandler : MonoBehaviour
    {
        private IEntity m_Player;

        private GameObject m_SlotPrefab;
        private GameObject m_Grid;
        private MutableItemContainer m_EquipmentContainer;

        public void Awake()
        {
            m_SlotPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/Equipment Slot");
            m_EquipmentContainer = GameObject.Find("Equipment").GetComponent<MutableItemContainer>();
            m_Grid = GameObject.Find("EquipmentSlots");
        }

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
            foreach (Transform child in m_Grid.transform)
            {
                children.Add(child.gameObject);
            }
            children.ForEach(child => Destroy(child));
            
            List<string> slots = m_Player.Slots;
            
            for(int i = 0; i < slots.Count; i++)
            {
                GameObject slotInstance = GameObject.Instantiate(
                    m_SlotPrefab,
                    m_Grid.transform);
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
            GameObject equipmentSlots = this.gameObject.FindChild("EquipmentSlots", true);
            LeanConstrainAnchoredPosition constrain = equipmentSlots.GetComponent<LeanConstrainAnchoredPosition>();
            GridLayoutGroup grid = equipmentSlots.GetComponent<GridLayoutGroup>();
            constrain.VerticalMax = (grid.transform.childCount * (grid.spacing.y + grid.cellSize.y)) / 2;
        }
    }
}

