using DevionGames.InventorySystem;
using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Unity
{
    public class JoyItem : EquipmentItem
    {
        public ItemInstance ItemInstance { get; protected set; }

        public virtual void AttachJoyObject(ItemInstance item)
        {
            ItemInstance = item;
        }
    }
}