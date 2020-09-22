using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem.ItemActions;
using System.Linq;

namespace DevionGames.InventorySystem
{
	[System.Serializable]
	public class UsableItem : Item
	{
        [SerializeField]
        private bool m_UseCategoryCooldown = true;
        [SerializeField]
        private float m_Cooldown = 1f;
        public float Cooldown {
            get {
                return this.m_UseCategoryCooldown ? Category.Cooldown : this.m_Cooldown;
            }
        }

        [SerializeField]
        public List<ItemAction> actions = new List<ItemAction>();

        private Sequence m_ActionSequence;
        private IEnumerator m_ActionBehavior;

        protected override void OnEnable()
        {
            base.OnEnable();
            actions.RemoveAll(x => x == null);
            for (int i = 0; i < actions.Count; i++) {
                ItemAction action = actions[i];
                action.item = this;
            }
            this.m_ActionSequence = new Sequence(actions.Cast<IAction>().ToArray());
        }

        public override void Use()
        {
            if (this.m_ActionBehavior != null) {
                CoroutineUtility.StopCoroutine(m_ActionBehavior);
            }
            this.m_ActionBehavior = SequenceCoroutine();
            CoroutineUtility.StartCoroutine(this.m_ActionBehavior);
        }

        private IEnumerator SequenceCoroutine() {
            this.m_ActionSequence.Start();
            while (this.m_ActionSequence.Tick()) {
                yield return null;
            }
        }
    }
}