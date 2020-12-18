using UnityEngine;

namespace DevionGames.InventorySystem
{
    [System.Serializable]
    public abstract class ItemModifier : ScriptableObject, IModifier<Item>
    {
        public abstract void Modify(Item item);
     
    }
}