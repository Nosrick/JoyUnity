using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/ApplyRootMotion")]
    public class ApplyRootMotion : TriggerAction
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;

        [SerializeField]
        private bool m_Apply = false;

        private Animator m_Animator;

        public override void OnStart()
        {
            this.m_Animator = this.m_Target== TargetType.Self ? GetComponent<Animator>(): InventoryManager.current.PlayerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.applyRootMotion = this.m_Apply;

            return ActionStatus.Success;
        }

    }
}
