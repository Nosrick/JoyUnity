using DevionGames.UIWidgets;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Unity;

namespace JoyLib.Code.Entities
{
    public class EntityPlayer : Entity
    {
        protected static MutableItemContainer InventoryContainer { get; set; }
        protected static MutableItemContainer EquipmentContainer { get; set; }

        public EntityPlayer(IEntity baseEntity) :
            base(baseEntity)
        {
            if (InventoryContainer is null)
            {
                InventoryContainer = WidgetUtility.Find<MutableItemContainer>("Inventory");
                InventoryContainer.Owner = this;
                EquipmentContainer = WidgetUtility.Find<MutableItemContainer>("Equipment");
                EquipmentContainer.Owner = this;
            }

            PlayerControlled = true;
            m_Driver = new PlayerDriver();
        }
        
        /*
        public override bool EquipItem(string slotRef, IItemInstance itemRef)
        {
            return base.EquipItem(slotRef, itemRef);
        }

        public override bool AddContents(IItemInstance actor)
        {
            if (actor is ItemInstance item)
            {
                bool result = true;
                //result &= Inventory.StackOrAdd(item);
                return result && base.AddContents(actor);
            }

            return false;
        }

        public override bool AddContents(IEnumerable<IItemInstance> actors)
        {
            bool result = true;
            foreach (ItemInstance item in actors)
            {
                //result &= Inventory.StackOrAdd(item);
            }
            
            return result && base.AddContents(actors);
        }

        public override bool RemoveContents(IItemInstance item)
        {
            if (item is ItemInstance instance)
            {
                bool result = true;
                result &= Inventory.RemoveItem(instance);
                return result && base.RemoveContents(item);
            }

            return false;
        }
        */

        public override void Clear()
        {
            InventoryContainer.RemoveItems();
            base.Clear();
        }
    }
}