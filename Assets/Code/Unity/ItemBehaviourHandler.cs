﻿using System;
using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Scripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.Unity
{
    public class ItemBehaviourHandler : MonoBehaviourHandler
    {
        public IEntity EntityInRange { get; protected set; }

        public ILiveItemHandler LiveItemHandler { get; set; }

        public override void Awake()
        {
            base.Awake();
            this.Initialise();
        }

        protected override void Initialise()
        {
            if (this.Initialised == false)
            {
                base.Initialise();
                this.LiveItemHandler = GlobalConstants.GameManager.ItemHandler;
            }
        }

        protected override void HandleInput(object data, InputActionChange change)
        {
            if (this.EntityInRange is null)
            {
                return;
            }
        
            if (change != InputActionChange.ActionPerformed)
            {
                return;
            }

            if (!(data is InputAction action))
            {
                return;
            }

            if (action.name.Equals("interact", StringComparison.OrdinalIgnoreCase))
            {
                this.PickupItems();
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            IJoyObject otherObj = other.gameObject.GetComponent<MonoBehaviourHandler>()?.JoyObject;

            if (otherObj is IEntity entity)
            {
                this.EntityInRange = entity;
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            if (this.EntityInRange is null)
            {
                return;
            }
            
            IJoyObject otherObj = other.gameObject.GetComponent<MonoBehaviourHandler>()?.JoyObject;

            if (otherObj is IEntity entity && entity.Guid.Equals(this.EntityInRange.Guid))
            {
                this.EntityInRange = null;
            }
        }
        
        public bool PickupItems()
        {
            this.Initialise();
            bool result = false;
            if (!(this.JoyObject is IItemInstance item))
            {
                return false;
            }
            IJoyAction addItemAction = this.EntityInRange.FetchAction("additemaction");
            result = addItemAction.Execute(
                new IJoyObject[] {this.EntityInRange, item},
                new string[] {"pickup"},
                new Dictionary<string, object>
                {
                    {"newOwner", true}
                });
            result &= this.LiveItemHandler.RemoveItemFromWorld(item.Guid);

            return result;
        }

        public void DropItem()
        {
            this.Initialise();

            if (!(this.JoyObject is IItemInstance itemInstance))
            {
                return;
            }
            IEntity dropper = this.JoyObject.MyWorld.GetEntity(this.JoyObject.WorldPosition);
            this.gameObject.transform.position = new Vector3(this.JoyObject.WorldPosition.x, this.JoyObject.WorldPosition.y);
            IJoyAction placeInWorldAction = dropper.FetchAction("placeiteminworldaction");
            placeInWorldAction.Execute(
                new IJoyObject[] {dropper, itemInstance},
                new string[] { "drop" }, null);
            this.gameObject.SetActive(true);
        }

        public override void AttachJoyObject(IJoyObject joyObject)
        {
            this.Initialise();
            base.AttachJoyObject(joyObject);
        }
    }
}
