using System.Collections.Generic;
using UnityEngine;

namespace Code.Collections
{
    public class GameObjectPool
    {
        protected List<GameObject> Objects { get; set; }
        protected List<GameObject> InactiveObjects { get; set; }
        protected GameObject Prefab { get; set; }
        protected GameObject Parent { get; set; }

        public GameObjectPool(GameObject prefab, GameObject parent)
        {
            this.Objects = new List<GameObject>();
            this.InactiveObjects = new List<GameObject>();
            this.Prefab = prefab;
            this.Parent = parent;
        }

        public GameObject Get()
        {
            if (this.InactiveObjects.Count > 0)
            {
                GameObject returnObject = this.InactiveObjects[0];
                this.InactiveObjects.RemoveAt(0);
                this.Objects.Add(returnObject);
                return returnObject;
            }

            GameObject newObject = Object.Instantiate(this.Prefab, this.Parent.transform);
            this.Objects.Add(newObject);
            return newObject;
        }

        public bool Retire(GameObject gameObject)
        {
            bool result = this.Objects.Remove(gameObject);
            if (result)
            {
                this.InactiveObjects.Add(gameObject);
            }

            return result;
        }

        public void RetireAll()
        {
            foreach (GameObject gameObject in this.Objects)
            {
                gameObject.SetActive(false);
            }
            this.InactiveObjects.AddRange(this.Objects);
            this.Objects.Clear();
        }
    }
}