using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;
using System.Linq;

namespace DevionGames.InventorySystem
{
    [Icon("Condition Item")]
    [ComponentMenu("Item Container/Conditions/Can Pickup")]
    public class CanPickup : TriggerAction
    {
        [SerializeField]
        private string m_WindowName = "Inventory";

        private ItemCollection m_ItemCollection;

        public override void OnStart()
        {
            this.m_ItemCollection = GetComponent<ItemCollection>();
        }

        public override ActionStatus OnUpdate()
        {
            return ItemContainer.CanAddItems(this.m_WindowName,this.m_ItemCollection.ToArray())? ActionStatus.Success: ActionStatus.Failure;
        }
    }

}