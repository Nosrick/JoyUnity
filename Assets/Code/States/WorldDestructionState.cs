using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldDestructionState : GameState
    {
        protected IWorldInstance m_OverWorld;
        protected IWorldInstance m_NextWorld;

        public WorldDestructionState(IWorldInstance overworld, IWorldInstance nextWorld)
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
            GameObject worldFog = GameObject.Find("WorldFog");
            GameObject worldFloors = GameObject.Find("WorldFloors");

            for(int i = 0; i < worldObjects.transform.childCount; i++)
            {
                worldObjects.transform.GetChild(i).gameObject.SetActive(false);
            }

            for(int i = 0; i < worldEntities.transform.childCount; i++)
            {
                worldEntities.transform.GetChild(i).gameObject.SetActive(false);
            }

            for(int i = 0; i < worldWalls.transform.childCount; i++)
            {
                worldWalls.transform.GetChild(i).gameObject.SetActive(false);
            }

            for(int i = 0; i < worldFog.transform.childCount; i++)
            {
                worldFog.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < worldFloors.transform.childCount; i++)
            {
                worldFloors.transform.GetChild(i).gameObject.SetActive(false);
            }

            Done = true;
        }

        public override GameState GetNextState()
        {
            return new WorldInitialisationState(m_OverWorld, m_NextWorld);
        }
    }
}
