using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/CrossFade")]
    public class CrossFade : TriggerAction
    {
        [SerializeField]
        private TargetType m_Target= TargetType.Player;
        [SerializeField]
        private string m_AnimatorState = "Pickup";
        [SerializeField]
        private float m_TransitionDuration = 0.2f;

        private Animator m_Animator;
        private int m_ShortNameHash;

        public override void OnStart()
        {
            this.m_ShortNameHash = Animator.StringToHash(this.m_AnimatorState);
            this.m_Animator = this.m_Target == TargetType.Self ? GetComponent<Animator>() : InventoryManager.current.PlayerInfo.animator; 
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.CrossFadeInFixedTime(this.m_ShortNameHash, this.m_TransitionDuration);
            return ActionStatus.Success;
        }

       /* private bool HasAnimatorState(Animator animator, int hash) {
            if (animator == null) {
                return false;
            }
            for (int j = 0; j < animator.layerCount; j++)
            {
                if (animator.HasState(j, hash))
                {
                    return true;
                }
            }
            return false;
        }*/
    }
}
