using System;
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

        public void Awake()
        {
            m_Items = this.GetComponent<ItemCollection>();
            if (LiveItemHandler is null)
            {
                LiveItemHandler = GameObject.Find("GameManager").GetComponent<LiveItemHandler>();
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
            IJoyObject otherObj = other.gameObject.GetComponent<MonoBehaviourHandler>().MyJoyObject;

            if (otherObj is Entity entity && entity.GUID.Equals(EntityInRange.GUID))
            {
                EntityInRange = null;
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
