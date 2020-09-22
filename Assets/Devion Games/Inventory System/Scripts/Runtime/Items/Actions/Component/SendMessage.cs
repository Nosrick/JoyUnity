using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/SendMessage")]
    [System.Serializable]
    public class SendMessage : ItemAction
    {
        [SerializeField]
        private string methodName=string.Empty;
        [SerializeField]
        private ArgumentVariable m_Argument= null;
        [SerializeField]
        private SendMessageOptions m_Options = SendMessageOptions.DontRequireReceiver;

        public override ActionStatus OnUpdate()
        {
            if (m_Argument.ArgumentType != ArgumentType.None)
            {
                Player.SendMessage(methodName, m_Argument.GetValue(), m_Options);
            }else {
                Player.SendMessage(methodName, m_Options);
            }

            return ActionStatus.Success;
        }
    }
}