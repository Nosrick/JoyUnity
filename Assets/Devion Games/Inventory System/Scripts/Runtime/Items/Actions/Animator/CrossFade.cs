using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon(typeof(Animator))]
    [ComponentMenu("Animator/CrossFade")]
    public class CrossFade : ItemAction
    {
        [SerializeField]
        private string m_AnimatorState = "Pickup";
        [SerializeField]
        private float m_TransitionDuration = 0.2f;

        private Animator m_Animator;
        private int m_ShortNameHash;

        public override void OnStart()
        {
            this.m_ShortNameHash = Animator.StringToHash(this.m_AnimatorState);
            this.m_Animator = Player.GetComponent<Animator>();

            if (!HasAnimatorState(this.m_Animator, this.m_ShortNameHash)) {
                Debug.LogWarning("Animator attached to player does not contain animator state "+this.m_AnimatorState);
            } 
        }

        public override ActionStatus OnUpdate()
        {
            if (m_Animator == null)
            {
                Debug.LogWarning("Missing Component on player of type Animator!");
                return ActionStatus.Failure;
            }
            this.m_Animator.CrossFadeInFixedTime(this.m_ShortNameHash, this.m_TransitionDuration);
            return ActionStatus.Success;
        }

        private bool HasAnimatorState(Animator animator, int hash) {
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
        }
    }
}
