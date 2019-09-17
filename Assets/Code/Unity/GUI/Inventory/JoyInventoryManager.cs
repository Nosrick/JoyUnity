using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Inventory
{
    public class JoyInventoryManager : MonoBehaviour
    {
        //The Entity whose inventory we are managing
        private Entity m_Player;

        //The prefabs we instantiate whenever we create this screen
        private GameObject m_SlotPrefab;
        private GameObject m_ListPrefab;
        private GameObject m_ItemPrefab;
        private GameObject m_ButtonPrefab;

        //The quad representing the background
        private GameObject m_Background;

        //The GameObject representing the cursor
        private GameObject m_Cursor;

        //The GameObject for the item description object
        private TextMeshProUGUI m_ItemDescription;

        //The GameObject representing the inventory storage object
        private GameObject m_Inventory;

        //The Canvas object for the scene
        private Canvas m_Canvas;

        //Indexy stuff for selecting
        private int m_Index = 0;
        private bool m_OnButtons = false;

        private const int SLOT_SCALE = 3;
        private const int INVENTORY_SCALE = 5;
        private const int QUAD_SCALE = 25;

        private const int NO_ITEM = -1;

        public void Start()
        {
            m_SlotPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventorySlot");
            m_ListPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventoryListItem");
            m_ItemPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventoryItem");
            m_ButtonPrefab = Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventoryButton");

            m_Background = GameObject.Find("InventoryBackgroundQuad");
            m_Cursor = GameObject.Find("InventoryCursor");
            m_Inventory = GameObject.Find("Inventory");
            m_Canvas = GameObject.Find("GUICanvas").GetComponent<Canvas>();
            m_ItemDescription = GameObject.Find("ItemDescription").GetComponent<TextMeshProUGUI>();

            float quadHeight = Camera.main.orthographicSize * 2.0f;
            float quadWidth = quadHeight * Screen.width / Screen.height;

            m_Background.transform.localScale = new Vector3(quadWidth * QUAD_SCALE, quadHeight * QUAD_SCALE, 1);
        }

        public void OnEnable()
        {
            if (m_Player != null)
            {
                DoAll();
            }
        }

        public void Update()
        {
            bool cursorMoved = false;
            if(Input.GetKeyDown(KeyCode.Keypad2))
            {
                m_Index += 1;
                m_Index %= m_Player.Backpack.Length;

                cursorMoved = true;
            }
            else if(Input.GetKeyDown(KeyCode.Keypad8))
            {
                m_Index -= 1;
                if(m_Index == -1)
                {
                    m_Index = m_Player.Backpack.Length - 1;
                }

                cursorMoved = true;
            }

            if(cursorMoved == true)
            {
                SetCursorIndex(m_Index);
                SetItemDescription(m_Index);
            }

            if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) && m_Cursor.activeInHierarchy)
            {
                if (m_OnButtons == false)
                {
                    CreateButtons();
                }
            }
        }

        private void CreateButtons()
        {

        }

        private void SetCursorIndex(int index)
        {
            RectTransform highlighted = m_Inventory.transform.GetChild(index).GetComponent<RectTransform>();

            float canvasScale = m_Canvas.scaleFactor;

            m_Cursor.transform.position = new Vector3(highlighted.transform.position.x + (highlighted.rect.xMin * highlighted.localToWorldMatrix.m00 * canvasScale), highlighted.transform.position.y, highlighted.transform.position.z);
        }

        private void SetItemDescription(int index)
        {
            if(index == NO_ITEM)
            {
                m_ItemDescription.text = "";
                return;
            }

            ItemInstance item = m_Inventory.transform.GetChild(index).GetChild(0).GetComponent<JoyInventoryListItem>().Item;

            m_ItemDescription.text = item.DisplayName + "\n\r" + item.DisplayDescription + "\n\r" + item.SlotString + "\n\r" + item.WeightString + "\n\r" + item.ItemType.MaterialDescription + "\n\r" + item.ContentString + "\n\r" + item.ConditionString;
        }

        public void SetPlayer(Entity player)
        {
            m_Player = player;
        }

        protected void DoAll()
        {
            DoSlots();
            DoInventory();
        }

        public void DoSlots()
        {
            int i = 0;
            GameObject slots = GameObject.Find("Slots");
            for(int j = 0; j < slots.transform.childCount; j++)
            {
                GameObject.Destroy(slots.transform.GetChild(j).gameObject);
            }

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
                gameObject.transform.position = new Vector3(-transform.sizeDelta.x / 3 - 100, (transform.sizeDelta.y / 2) - i * 24 - 48);
            
                gameObject.transform.SetParent(slots.transform, false);
                gameObject.transform.localScale = new Vector3(SLOT_SCALE, SLOT_SCALE, SLOT_SCALE);

                i += 1;
            }

            Debug.Log("Finished setting up slots.");
        }

        public void DoInventory()
        {
            for(int j = 0; j < m_Inventory.transform.childCount; j++)
            {
                GameObject.Destroy(m_Inventory.transform.GetChild(j).gameObject);
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
                gameObject.transform.position = new Vector3(-transform.sizeDelta.x / 4 + 132, (transform.sizeDelta.y / 2) - i * 32 - 64);

                gameObject.transform.SetParent(m_Inventory.transform, false);
                gameObject.transform.localScale = new Vector3(INVENTORY_SCALE, INVENTORY_SCALE, INVENTORY_SCALE);

                i += 1;
            }

            if(m_Inventory.transform.childCount > 0)
            {
                m_Cursor.SetActive(true);
                SetCursorIndex(0);
                SetItemDescription(0);
            }
            else
            {
                m_Cursor.SetActive(false);
                SetItemDescription(NO_ITEM);
            }

            Debug.Log("Finished setting up inventory.");
        }
    }
}
