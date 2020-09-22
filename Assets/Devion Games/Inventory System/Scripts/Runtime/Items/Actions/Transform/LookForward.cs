using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem.ItemActions
{
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/LookForward")]
    public class LookForward : ItemAction
    {
        private Transform m_Transform;
        private Transform m_CameraTransform;

        public override void OnStart()
        {
            this.m_CameraTransform = Camera.main.transform;
            this.m_Transform = Player.transform;
        }

        public override ActionStatus OnUpdate()
        {
            Quaternion lookRotation = Quaternion.Euler(this.m_Transform.eulerAngles.x, this.m_CameraTransform.eulerAngles.y, this.m_Transform.eulerAngles.z);
            this.m_Transform.rotation = lookRotation;
            return ActionStatus.Success;
        }
    }
}