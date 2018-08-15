using JobLib.Code.Unity;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldDestructionState : GameState
    {
        protected WorldInstance m_OverWorld;
        protected WorldInstance m_NextWorld;

        public WorldDestructionState(WorldInstance overworld, WorldInstance nextWorld)
        {
            m_OverWorld = overworld;
            m_NextWorld = nextWorld;
        }

        public override void Start()
        {
            base.Start();
            DestroyWorld();
        }

        protected void DestroyWorld()
        {
            GameObject worldObjects = GameObject.Find("WorldObjects");
            GameObject worldEntities = GameObject.Find("WorldEntities");
            GameObject worldWalls = GameObject.Find("WorldWalls");

            for(int i = 0; i < worldObjects.transform.childCount; i++)
            {
                GameObject.Destroy(worldObjects.transform.GetChild(i).gameObject);
            }

            for(int i = 0; i < worldEntities.transform.childCount; i++)
            {
                GameObject.Destroy(worldEntities.transform.GetChild(i).gameObject);
            }

            for(int i = 0; i < worldWalls.transform.childCount; i++)
            {
                GameObject.Destroy(worldWalls.transform.GetChild(i).gameObject);
            }

            Done = true;
        }

        public override GameState GetNextState()
        {
            return new WorldInitialisationState(m_OverWorld, m_NextWorld);
        }
    }
}
