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

        public void Start()
        {
            m_SlotPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventorySlot");
            m_ListPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventoryListItem");
            m_ItemPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventoryItem");

            if (m_Player != null)
            {
                DoSlots();
            }
        }

        public void SetPlayer(Entity player)
        {
            m_Player = player;
        }

        public void DoSlots()
        {
            int i = 0;
            foreach (string slot in m_Player.Slots)
            {
                GameObject gameObject = Instantiate(m_SlotPrefab);
                JoyInventoryCell cell = gameObject.GetComponent<JoyInventoryCell>();

                TextMeshProUGUI textMesh = cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                textMesh.text = slot + ":";

                JoyInventoryItem item = Instantiate(m_ItemPrefab).GetComponent<JoyInventoryItem>();
                if(m_Player.GetEquipment(slot) != null)
                {
                    item.Item = m_Player.GetEquipment(slot);

                    cell.SetItem(item);
                }

                item.transform.SetParent(cell.transform, false);
                cell.transform.SetParent(gameObject.transform, false);

                RectTransform transform = this.GetComponent<RectTransform>();
                gameObject.transform.position = new Vector3(-transform.sizeDelta.x / 4, i * 16);
            
                gameObject.transform.SetParent(this.transform, false);
                gameObject.transform.localScale = new Vector3(1, 1, 1);

                i += 1;
            }
        }
    }
}
