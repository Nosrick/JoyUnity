using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/SetPosition")]
    public class SetPosition : ItemAction
    {
        [SerializeField]
        private Vector3 m_Position=Vector3.zero;

        private Transform m_Transform;


        public override void OnStart()
        {
            this.m_Transform = Player.transform;
        }

        public override ActionStatus OnUpdate()
        {
            this.m_Transform.position = this.m_Position;
            return ActionStatus.Success;
        }
    }
}