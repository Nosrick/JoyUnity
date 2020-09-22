using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon(typeof(GameObject))]
    [ComponentMenu("GameObject/Instantiate")]
    public class Instantiate : TriggerAction
    {
        [SerializeField]
        private TargetType m_Target= TargetType.Self;
        [SerializeField]
        private GameObject m_Original = null;
        [SerializeField]
        private Vector3 m_PositionOffset = Vector3.up;
        public override ActionStatus OnUpdate()
        {
            GameObject target = GetTarget(this.m_Target);
            Instantiate(this.m_Original, target.transform.position+this.m_PositionOffset, Quaternion.identity);
            return ActionStatus.Success;
        }
    }
}