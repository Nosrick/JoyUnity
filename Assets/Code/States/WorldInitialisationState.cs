﻿using JobLib.Code.Unity;
using JoyLib.Code.Entities;
using JoyLib.Code.Graphics;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldInitialisationState : GameState
    {
        protected WorldInstance m_Overworld;
        protected WorldInstance m_ActiveWorld;

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
                upstairs.GetComponent<SpriteRenderer>().sprite = ObjectIcons.GetSprite("Stairs", "UpStairs0");
                upstairs.transform.parent = objectHolder.transform;
            }

            //Make each downstairs
            foreach(Vector2Int position in m_ActiveWorld.Areas.Keys)
            {
                GameObject downstairs = GameObject.Instantiate(sprite);
                downstairs.transform.position = new Vector3(position.x, position.y, 0.0f);
                downstairs.GetComponent<SpriteRenderer>().sortingLayerName = "Walls";
                downstairs.GetComponent<SpriteRenderer>().sprite = ObjectIcons.GetSprite("Stairs", "DownStairs0");
                downstairs.transform.parent = objectHolder.transform;
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
                    switch(m_ActiveWorld.Tiles[i, j])
                    {
                        case WorldTile.Paving:
                            gameObject.GetComponent<SpriteRenderer>().sprite = ObjectIcons.GetSprite("Paving", "MiddleMiddle0");
                            break;

                        case WorldTile.Plains:
                            gameObject.GetComponent<SpriteRenderer>().sprite = ObjectIcons.GetSprite("Plains", "MiddleMiddle0");
                            break;
                    }
                    
                    gameObject.transform.parent = objectHolder.transform;

                    //Make the fog of war
                    //TODO: MAKE THIS VIABLE
                    GameObject fogOfWar = GameObject.Instantiate(sprite);
                    fogOfWar.transform.position = new Vector3(i, j);
                    fogOfWar.GetComponent<SpriteRenderer>().sortingLayerName = "Fog of War";
                    fogOfWar.GetComponent<SpriteRenderer>().sprite = ObjectIcons.GetSprite("Obscure", "Obscure0");
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
