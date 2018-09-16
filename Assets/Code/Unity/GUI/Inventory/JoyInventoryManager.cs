using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Inventory
{
    public class JoyInventoryManager : MonoBehaviour
    {
        private Entity m_Player;
        private GameObject m_SlotPrefab;
        private GameObject m_ListPrefab;
        private GameObject m_ItemPrefab;

        //The quad representing the background
        private GameObject m_Background;

        private const int SLOT_SCALE = 3;
        private const int INVENTORY_SCALE = 5;
        private const int QUAD_SCALE = 25;

        public void Start()
        {
            m_SlotPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventorySlot");
            m_ListPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventoryListItem");
            m_ItemPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventoryItem");

            m_Background = GameObject.Find("InventoryBackgroundQuad");

            float quadHeight = Camera.main.orthographicSize * 2.0f;
            float quadWidth = quadHeight * Screen.width / Screen.height;

            m_Background.transform.localScale = new Vector3(quadWidth * QUAD_SCALE, quadHeight * QUAD_SCALE, 1);

            if (m_Player != null)
            {
                DoAll();
            }
        }

        public void SetPlayer(Entity player)
        {
            m_Player = player;
        }

        public void DoAll()
        {
            DoSlots();
            DoInventory();
        }

        public void DoSlots()
        {
            int i = 0;
            GameObject slots = GameObject.Find("Slots");
            foreach (string slot in m_Player.Slots)
            {
                GameObject gameObject = Instantiate(m_SlotPrefab);
                JoyInventoryCell cell = gameObject.GetComponent<JoyInventoryCell>();

                TextMeshProUGUI textMesh = cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                textMesh.text = slot + ":";

                ItemInstance itemInstance = m_Player.GetEquipment(slot);
                if(itemInstance != null)
                { 
                    cell.SetItem(itemInstance);
                }

                RectTransform transform = this.GetComponent<RectTransform>();
                gameObject.transform.position = new Vector3(-transform.sizeDelta.x / 3 - 100, (transform.sizeDelta.y / 2) - i * 32 - 32);
            
                gameObject.transform.SetParent(slots.transform, false);
                gameObject.transform.localScale = new Vector3(SLOT_SCALE, SLOT_SCALE, SLOT_SCALE);

                i += 1;
            }
        }

        public void DoInventory()
        {
            GameObject inventory = GameObject.Find("Inventory");
            while(inventory.transform.childCount != 0)
            {
                GameObject.Destroy(inventory.transform.GetChild(0));
            }

            int i = 0;
            foreach (ItemInstance item in m_Player.Backpack)
            {
                GameObject gameObject = Instantiate(m_ListPrefab);
                JoyInventoryListItem list = gameObject.transform.GetChild(0).GetComponent<JoyInventoryListItem>();

                list.Item = item;

                TextMeshProUGUI textMesh = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                textMesh.text = item.DisplayName;

                RectTransform transform = this.GetComponent<RectTransform>();
                gameObject.transform.position = new Vector3(-transform.sizeDelta.x / 4 + 100, (transform.sizeDelta.y / 2) - i * 32 - 64);

                gameObject.transform.SetParent(inventory.transform, false);
                gameObject.transform.localScale = new Vector3(INVENTORY_SCALE, INVENTORY_SCALE, INVENTORY_SCALE);

                i += 1;
            }
        }
    }
}
