using System.Collections.Generic;
using UnityEngine;

namespace InventoryMaster.Scripts.Item
{
    public class ItemAttributeList : ScriptableObject
    {
        [SerializeField]
        public List<ItemAttribute> itemAttributeList = new List<ItemAttribute>();

    }
}
