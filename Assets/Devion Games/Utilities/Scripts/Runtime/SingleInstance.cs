using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevionGames
{
    public class SingleInstance : MonoBehaviour
    {
        private static Dictionary<string, GameObject> m_Instances = new Dictionary<string, GameObject>();

        void Awake()
        {
            GameObject instance = null;
            SingleInstance.m_Instances.TryGetValue(this.name, out instance);
            if (instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                SingleInstance.m_Instances[this.name] = gameObject;
            }
            else
            {
                //Debug.Log("Multiple "+gameObject.name+" in scene. Destroying instance!");
                DestroyImmediate(gameObject);
            }
        }

        public static List<GameObject> GetInstanceObjects() {
            return m_Instances.Values.Where(x=>x != null).ToList();
        }
    }
}