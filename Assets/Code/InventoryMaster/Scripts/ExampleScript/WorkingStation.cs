using UnityEngine;

namespace InventoryMaster.Scripts.ExampleScript
{
    public class WorkingStation : MonoBehaviour
    {

        public KeyCode openInventory;
        public GameObject craftSystem;
        public int distanceToOpenWorkingStation = 3;
        bool showCraftSystem;
        Inventory.Inventory craftInventory;
        CraftSystem.CraftSystem cS;


        // Use this for initialization
        void Start()
        {
            if (craftSystem != null)
            {
                craftInventory = craftSystem.GetComponent<Inventory.Inventory>();
                cS = craftSystem.GetComponent<CraftSystem.CraftSystem>();
            }
        }

        // Update is called once per frame
        void Update()
        {

            float distance = Vector3.Distance(this.gameObject.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

            if (Input.GetKeyDown(openInventory) && distance <= distanceToOpenWorkingStation)
            {
                showCraftSystem = !showCraftSystem;
                if (showCraftSystem)
                {
                    craftInventory.openInventory();
                }
                else
                {
                    cS.backToInventory();
                    craftInventory.closeInventory();
                }
            }
            if (showCraftSystem && distance > distanceToOpenWorkingStation)
            {
                cS.backToInventory();
                craftInventory.closeInventory();
            }


        }
    }
}
