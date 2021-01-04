using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity;
using JoyLib.Code.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.States
{
    public class WorldInitialisationState : GameState
    {
        protected IWorldInstance m_Overworld;
        protected IWorldInstance m_ActiveWorld;

        protected IObjectIconHandler m_ObjectIcons = GlobalConstants.GameManager.ObjectIconHandler;

        public WorldInitialisationState(IWorldInstance overWorld, IWorldInstance activeWorld)
        {
            this.m_Overworld = overWorld;
            this.m_ActiveWorld = activeWorld;
        }

        public override void LoadContent()
        {
        }

        public override void Start()
        {
            this.InstantiateWorld();
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
        }

        public override void HandleInput(object data, InputActionChange action)
        {
        }

        protected void InstantiateWorld()
        {
            IGameManager gameManager = GlobalConstants.GameManager;
            List<IBasicValue<float>> values = new List<IBasicValue<float>>
            {
                new ConcreteBasicFloatValue("weight", 1),
                new ConcreteBasicFloatValue("bonus", 1),
                new ConcreteBasicFloatValue("size", 1),
                new ConcreteBasicFloatValue("hardness", 1),
                new ConcreteBasicFloatValue("density", 1)
            };
            
            int terrainLayer = LayerMask.NameToLayer("Terrain");
            //Make the upstairs
            if (this.m_ActiveWorld.GUID != this.m_Overworld.GUID)
            {
                GameObject child = gameManager.FloorPool.Get();
                child.layer = terrainLayer;
                child.transform.position = new Vector3(this.m_ActiveWorld.SpawnPoint.x, this.m_ActiveWorld.SpawnPoint.y, 0.0f);
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = this.m_ObjectIcons.GetSprite("Stairs", "Upstairs");
                spriteRenderer.sortingLayerName = "Walls";
                child.name = this.m_ActiveWorld.Parent.Name + " stairs";
                TooltipComponent tooltip = child.GetComponent<TooltipComponent>();
                tooltip.WorldPosition = this.m_ActiveWorld.SpawnPoint;
                tooltip.RefreshTooltip = WorldState.GetTooltipData;
                child.SetActive(true);
            }

            //Make each downstairs
            foreach(KeyValuePair<Vector2Int, IWorldInstance> pair in this.m_ActiveWorld.Areas)
            {
                GameObject child = gameManager.FloorPool.Get();
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                child.layer = terrainLayer;
                spriteRenderer.sortingLayerName = "Walls";
                spriteRenderer.sprite = this.m_ObjectIcons.GetSprite("Stairs", "Downstairs");
                child.name = pair.Value.Name + " stairs";
                child.transform.position = new Vector3(pair.Key.x, pair.Key.y);
                TooltipComponent tooltip = child.GetComponent<TooltipComponent>();
                tooltip.WorldPosition = pair.Key;
                tooltip.RefreshTooltip = WorldState.GetTooltipData;
                child.SetActive(true);
            }

            int fogLayer = LayerMask.NameToLayer("Fog of War");
            Vector3 pos = Vector3.zero;
            for(int i = 0; i < this.m_ActiveWorld.Tiles.GetLength(0); i++)
            {
                for(int j = 0; j < this.m_ActiveWorld.Tiles.GetLength(1); j++)
                {
                    pos.x = i;
                    pos.y = j;

                    Vector2Int intPos = new Vector2Int(i, j);
                    
                    //Make the fog of war
                    GameObject fog = gameManager.FogPool.Get();
                    fog.layer = fogLayer;
                    fog.transform.position = pos;
                    SpriteRenderer goSpriteRenderer = fog.GetComponent<SpriteRenderer>();
                    goSpriteRenderer.sortingLayerName = "Fog of War";
                    goSpriteRenderer.sprite = this.m_ObjectIcons.GetSprite("Obscure", "Obscure");
                    fog.name = "Fog of War";
                    fog.SetActive(true);
                    
                    
                    //Make the floor
                    GameObject floor = gameManager.FloorPool.Get();
                    floor.layer = terrainLayer;
                    goSpriteRenderer = floor.GetComponent<SpriteRenderer>();
                    goSpriteRenderer.sortingLayerName = "Terrain";
                    goSpriteRenderer.sprite = this.m_ObjectIcons.GetSprite(this.m_ActiveWorld.Tiles[i, j].TileSet, 
                            //TODO: This will eventually be a tile direction selection algorithm
                            "surroundfloor");
                    floor.name = this.m_ActiveWorld.Name + " floor";
                    TooltipComponent tooltip = floor.GetComponent<TooltipComponent>();
                    tooltip.Move(intPos);
                    tooltip.RefreshTooltip = WorldState.GetTooltipData;
                    floor.SetActive(true);
                }
            }
            
            int wallLayer = LayerMask.NameToLayer("Walls");
            //Create the walls
            foreach(IJoyObject wall in this.m_ActiveWorld.Walls.Values)
            {
                wall.MyWorld = this.m_ActiveWorld;
                GameObject gameObject = gameManager.WallPool.Get();
                gameObject.layer = wallLayer;
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Walls";
                gameObject.GetComponent<MonoBehaviourHandler>()
                    .AttachJoyObject(wall);
                gameObject.SetActive(true);
            }
            
            this.CreateItems(this.m_ActiveWorld.Objects);

            int entityLayer = LayerMask.NameToLayer("Entities");
            //Create the entities
            foreach(IEntity entity in this.m_ActiveWorld.Entities)
            {
                GameObject gameObject = gameManager.EntityPool.Get();
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
                gameObject.SetActive(true);
                gameObject.layer = entityLayer;
                MonoBehaviourHandler newEntity = gameObject.GetComponent<MonoBehaviourHandler>();
                newEntity.AttachJoyObject(entity);

                if (entity.PlayerControlled)
                {
                    newEntity.tag = "Player";
                }
                this.CreateItems(entity.Backpack, false);
            }

            Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            IEntity player = this.m_ActiveWorld.Player;
            Transform transform = camera.transform;
            transform.position = new Vector3(player.WorldPosition.x, player.WorldPosition.y, transform.position.z);

            this.Done = true;
        }

        protected void CreateItems(IEnumerable<IJoyObject> items, bool active = true)
        {
            if (items.Any() == false)
            {
                return;
            }
            
            IGameManager gameManager = GlobalConstants.GameManager;
            int itemLayer = LayerMask.NameToLayer("Objects");
            foreach (IJoyObject item in items)
            {
                if (item is ItemInstance itemInstance)
                {
                    itemInstance.Instantiate(gameManager.ItemPool.Get());
                    itemInstance.MonoBehaviourHandler.gameObject.layer = itemLayer;
                    itemInstance.MonoBehaviourHandler.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Objects";
                    itemInstance.MonoBehaviourHandler.gameObject.SetActive(active);
                    if (itemInstance.Contents.IsNullOrEmpty() == false)
                    {
                        this.CreateItems(itemInstance.Contents, false);
                    }
                }
            }
        }

        public override GameState GetNextState()
        {
            return new WorldState(this.m_Overworld, this.m_ActiveWorld);
        }
    }
}
