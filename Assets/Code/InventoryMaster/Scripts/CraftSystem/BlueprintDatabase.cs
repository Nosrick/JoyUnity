using System.Collections.Generic;
using UnityEngine;

namespace InventoryMaster.Scripts.CraftSystem
{
    public class BlueprintDatabase : ScriptableObject
    {
        [SerializeField]
        public List<Blueprint> blueprints = new List<Blueprint>();
    }
}
