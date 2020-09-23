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
            MonoBehaviourHandler itemPrefab = Resources.Load<MonoBehaviourHandler>("Prefabs/ItemInstance");
            GameObject sprite = Resources.Load<GameObject>("Prefabs/Sprite");

            //Make the upstairs
            if (m_ActiveWorld.GUID != m_Overworld.GUID)
            {
                GameObject upstairs = GameObject.Instantiate(sprite, objectHolder.transform, true);
                upstairs.transform.position = new Vector3(m_ActiveWorld.SpawnPoint.x, m_ActiveWorld.SpawnPoint.y, 0.0f);
                SpriteRenderer spriteRenderer = upstairs.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Walls";
                spriteRenderer.sprite = m_ObjectIcons.GetSprite("Stairs", "UpStairs");
                upstairs.transform.name = m_ActiveWorld.Parent.Name + " stairs";
            }

            //Make each downstairs
            foreach(KeyValuePair<Vector2Int, WorldInstance> pair in m_ActiveWorld.Areas)
            {
                Vector2Int position = pair.Key;
                GameObject downstairs = GameObject.Instantiate(sprite, objectHolder.transform, true);
                downstairs.transform.position = new Vector3(position.x, position.y, 0.0f);
                SpriteRenderer spriteRenderer = downstairs.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Walls";
                spriteRenderer.sprite = m_ObjectIcons.GetSprite("Stairs", "DownStairs");
                downstairs.transform.name = pair.Value.Name + " stairs";
            }

            //Make the floors and the fog of war
            for(int i = 0; i < m_ActiveWorld.Tiles.GetLength(0); i++)
            {
                for(int j = 0; j < m_ActiveWorld.Tiles.GetLength(1); j++)
                {
                    //Make the floor
                    GameObject gameObject = GameObject.Instantiate(sprite, objectHolder.transform, true);
                    gameObject.transform.position = new Vector3(i, j);
                    SpriteRenderer goSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    goSpriteRenderer.sortingLayerName = "Terrain";
                    goSpriteRenderer.sprite = 
                        m_ObjectIcons.GetSprite(m_ActiveWorld.Tiles[i, j].TileSet, 
                                                             //TODO: This will eventually be a tile direction selection algorithm
                                                             "surroundfloor");

                    //Make the fog of war
                    //TODO: MAKE THIS VIABLE
                    GameObject fogOfWar = GameObject.Instantiate(sprite, fogOfWarHolder.transform, true);
                    fogOfWar.transform.position = new Vector3(i, j);
                    SpriteRenderer fogSpriteRenderer = fogOfWar.GetComponent<SpriteRenderer>();
                    fogSpriteRenderer.sortingLayerName = "Fog of War";
                    fogSpriteRenderer.sprite = m_ObjectIcons.GetSprite("Obscure", "Obscure");
                    fogOfWar.name = "Fog of War";
                }
            }

            //Create the objects
            foreach (JoyObject obj in m_ActiveWorld.Objects)
            {
                MonoBehaviourHandler newObject = GameObject.Instantiate(itemPrefab, objectHolder.transform, true);
                newObject.AttachJoyObject(obj);
            }

            //Create the walls
            foreach(JoyObject wall in m_ActiveWorld.Walls.Values)
            {
                MonoBehaviourHandler newObject = GameObject.Instantiate(prefab, wallHolder.transform, true);
                newObject.AttachJoyObject(wall);
            }

            //Create the entities
            foreach(Entity entity in m_ActiveWorld.Entities)
            {
                MonoBehaviourHandler newEntity = GameObject.Instantiate(prefab, entityHolder.transform, true);
                newEntity.AttachJoyObject(entity);

                if (newEntity.MyJoyObject is Entity tempEntity)
                {
                    if (tempEntity.PlayerControlled)
                    {
                        newEntity.tag = "Player";
                    }
                }
            }

            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            Entity player = m_ActiveWorld.Player;
            Transform transform = camera.transform;
            transform.position = new Vector3(player.WorldPosition.x, player.WorldPosition.y, transform.position.z);

            Done = true;
        }

        public override GameState GetNextState()
        {
            return new WorldState(m_Overworld, m_ActiveWorld, Gameplay.GameplayFlags.Moving);
        }
    }
}
