using DevionGames.InventorySystem;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    public class ItemBehaviourHandler : MonoBehaviourHandler
    {
        protected ItemCollection m_Items;
        
        public Entity EntityInRange { get; protected set; }
        
        protected static GameObject WorldObjects { get; set; }

        public void Awake()
        {
            Initialise();
        }

        protected void Initialise()
        {
            if (m_Items is null)
            {
                m_Items = this.GetComponent<ItemCollection>();
            }
            if (LiveItemHandler is null)
            {
                LiveItemHandler = GlobalConstants.GameManager.ItemHandler;
            }
            if (WorldObjects is null)
            {
                WorldObjects = GameObject.Find("WorldObjects");
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            IJoyObject otherObj = other.gameObject.GetComponent<MonoBehaviourHandler>().MyJoyObject;

            if (otherObj is Entity entity)
            {
                EntityInRange = entity;
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (EntityInRange is null)
            {
                return;
            }
            
            IJoyObject otherObj = other.gameObject.GetComponent<MonoBehaviourHandler>().MyJoyObject;

            if (otherObj is Entity entity && entity.GUID.Equals(EntityInRange.GUID))
            {
                EntityInRange = null;
            }
        }

        public override void AttachJoyObject(IJoyObject joyObject)
        {
            Initialise();
            base.AttachJoyObject(joyObject);
            this.transform.parent = WorldObjects.transform;

            if (joyObject is ItemInstance itemInstance)
            {
                m_Items.Add(itemInstance);
            }
        }

        public static ILiveItemHandler LiveItemHandler
        {
            get;
            protected set;
        }
    }
}
