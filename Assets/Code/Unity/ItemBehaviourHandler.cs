using DevionGames.InventorySystem;
using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    public class ItemBehaviourHandler : MonoBehaviourHandler
    {
        protected ItemCollection m_Items;

        public void Awake()
        {
            m_Items = this.GetComponent<ItemCollection>();
            if (LiveItemHandler is null)
            {
                LiveItemHandler = GameObject.Find("GameManager").GetComponent<LiveItemHandler>();
            }
        }
    
        public override void AttachJoyObject(JoyObject joyObject)
        {
            base.AttachJoyObject(joyObject);

            if (joyObject is ItemInstance itemInstance)
            {
                m_Items.Add(itemInstance.Item);
            }
        }

        public static LiveItemHandler LiveItemHandler
        {
            get;
            protected set;
        }
    }
}
