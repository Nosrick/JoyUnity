using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DevionGames.InventorySystem
{
    [Icon(typeof(Transform))]
    [ComponentMenu("Transform/Move Towards")]
    public class MoveTowards : TriggerAction
    {
        [SerializeField]
        private Vector3 m_Position = Vector3.zero;
        [SerializeField]
        private float m_Speed = 1f;
        [SerializeField]
        private bool m_LookAtPosition = true;
        [SerializeField]
        private bool m_UsePath=false;

        private NavMeshPath m_Path;
        private int m_CurrentPathIndex = 0;

        public override void OnStart()
        {
            if (this.m_UsePath)
            {
                this.m_CurrentPathIndex = 0;
                this.m_Path = new NavMeshPath();
                NavMesh.CalculatePath(InventoryManager.current.PlayerInfo.transform.position, this.m_Position, NavMesh.AllAreas, this.m_Path);
            }
        }

        public override ActionStatus OnUpdate()
        {
            Transform transform = InventoryManager.current.PlayerInfo.transform;

            float step = this.m_Speed * Time.deltaTime;
            if (this.m_Path != null && this.m_Path.corners.Length > this.m_CurrentPathIndex)
            {
                Vector3 nextPosition = this.m_Path.corners[this.m_CurrentPathIndex];
                LookAtPosition(nextPosition);
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, step);
                if (Vector3.Distance(nextPosition, transform.position) < 0.1f && this.m_Path.corners.Length > this.m_CurrentPathIndex) {
                    this.m_CurrentPathIndex++;
           
                }
                for (int i = 0; i < this.m_Path.corners.Length - 1; i++){
                    Debug.DrawLine(this.m_Path.corners[i], this.m_Path.corners[i + 1], Color.red);
                }
                return ActionStatus.Running;
            }

            LookAtPosition(this.m_Position);
            transform.position = Vector3.MoveTowards(transform.position, this.m_Position, step);
            
            return Vector3.Distance(this.m_Position, transform.position) < 0.1f ? ActionStatus.Success : ActionStatus.Running;
        }

        private void LookAtPosition(Vector3 targetPosition) {
            if (!this.m_LookAtPosition) {
                return;
            }
            targetPosition.y = InventoryManager.current.PlayerInfo.transform.position.y;
            Vector3 dir = targetPosition - InventoryManager.current.PlayerInfo.transform.position;
            if (dir.sqrMagnitude > 0f)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(dir);
                Quaternion lastRotation = InventoryManager.current.PlayerInfo.transform.rotation;

                lastRotation = Quaternion.Slerp(lastRotation, desiredRotation, 5f * Time.deltaTime);
                InventoryManager.current.PlayerInfo.transform.rotation = lastRotation;
            }
        }
    }
}