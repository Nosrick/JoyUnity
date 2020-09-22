using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon("Condition Item")]
    [ComponentMenu("Item Container/Conditions/Has Item")]
    public class HasItem : TriggerAction
    {
        [ItemPicker(true)]
        [SerializeField]
        protected List<ItemCondition> requiredItems = new List<ItemCondition>();

        public override ActionStatus OnUpdate()
        {
            for (int i = 0; i < requiredItems.Count; i++)
            {
                ItemCondition condition = requiredItems[i];
                if (condition.item != null && !string.IsNullOrEmpty(condition.stringValue)) { 

                    if (!ItemContainer.HasItem(condition.stringValue,condition.item, 1))
                    {
                        if (InventoryManager.UI.notification != null)
                        {
                            InventoryManager.UI.notification.AddItem(InventoryManager.Notifications.missingItem,condition.item.Name,condition.stringValue);
                        }
                        return ActionStatus.Failure;
                    }
                }
            }

            return ActionStatus.Success;
        }
    }
}