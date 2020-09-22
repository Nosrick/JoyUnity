using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Set Trigger")]
    public class SetTrigger : ItemAction
    {
        [SerializeField]
        private string m_ParameterName = string.Empty;

        private Animator m_Animator;

        public override void OnStart()
        {
            this.m_Animator = Player.GetComponent<Animator>();
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component on player of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.SetTrigger(this.m_ParameterName);
            return ActionStatus.Success;
        }
    }
}
