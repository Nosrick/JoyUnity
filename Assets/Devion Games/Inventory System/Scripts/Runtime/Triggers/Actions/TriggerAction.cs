using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public abstract class TriggerAction : MonoBehaviour, IAction
    {

        protected GameObject Player {
            get { return InventoryManager.current.PlayerInfo.gameObject; }
        }

        public abstract ActionStatus OnUpdate();

        public virtual void OnStart(){}

        public virtual void OnEnd(){}

        public virtual void OnSequenceStart(){}

        public virtual void OnSequenceEnd(){}

        protected GameObject GetTarget(TargetType type)
        {
            return type == TargetType.Player ? Player : gameObject;
        }
    }

    public enum TargetType { 
        Self,
        Player
    }
}
