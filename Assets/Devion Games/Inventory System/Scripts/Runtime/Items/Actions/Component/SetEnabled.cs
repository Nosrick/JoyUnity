using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon("Component")]
    [ComponentMenu("Component/Set Enabled")]
    [System.Serializable]
    public class SetEnabled : ItemAction
    {
        [Tooltip("The component to set enabled")]
        [SerializeField]
        private string m_ComponentName=string.Empty;
        [SerializeField]
        private bool m_Enable=false;

        private Behaviour m_Component;

        public override void OnStart()
        {
            this.m_Component = Player.GetComponent(this.m_ComponentName) as Behaviour;
            if (this.m_Component == null)
            {
                this.m_Component = Player.GetComponent(this.m_ComponentName) as Behaviour;
            }
        }

        public override ActionStatus OnUpdate()
        {
            if (this.m_Component == null){
                Debug.LogWarning("Missing Component of type " + this.m_ComponentName + "!");
                return ActionStatus.Failure;
            }
            this.m_Component.enabled = this.m_Enable;
            return ActionStatus.Success;
        }
    }
}