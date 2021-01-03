using System.Collections.Generic;
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

            //Make the upstairs
            if (this.m_ActiveWorld.GUID != this.m_Overworld.GUID)
            {
                GameObject child = gameManager.FloorPool.Get();

                child.transform.position = new Vector3(this.m_ActiveWorld.SpawnPoint.x, this.m_ActiveWorld.SpawnPoint.y, 0.0f);
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Walls";
                child.transform.name = this.m_ActiveWorld.Parent.Name + " stairs";
                child.SetActive(true);
                /*
                IJoyObject upstairs = new JoyObject(
                    this.m_ActiveWorld.Parent.Name + " stairs",
                    gameManager.DerivedValueHandler.GetItemStandardBlock(values),
                    this.m_ActiveWorld.SpawnPoint,
                    "Stairs",
                    new IJoyAction[0],
                    this.m_ObjectIcons.GetSprites("Stairs", "UpStairs"));
                upstairs.MyWorld = this.m_ActiveWorld;
                child.GetComponent<MonoBehaviourHandler>().AttachJoyObject(upstairs);
                */
            }

            //Make each downstairs
            foreach(KeyValuePair<Vector2Int, IWorldInstance> pair in this.m_ActiveWorld.Areas)
            {
                GameObject child = gameManager.FloorPool.Get();
                
                Vector2Int position = pair.Key;
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Walls";
                spriteRenderer.sprite = this.m_ObjectIcons.GetSprite("Stairs", "Downstairs");
                child.transform.name = pair.Value.Name + " stairs";
                child.transform.position = new Vector3(pair.Key.x, pair.Key.y);
                child.SetActive(true);
                /*
                IJoyObject downstairs = new JoyObject(
                    pair.Value.Name + " stairs",
                    gameManager.DerivedValueHandler.GetItemStandardBlock(values),
                    position,
                    "Stairs",
                    new IJoyAction[0],
                    this.m_ObjectIcons.GetSprites("Stairs", "Downstairs"));
                downstairs.MyWorld = this.m_ActiveWorld;
                child.GetComponent<MonoBehaviourHandler>().AttachJoyObject(downstairs);
                */
            }

            Vector3 pos = Vector3.zero;
            for(int i = 0; i < this.m_ActiveWorld.Tiles.GetLength(0); i++)
            {
                for(int j = 0; j < this.m_ActiveWorld.Tiles.GetLength(1); j++)
                {
                    pos.x = i;
                    pos.y = j;
                    
                    //Make the fog of war
                    GameObject fog = gameManager.FogPool.Get();
                    fog.transform.position = pos;
                    SpriteRenderer goSpriteRenderer = fog.GetComponent<SpriteRenderer>();
                    goSpriteRenderer.sortingLayerName = "Fog of War";
                    goSpriteRenderer.sprite = this.m_ObjectIcons.GetSprite("Obscure", "Obscure");
                    fog.name = "Fog of War";
                    fog.SetActive(true);
                    /*
                    IJoyObject fogObj = new JoyObject(
                        "Fog of War",
                        gameManager.DerivedValueHandler.GetItemStandardBlock(values),
                        new Vector2Int(i, j),
                        this.m_ActiveWorld.Tiles[i, j].TileSet,
                        new IJoyAction[0],
                        this.m_ObjectIcons.GetSprites("Obscure", "Obscure"));
                    fogObj.MyWorld = this.m_ActiveWorld;
                    fog.GetComponent<MonoBehaviourHandler>().AttachJoyObject(fogObj);
                    */
                    
                    
                    //Make the floor
                    GameObject floor = gameManager.FloorPool.Get();
                    floor.transform.position = pos;
                    SpriteRenderer fogSpriteRenderer = floor.GetComponent<SpriteRenderer>();
                    fogSpriteRenderer.sortingLayerName = "Terrain";
                    fogSpriteRenderer.sprite = this.m_ObjectIcons.GetSprite(this.m_ActiveWorld.Tiles[i, j].TileSet, 
                            //TODO: This will eventually be a tile direction selection algorithm
                            "surroundfloor");
                    floor.name = this.m_ActiveWorld.Name + " floor";
                    floor.SetActive(true);
                    
                    
                    /*
                    IJoyObject floorObj = new JoyObject(
                        this.m_ActiveWorld.Name + " floor",
                        gameManager.DerivedValueHandler.GetItemStandardBlock(values),
                        new Vector2Int(i, j),
                        this.m_ActiveWorld.Tiles[i, j].TileSet,
                        new IJoyAction[0],
                        this.m_ObjectIcons.GetSprites(this.m_ActiveWorld.Tiles[i, j].TileSet, "surroundfloor"));
                    floorObj.MyWorld = this.m_ActiveWorld;
                    floor.GetComponent<MonoBehaviourHandler>().AttachJoyObject(floorObj);
                    */
                }
            }

            //Create the walls
            foreach(IJoyObject wall in this.m_ActiveWorld.Walls.Values)
            {
                wall.MyWorld = this.m_ActiveWorld;
                GameObject gameObject = gameManager.WallPool.Get();
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Walls";
                gameObject.GetComponent<MonoBehaviourHandler>()
                    .AttachJoyObject(wall);
                gameObject.SetActive(true);
            }
            
            this.CreateItems(this.m_ActiveWorld.Objects);
            
            //Create the entities
            foreach(IEntity entity in this.m_ActiveWorld.Entities)
            {
                GameObject gameObject = gameManager.EntityPool.Get();
                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
                gameObject.SetActive(true);
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
            IGameManager gameManager = GlobalConstants.GameManager;
            foreach (IJoyObject item in items)
            {
                if (item is ItemInstance itemInstance)
                {
                    itemInstance.Instantiate(gameManager.ItemPool.Get());
                    itemInstance.MonoBehaviourHandler.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Objects";
                    itemInstance.MonoBehaviourHandler.gameObject.SetActive(active);
                }
            }
        }

        public override GameState GetNextState()
        {
            return new WorldState(this.m_Overworld, this.m_ActiveWorld);
        }
    }
}
