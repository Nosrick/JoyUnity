using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldInitialisationState : GameState
    {
        protected WorldInstance m_Overworld;
        protected WorldInstance m_ActiveWorld;

        protected ObjectIconHandler m_ObjectIcons = GameObject.Find("GameManager")
                                                        .GetComponent<ObjectIconHandler>();

        public WorldInitialisationState(WorldInstance overWorld, WorldInstance activeWorld)
        {
            m_Overworld = overWorld;
            m_ActiveWorld = activeWorld;
        }

        public override void Start()
        {
            base.Start();
            InstantiateWorld();
        }

        protected void InstantiateWorld()
        {
            GameObject worldHolder = GameObject.Find("WorldHolder");
            GameObject objectHolder = GameObject.Find("WorldObjects");
            GameObject entityHolder = GameObject.Find("WorldEntities");
            GameObject fogOfWarHolder = GameObject.Find("WorldFog");
            GameObject wallHolder = GameObject.Find("WorldWalls");

            MonoBehaviourHandler prefab = Resources.Load<MonoBehaviourHandler>("Prefabs/MonoBehaviourHandler");
            GameObject sprite = Resources.Load<GameObject>("Prefabs/Sprite");

            //Make the upstairs
            if (m_ActiveWorld.GUID != m_Overworld.GUID)
            {
                GameObject upstairs = GameObject.Instantiate(sprite);
                upstairs.transform.position = new Vector3(m_ActiveWorld.SpawnPoint.x, m_ActiveWorld.SpawnPoint.y, 0.0f);
                upstairs.GetComponent<SpriteRenderer>().sortingLayerName = "Walls";
                upstairs.GetComponent<SpriteRenderer>().sprite = m_ObjectIcons.GetSprite("Stairs", "UpStairs");
                upstairs.transform.parent = objectHolder.transform;
                upstairs.transform.name = m_ActiveWorld.Parent.Name + " stairs";
            }

            //Make each downstairs
            foreach(KeyValuePair<Vector2Int, WorldInstance> pair in m_ActiveWorld.Areas)
            {
                Vector2Int position = pair.Key;
                GameObject downstairs = GameObject.Instantiate(sprite);
                downstairs.transform.position = new Vector3(position.x, position.y, 0.0f);
                downstairs.GetComponent<SpriteRenderer>().sortingLayerName = "Walls";
                downstairs.GetComponent<SpriteRenderer>().sprite = m_ObjectIcons.GetSprite("Stairs", "DownStairs");
                downstairs.transform.parent = objectHolder.transform;
                downstairs.transform.name = pair.Value.Name + " stairs";
            }

            //Make the floors and the fog of war
            for(int i = 0; i < m_ActiveWorld.Tiles.GetLength(0); i++)
            {
                for(int j = 0; j < m_ActiveWorld.Tiles.GetLength(1); j++)
                {
                    //Make the floor
                    GameObject gameObject = GameObject.Instantiate(sprite);
                    gameObject.transform.position = new Vector3(i, j);
                    gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";

                    gameObject.GetComponent<SpriteRenderer>().sprite = 
                        m_ObjectIcons.GetSprite(m_ActiveWorld.Tiles[i, j].TileSet, 
                                                             //TODO: This will eventually be a tile direction selection algorithm
                                                             "solofloor");      
                    gameObject.transform.parent = objectHolder.transform;

                    //Make the fog of war
                    //TODO: MAKE THIS VIABLE
                    GameObject fogOfWar = GameObject.Instantiate(sprite);
                    fogOfWar.transform.position = new Vector3(i, j);
                    fogOfWar.GetComponent<SpriteRenderer>().sortingLayerName = "Fog of War";
                    fogOfWar.GetComponent<SpriteRenderer>().sprite = m_ObjectIcons.GetSprite("Obscure", "Obscure0");
                    fogOfWar.transform.parent = fogOfWarHolder.transform;
                    fogOfWar.name = "Fog of War";
                }
            }

            //Create the objects
            foreach (JoyObject obj in m_ActiveWorld.Objects)
            {
                MonoBehaviourHandler newObject = GameObject.Instantiate(prefab);
                newObject.AttachJoyObject(obj);
                newObject.transform.parent = objectHolder.transform;
            }

            //Create the walls
            foreach(JoyObject wall in m_ActiveWorld.Walls.Values)
            {
                MonoBehaviourHandler newObject = GameObject.Instantiate(prefab);
                newObject.AttachJoyObject(wall);
                newObject.transform.parent = wallHolder.transform;
            }

            //Create the entities
            foreach(Entity entity in m_ActiveWorld.Entities)
            {
                MonoBehaviourHandler newEntity = GameObject.Instantiate(prefab);
                newEntity.AttachJoyObject(entity);
                newEntity.transform.parent = entityHolder.transform;
            }

            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            Entity player = m_ActiveWorld.Player;
            camera.transform.position = new Vector3(player.WorldPosition.x, player.WorldPosition.y, camera.transform.position.z);

            Done = true;
        }

        public override GameState GetNextState()
        {
            return new WorldState(m_Overworld, m_ActiveWorld, Gameplay.GameplayFlags.Moving);
        }
    }
}
