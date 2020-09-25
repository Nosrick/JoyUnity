using System;
using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class EquipmentHandler : MonoBehaviour
    {
        private Entity m_Player;

        private GameObject m_SlotPrefab;
        private GameObject m_Grid;
        private ItemContainer m_EquipmentContainer;

        public void Awake()
        {
            m_SlotPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/Equipment Slot");
            m_EquipmentContainer = GameObject.Find("EquipmentCanvas").GetComponent<ItemContainer>();
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

        public void SetPlayer(Entity player)
        {
            m_Player = player;
            CalculateSlots();
        }

        private void CalculateSlots()
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in m_Grid.transform)
            {
                children.Add(child.gameObject);
            }
            children.ForEach(child => Destroy(child));
            
            string[] slots = m_Player.Slots;
            
            for(int i = 0; i < slots.Length; i++)
            {
                GameObject slotInstance = GameObject.Instantiate(
                    m_SlotPrefab,
                    m_Grid.transform);
                TextMeshProUGUI slotName = slotInstance.GetComponentInChildren<TextMeshProUGUI>();
                slotName.text = slots[i];
                ItemSlot slotScript = slotInstance.GetComponent<ItemSlot>();
                slotScript.Container = m_EquipmentContainer;
            }
        }
    }
}

