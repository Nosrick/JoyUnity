using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/Set Float")]
    public class SetFloat : TriggerAction
    {
        [SerializeField]
        private TargetType m_Target = TargetType.Player;
        [SerializeField]
        private string m_ParameterName = "Forward Input";
        [SerializeField]
        private float m_Value = 1f;

        private Animator m_Animator;

        public override void OnStart()
        {
            this.m_Animator = this.m_Target == TargetType.Self ? GetComponent<Animator>() : InventoryManager.current.PlayerInfo.animator;
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.SetFloat(this.m_ParameterName, this.m_Value);

            return ActionStatus.Success;
        }

       /* private bool HasParameter(Animator animator, string paramName)
        {
            if (animator == null){
                return false;
            }
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName)
                    return true;
            }
            return false;
        }*/

    }
}
