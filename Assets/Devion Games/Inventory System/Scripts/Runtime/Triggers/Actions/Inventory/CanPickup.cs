using System.Linq;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [Icon("Condition Item")]
    [ComponentMenu("Inventory System/Can Pickup")]
    public class CanPickup : Action, ICondition
    {
        [SerializeField]
        private string m_WindowName = "Inventory";

        private ItemCollection m_ItemCollection;

        public override void OnStart()
        {
            this.m_ItemCollection = gameObject.GetComponent<ItemCollection>();
        }

        public override ActionStatus OnUpdate()
        {
            return ItemContainer.CanAddItems(this.m_WindowName,this.m_ItemCollection.ToArray())? ActionStatus.Success: ActionStatus.Failure;
        }
    }

}