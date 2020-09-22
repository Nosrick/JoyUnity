using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Look At")]
    public class LookAtTrigger : TriggerAction
    {
        [SerializeField]
        private float m_Speed = 15f;

        private Quaternion m_LastRotation;
        private Quaternion m_DesiredRotation;

        public override void OnStart()
        {
            this.m_LastRotation = InventoryManager.current.PlayerInfo.transform.rotation;
            this.m_DesiredRotation = m_LastRotation;
        }

        public override ActionStatus OnUpdate()
        {
            Vector3 targetPosition = transform.position;
            Vector3 gameObjectPosition = InventoryManager.current.PlayerInfo.transform.position;
            targetPosition.y = gameObjectPosition.y;

            Vector3 dir = targetPosition - gameObjectPosition;
            if (dir.sqrMagnitude > 0f)
            {
                m_DesiredRotation = Quaternion.LookRotation(dir);
            }
         
            m_LastRotation = Quaternion.Slerp(m_LastRotation, m_DesiredRotation, this.m_Speed * Time.deltaTime);
            InventoryManager.current.PlayerInfo.transform.rotation = m_LastRotation;
            return Quaternion.Angle(m_LastRotation, m_DesiredRotation) > 5? ActionStatus.Running: ActionStatus.Success;
        }
    }
}