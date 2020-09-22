using UnityEngine;

namespace InventoryMaster.Scripts.Item
{
    public class SlotCover : MonoBehaviour
    {

        Inventory.Inventory inv;
        RectTransform rT;

        // Use this for initialization
        void Start()
        {
            inv = transform.parent.parent.parent.parent.GetComponent<Inventory.Inventory>();
            rT = GetComponent<RectTransform>();

        }

        // Update is called once per frame
        void Update()
        {

            rT.sizeDelta = new Vector3(inv.slotSize, inv.slotSize, 0);
        }
    }
}
