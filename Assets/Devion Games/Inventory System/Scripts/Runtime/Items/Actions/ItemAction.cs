using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [System.Serializable]
    public abstract class ItemAction : ScriptableObject, IAction
    {
        
        [HideInInspector]
        public bool enabled=true;
        [HideInInspector]
        public Item item;

        public GameObject Player {
            get { return InventoryManager.current.PlayerInfo.gameObject; }
        }

        public bool isActiveAndEnabled
        {
            get
            {
                return enabled;
            }
        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
        }

        public abstract ActionStatus OnUpdate();

        public virtual void OnStart() { }

        public virtual void OnEnd() { }

        public virtual void OnSequenceStart() { }

        public virtual void OnSequenceEnd() { }

    }
}