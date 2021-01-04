using UnityEngine;

namespace JoyLib.Code.Unity
{
    [DisallowMultipleComponent]
    public class GridPosition : MonoBehaviour
    {
        [SerializeField] protected Vector2Int m_WorldPosition;

        public Vector2Int WorldPosition
        {
            get => this.m_WorldPosition;
            set => this.m_WorldPosition = value;
        }
        
        public void Move(Vector2Int position)
        {
            this.WorldPosition = position;
            this.gameObject.transform.position = new Vector3(position.x, position.y);
        }
    }
}