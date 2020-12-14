using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldInitialisationState : GameState
    {
        protected IWorldInstance m_Overworld;
        protected IWorldInstance m_ActiveWorld;
        
        protected GameObject WorldHolder { get; set; }
        protected GameObject ObjectHolder { get; set; }
        protected GameObject EntityHolder { get; set; }
        protected GameObject FogHolder { get; set; }
        protected GameObject WallHolder { get; set; }
        protected GameObject FloorHolder { get; set; }
        
        protected MonoBehaviourHandler Prefab { get; set; }
        protected MonoBehaviourHandler ItemPrefab { get; set; }
        protected GameObject Sprite { get; set; }

        protected IObjectIconHandler m_ObjectIcons = GlobalConstants.GameManager.ObjectIconHandler;

        public WorldInitialisationState(IWorldInstance overWorld, IWorldInstance activeWorld)
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
            IGameManager gameManager = GlobalConstants.GameManager;
            
            //Make the upstairs
            if (m_ActiveWorld.GUID != m_Overworld.GUID)
            {
                GameObject child = gameManager.ItemPool.Get();

                child.transform.position = new Vector3(m_ActiveWorld.SpawnPoint.x, m_ActiveWorld.SpawnPoint.y, 0.0f);
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Walls";
                spriteRenderer.sprite = m_ObjectIcons.GetSprite("Stairs", "UpStairs");
                child.transform.name = m_ActiveWorld.Parent.Name + " stairs";
            }

            //Make each downstairs
            foreach(KeyValuePair<Vector2Int, IWorldInstance> pair in m_ActiveWorld.Areas)
            {
                GameObject child = gameManager.ItemPool.Get();
                
                Vector2Int position = pair.Key;
                child.transform.position = new Vector3(position.x, position.y, 0.0f);
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Walls";
                spriteRenderer.sprite = m_ObjectIcons.GetSprite("Stairs", "Downstairs");
                child.transform.name = m_ActiveWorld.Parent.Name + " stairs";
            }

            int index = 0;
            Vector3 pos = Vector3.zero;
            for(int i = 0; i < m_ActiveWorld.Tiles.GetLength(0); i++)
            {
                for(int j = 0; j < m_ActiveWorld.Tiles.GetLength(1); j++)
                {
                    pos.x = i;
                    pos.y = j;
                    
                    //Make the fog of war
                    GameObject fog = gameManager.FogPool.Get();
                    fog.transform.position = pos;
                    SpriteRenderer goSpriteRenderer = fog.GetComponent<SpriteRenderer>();
                    goSpriteRenderer.sortingLayerName = "Fog of War";
                    goSpriteRenderer.sprite = m_ObjectIcons.GetSprite("Obscure", "Obscure");
                    fog.name = "Fog of War";
                    fog.SetActive(true);
                    
                    //Make the floor
                    GameObject floor = gameManager.FloorPool.Get();
                    floor.transform.position = pos;
                    SpriteRenderer fogSpriteRenderer = floor.GetComponent<SpriteRenderer>();
                    fogSpriteRenderer.sortingLayerName = "Terrain";
                    fogSpriteRenderer.sprite = 
                        m_ObjectIcons.GetSprite(m_ActiveWorld.Tiles[i, j].TileSet, 
                            //TODO: This will eventually be a tile direction selection algorithm
                            "surroundfloor");
                    floor.name = this.m_ActiveWorld.Name + " floor";
                    fog.SetActive(true);

                    index += 1;
                }
            }

            //Create the walls
            foreach(IJoyObject wall in m_ActiveWorld.Walls.Values)
            {
                GameObject gameObject = gameManager.WallPool.Get();
                gameObject.GetComponent<MonoBehaviourHandler>()
                    .AttachJoyObject(wall);
                gameObject.SetActive(true);
                index += 1;
            }
            
            this.CreateItems(this.m_ActiveWorld.Objects);
            
            //Create the entities
            foreach(Entity entity in m_ActiveWorld.Entities)
            {
                GameObject gameObject = gameManager.EntityPool.Get();
                gameObject.SetActive(true);
                MonoBehaviourHandler newEntity = gameObject.GetComponent<MonoBehaviourHandler>();
                newEntity.AttachJoyObject(entity);

                if (entity.PlayerControlled)
                {
                    newEntity.tag = "Player";
                }
                this.CreateItems(entity.Backpack);
            }

            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            IEntity player = m_ActiveWorld.Player;
            Transform transform = camera.transform;
            transform.position = new Vector3(player.WorldPosition.x, player.WorldPosition.y, transform.position.z);

            Done = true;
        }

        protected void CreateItems(IEnumerable<IJoyObject> items)
        {
            IGameManager gameManager = GlobalConstants.GameManager;
            foreach (IItemInstance item in items)
            {
                if (item is ItemInstance itemInstance)
                {
                    itemInstance.Instantiate(gameManager.ItemPool.Get());
                }
            }
        }

        public override GameState GetNextState()
        {
            return new WorldState(m_Overworld, m_ActiveWorld, Gameplay.GameplayFlags.Moving);
        }
    }
}
