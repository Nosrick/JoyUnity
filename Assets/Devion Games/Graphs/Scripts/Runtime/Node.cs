using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.Graphs
{
    [System.Serializable]
    public abstract class Node
    {
        public string id;
        public string name;
        public Vector2 position;
        [System.NonSerialized]
        public Graph graph;

        public Node()
        {
            id = System.Guid.NewGuid().ToString();
        }

        public virtual void OnAfterDeserialize() { }
        public virtual void OnBeforeSerialize() { }
    }
}