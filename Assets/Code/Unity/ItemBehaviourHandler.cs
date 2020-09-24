using DevionGames.InventorySystem;
using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Unity
{
    public class ItemBehaviourHandler : MonoBehaviourHandler
    {
        protected ItemCollection m_Items;

        public void Awake()
        {
            m_Items = this.GetComponent<ItemCollection>();
        }
    
        public override void AttachJoyObject(JoyObject joyObject)
        {
            base.AttachJoyObject(joyObject);

            if (joyObject is ItemInstance itemInstance)
            {
                m_Items.Add(itemInstance.Item);
            }
        }
    }
}
