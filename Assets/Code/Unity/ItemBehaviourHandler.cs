using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Unity
{
    public class ItemBehaviourHandler : MonoBehaviourHandler
    {
        public IEntity EntityInRange { get; protected set; }

        public static ILiveItemHandler LiveItemHandler { get; set; }
        public static ILiveEntityHandler LiveEntityHandler { get; set; }

        public void Awake()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (LiveItemHandler is null)
            {
                LiveItemHandler = GlobalConstants.GameManager.ItemHandler;
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            IJoyObject otherObj = other.gameObject.GetComponent<MonoBehaviourHandler>().MyJoyObject;

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
            
            IJoyObject otherObj = other.gameObject.GetComponent<MonoBehaviourHandler>().MyJoyObject;

            if (otherObj is IEntity entity && entity.GUID.Equals(this.EntityInRange.GUID))
            {
                this.EntityInRange = null;
            }
        }
        
        public bool PickupItems()
        {
            this.Initialise();
            bool result = false;
            if (this.MyJoyObject is IItemInstance item)
            {
                IJoyAction addItemAction = this.EntityInRange.FetchAction("additemaction");
                result = addItemAction.Execute(
                    new IJoyObject[] {this.EntityInRange, item},
                    new string[] {"pickup"},
                    new object[] {true});
                result &= LiveItemHandler.RemoveItemFromWorld(item.GUID);
            }

            return result;
        }

        public void DropItem(IItemInstance item)
        {
            this.Initialise();
            
            if (this.MyJoyObject is IItemInstance itemInstance)
            {
                IEntity dropper = this.MyJoyObject.MyWorld.GetEntity(this.MyJoyObject.WorldPosition);
                this.gameObject.transform.position = new Vector3(this.MyJoyObject.WorldPosition.x, this.MyJoyObject.WorldPosition.y);
                IJoyAction placeInWorldAction = dropper.FetchAction("placeiteminworldaction");
                placeInWorldAction.Execute(
                    new IJoyObject[] {dropper, itemInstance},
                    new string[] { "drop" });
                this.gameObject.SetActive(true);
            }
        }

        public override void AttachJoyObject(IJoyObject joyObject)
        {
            this.Initialise();
            base.AttachJoyObject(joyObject);
        }
    }
}
