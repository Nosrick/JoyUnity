using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/SendMessage")]
    public class SendMessage : TriggerAction
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private string methodName = string.Empty;
        [SerializeField]
        private ArgumentVariable m_Argument = null;
        [SerializeField]
        private SendMessageOptions m_Options = SendMessageOptions.DontRequireReceiver;

        private GameObject m_TargetÓbject;

        public override void OnStart()
        {
            this.m_TargetÓbject = this.m_Target == TargetType.Player ? Player : gameObject;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Argument.ArgumentType != ArgumentType.None)
            {
                this.m_TargetÓbject.SendMessage(methodName, m_Argument.GetValue(), m_Options);
            }
            else
            {
                this.m_TargetÓbject.SendMessage(methodName, m_Options);
            }

            return ActionStatus.Success;
        }
    }
}