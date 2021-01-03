﻿using System;
using System.Collections;
using JoyLib.Code.Conversation;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Helpers;
using JoyLib.Code.IO;
using JoyLib.Code.Physics;
using JoyLib.Code.Unity;
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.States
{
    public class WorldState : GameState
    {
        protected IWorldInstance m_ActiveWorld;

        protected IWorldInstance m_Overworld;

        protected Camera m_Camera;

        protected DateTime m_DateTime;

        protected const int TICK_TIMER = 50;
        protected double m_TickTimer;

        protected GameObject m_FogOfWarHolder;

        protected readonly WorldSerialiser m_WorldSerialiser;

        protected IGameManager GameManager { get; set; }
        protected IPhysicsManager PhysicsManager { get; set; }
        protected IEntityRelationshipHandler RelationshipHandler { get; set; }
        protected IConversationEngine ConversationEngine { get; set; }

        protected IEnumerator TickTimer { get; set; }

        protected IJoyObject PrimaryTarget { get; set; }

        public WorldState(IWorldInstance overworldRef, IWorldInstance activeWorldRef) : base()
        {
            this.m_WorldSerialiser = new WorldSerialiser();

            this.m_ActiveWorld = activeWorldRef;
            this.m_Overworld = overworldRef;

            this.m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            this.m_FogOfWarHolder = GameObject.Find("WorldFog");

            this.GameManager = GlobalConstants.GameManager;
            this.PhysicsManager = this.GameManager.PhysicsManager;
            this.RelationshipHandler = this.GameManager.RelationshipHandler;
            this.ConversationEngine = this.GameManager.ConversationEngine;
            this.GUIManager = this.GameManager.GUIManager;

            this.TickTimer = this.TickEvent();
            this.GameManager.MyGameObject.GetComponent<MonoBehaviour>().StartCoroutine(this.TickTimer);

            //GlobalConstants.GameManager.Player = m_ActiveWorld.Player;

            this.m_ActiveWorld.Tick();
        }

        public override void LoadContent()
        { }

        public override void SetUpUi()
        {
            this.GUIManager.CloseAllOtherGUIs();
            this.GUIManager.OpenGUI(GUINames.NEEDSRECT);
            this.GUIManager.OpenGUI(GUINames.DERIVED_VALUES);

            this.GUIManager.GetGUI(GUINames.INVENTORY).GetComponent<ItemContainer>().Owner = this.PlayerWorld.Player;
            //GUIManager.GetGUI(GUINames.EQUIPMENT).GetComponent<ItemContainer>().Owner = this.PlayerWorld.Player;

            EquipmentHandler equipmentHandler = this.GUIManager.GetGUI(GUINames.EQUIPMENT).GetComponent<EquipmentHandler>();
            equipmentHandler.SetPlayer(this.m_ActiveWorld.Player);
        }

        public override void Start()
        {
            this.m_ActiveWorld.Player.Tick();

            this.SetEntityWorld(this.Overworld);

            this.SetUpUi();
        }

        public override void Stop()
        {
            this.m_WorldSerialiser.Serialise(this.m_Overworld);
        }

        protected void SetEntityWorld(IWorldInstance world)
        {
            for (int i = 0; i < world.Entities.Count; i++)
            {
                world.Entities[i].MyWorld = world;
            }

            foreach (IWorldInstance nextWorld in world.Areas.Values)
            {
                this.SetEntityWorld(nextWorld);
            }
        }

        protected void ChangeWorld(IWorldInstance newWorld, Vector2Int spawnPoint)
        {
            this.Done = true;

            IWorldInstance oldWorld = this.m_ActiveWorld;
            IEntity player = oldWorld.Player;

            player.FetchAction("enterworldaction")
                .Execute(
                    new IJoyObject[] {player},
                    new[] {"exploration", "world change"},
                    new object[] {newWorld});
            this.m_ActiveWorld = newWorld;

            player = this.m_ActiveWorld.Player;

            player.Move(spawnPoint);
            player.Tick();

            this.Tick();
        }

        protected IEnumerator TickEvent()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (this.AutoTurn)
                {
                    this.Tick();
                }
            }
        }
        
        /*
        protected void OnMouseOverJoyObject(object sender, JoyObjectMouseOverEventArgs args)
        {
            if (GUIManager.IsActive(GUINames.CONTEXT_MENU) == false)
            {
                PrimaryTarget = args.Actor;
                SetUpTooltip();
            }
        }

        protected void OnMouseExitJoyObject(object sender, EventArgs args)
        {
            if (GUIManager.IsActive(GUINames.CONTEXT_MENU) == false)
            {
                PrimaryTarget = null;
            }

            GUIManager.CloseGUI(GUINames.TOOLTIP);
        }

        protected void SetUpTooltip()
        {
            if (m_ActiveWorld.Player.VisionProvider.CanSee(m_ActiveWorld.Player, m_ActiveWorld,
                PrimaryTarget.WorldPosition) == false)
            {
                return;
            }

            if (PrimaryTarget is IEntity entity)
            {
                string relationshipName = "You";
                if (PrimaryTarget.GUID != m_ActiveWorld.Player.GUID)
                {
                    try
                    {
                        relationshipName = RelationshipHandler.GetBestRelationship(
                            GlobalConstants.GameManager.Player,
                            PrimaryTarget).DisplayName;
                    }
                    catch (Exception e)
                    {
                        relationshipName = "Stranger";
                    }
                }

                List<Tuple<string, string>> data = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("Relationship: ", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(relationshipName)),
                    new Tuple<string, string>("Gender: ", entity.Gender.Name),
                    new Tuple<string, string>("Job: ", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(entity.CurrentJob.Name)),
                    new Tuple<string, string>("Species: ", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(entity.CreatureType))
                };

                this.GUIManager.OpenGUI(GUINames.TOOLTIP)
                    .GetComponent<Tooltip>()
                    .Show(
                        entity.JoyName,
                        null,
                        entity.Sprite,
                        data);
            }
            else if (PrimaryTarget is IItemInstance item)
            {
                this.GUIManager.OpenGUI(GUINames.TOOLTIP)
                    .GetComponent<Tooltip>()
                    .Show(
                        item.JoyName,
                        null,
                        item.Sprite,
                        item.Tooltip);
            }
        }
        */

        public override void HandleInput(object data, InputActionChange change)
        {
            bool hasMoved = false;

            IEntity player = this.m_ActiveWorld.Player;

            if (player.FulfillmentData.Counter <= 0 && this.AutoTurn)
            {
                this.ManualAutoTurn = false;
                this.AutoTurn = true;
            }

            if (this.AutoTurn)
            {
                return;
            }

            /*
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorld = m_Camera.ScreenToWorldPoint(Input.mousePosition);
                int x = (int)mouseWorld.x;
                int y = (int)mouseWorld.y;

                Pathfinder pathfinder = new Pathfinder();
                Queue<Vector2Int> path = pathfinder.FindPath(player.WorldPosition, new Vector2Int(x, y), m_ActiveWorld);
                player.SetPath(path);
                autoTurn = true;
            }
            */
            
            if (change != InputActionChange.ActionPerformed)
            {
                return;
            }

            if (!(data is InputAction inputAction))
            {
                return;
            }

            Vector2Int newPlayerPoint = this.m_ActiveWorld.Player.WorldPosition;

            if (inputAction.name.Equals("close all windows", StringComparison.OrdinalIgnoreCase))
            {
                this.GUIManager.CloseAllOtherGUIs(GUINames.NEEDSRECT);
            }

            if (inputAction.name.Equals("toggle inventory", StringComparison.OrdinalIgnoreCase))
            {
                this.GUIManager.ToggleGUI(GUINames.INVENTORY);
            }
            else if (inputAction.name.Equals("toggle equipment", StringComparison.OrdinalIgnoreCase))
            {
                this.GUIManager.ToggleGUI(GUINames.EQUIPMENT);
            }
            else if (inputAction.name.Equals("toggle journal", StringComparison.OrdinalIgnoreCase))
            {
                this.GUIManager.ToggleGUI(GUINames.QUEST_JOURNAL);
            }
            else if (inputAction.name.Equals("toggle job management", StringComparison.OrdinalIgnoreCase))
            {
                this.GUIManager.ToggleGUI(GUINames.JOB_MANAGEMENT);
            }
            else if (inputAction.name.Equals("toggle character sheet", StringComparison.OrdinalIgnoreCase))
            {
                this.GUIManager.ToggleGUI(GUINames.CHARACTER_SHEET);
            }

            if (inputAction.name.Equals("interact", StringComparison.OrdinalIgnoreCase))
            {
                //Going up a level
                if (this.m_ActiveWorld.Parent != null && player.WorldPosition == this.m_ActiveWorld.SpawnPoint &&
                         !player.HasMoved)
                {
                    this.ChangeWorld(this.m_ActiveWorld.Parent, this.m_ActiveWorld.GetTransitionPointForParent());
                    return;
                }
                //Going down a level
                if (this.m_ActiveWorld.Areas.ContainsKey(player.WorldPosition) && !player.HasMoved)
                {
                    this.ChangeWorld(this.m_ActiveWorld.Areas[player.WorldPosition], this.m_ActiveWorld.Areas[player.WorldPosition].SpawnPoint);
                    return;
                }
            }

            if (inputAction.name.Equals("skip turn", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("TICK");
                this.Tick();
            }
            //North
            else if (inputAction.name.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                newPlayerPoint.y += 1;
                hasMoved = true;
            }
            //North east
            else if (inputAction.name.Equals("NE", StringComparison.OrdinalIgnoreCase))
            {
                newPlayerPoint.x += 1;
                newPlayerPoint.y += 1;
                hasMoved = true;
            }
            //East
            else if (inputAction.name.Equals("E", StringComparison.OrdinalIgnoreCase))
            {
                newPlayerPoint.x += 1;
                hasMoved = true;
            }
            //South east
            else if (inputAction.name.Equals("SE", StringComparison.OrdinalIgnoreCase))
            {
                newPlayerPoint.x += 1;
                newPlayerPoint.y -= 1;
                hasMoved = true;
            }
            //South
            else if (inputAction.name.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                newPlayerPoint.y -= 1;
                hasMoved = true;
            }
            //South west
            else if (inputAction.name.Equals("SW", StringComparison.OrdinalIgnoreCase))
            {
                newPlayerPoint.x -= 1;
                newPlayerPoint.y -= 1;
                hasMoved = true;
            }
            //West
            else if (inputAction.name.Equals("W", StringComparison.OrdinalIgnoreCase))
            {
                newPlayerPoint.x -= 1;
                hasMoved = true;
            }
            //North west
            else if (inputAction.name.Equals("NW", StringComparison.OrdinalIgnoreCase))
            {
                newPlayerPoint.x -= 1;
                newPlayerPoint.y += 1;
                hasMoved = true;
            }

            if (hasMoved)
            {
                PhysicsResult physicsResult = this.PhysicsManager.IsCollision(player.WorldPosition, newPlayerPoint, this.m_ActiveWorld);

                if (physicsResult == PhysicsResult.EntityCollision)
                {
                    IEntity tempEntity = this.m_ActiveWorld.GetEntity(newPlayerPoint);
                    this.PlayerWorld.SwapPosition(player, tempEntity);
                    this.Tick();
                        
                    /*
                        if (m_GameplayFlags == GameplayFlags.Interacting)
                        { }
                        else if (m_GameplayFlags == GameplayFlags.Giving)
                        { }
                        else if (m_GameplayFlags == GameplayFlags.Moving)
                        {
                            
                        }
                        else if (m_GameplayFlags == GameplayFlags.Attacking)
                        {
                            if (tempEntity.GUID != player.GUID)
                            {
                                //CombatEngine.SwingWeapon(player, tempEntity);
                                IEnumerable<IRelationship> relationships =
                                    RelationshipHandler.Get(new IJoyObject[] {tempEntity, player});
                                foreach (IRelationship relationship in relationships)
                                {
                                    relationship.ModifyValueOfParticipant(player.GUID, tempEntity.GUID, -50);
                                }

                                if (!tempEntity.Alive)
                                {
                                    m_ActiveWorld.RemoveEntity(newPlayerPoint);

                                    //Find a way to remove the GameObject
                                    for (int i = 0; i < m_EntitiesHolder.transform.childCount; i++)
                                    {
                                        if (m_EntitiesHolder.transform.GetChild(i).name
                                            .Contains(tempEntity.GUID.ToString()))
                                        {
                                            this.GameManager.EntityPool.Retire(m_EntitiesHolder.transform.GetChild(i)
                                                .gameObject);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        */
                }
                else if (physicsResult == PhysicsResult.WallCollision)
                {
                    //Do nothing!
                }
                else
                {
                    if (newPlayerPoint.x >= 0 && newPlayerPoint.x < this.m_ActiveWorld.Tiles.GetLength(0) &&
                        newPlayerPoint.y >= 0 && newPlayerPoint.y < this.m_ActiveWorld.Tiles.GetLength(1))
                    {
                        player.Move(newPlayerPoint);
                        this.Tick();
                    }
                }
            }

            if (this.GUIManager.AreAnyOpen() == false)
            {
                this.GUIManager.OpenGUI(GUINames.NEEDSRECT);
                this.GUIManager.OpenGUI(GUINames.DERIVED_VALUES);
            }

            if (this.GUIManager.RemovesControl())
            {
                return;
            }

            /*
                else if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    if (player.TargetingAbility.TargetType == AbilityTarget.Adjacent)
                    {
                        if (AdjacencyHelper.IsAdjacent(player.WorldPosition, player.TargetPoint))
                        {
                            IEntity tempEntity = m_ActiveWorld.GetEntity(player.TargetPoint);
                            if (tempEntity != null && Input.GetKeyDown(KeyCode.Return))
                            {
                                player.TargetingAbility.OnUse(player, tempEntity);
                                Tick();
                                m_GameplayFlags = GameplayFlags.Moving;
                            }
                        }
                    }
                    else if (player.TargetingAbility.TargetType == AbilityTarget.Ranged)
                    {
                        IEntity tempEntity = m_ActiveWorld.GetEntity(player.TargetPoint);
                        if (tempEntity != null && Input.GetKeyDown(KeyCode.Return))
                        {
                            player.TargetingAbility.OnUse(player, tempEntity);
                            Tick();
                            m_GameplayFlags = GameplayFlags.Moving;
                        }
                    }
                }
                */

            this.m_Camera.transform.position = new Vector3(player.WorldPosition.x, player.WorldPosition.y, this.m_Camera.transform.position.z);
        }

        protected void Tick()
        {
            this.m_ActiveWorld.Tick();

            /*
            for (int i = 0; i < s_ActiveWorld.entities.Count; i++)
            {
                if (s_ActiveWorld.entities[i].GUID % 10 == tickCounter)
                {
                    Thread childThread = new Thread(new ThreadStart(s_ActiveWorld.entities[i].Tick));
                    childThread.Start();
                }
            }

            tickCounter += 1;
            tickCounter %= 10;
            */
        }

        protected void DrawObjects()
        {
            IEntity player = this.m_ActiveWorld.Player;
            for (int i = 0; i < this.m_FogOfWarHolder.transform.childCount; i++)
            {
                GameObject fog = this.m_FogOfWarHolder.transform.GetChild(i).gameObject;
                var transformPosition = fog.transform.position;
                Vector2Int position = new Vector2Int((int) transformPosition.x, (int) transformPosition.y);

                bool canSee = player.VisionProvider.CanSee(player, this.m_ActiveWorld, position);
                if (canSee)
                {
                    int lightLevel = this.m_ActiveWorld.LightCalculator.Light.GetLight(position);
                    fog.GetComponent<SpriteRenderer>().color = LightLevelHelper.GetColour(
                        lightLevel,
                        player.VisionProvider.MinimumLightLevel,
                        player.VisionProvider.MaximumLightLevel);
                }
                else
                {
                    fog.GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
        }

        public override void Update()
        {
            IEntity player = this.m_ActiveWorld.Player;

            if (!this.AutoTurn && player.FulfillmentData.Counter > 0)
            {
                this.AutoTurn = true;
            }
            else if (this.AutoTurn && player.FulfillmentData.Counter <= 0 && !this.ManualAutoTurn)
            {
                this.AutoTurn = false;
            }

            this.DrawObjects();
        }

        public override GameState GetNextState()
        {
            this.GameManager.MyGameObject.GetComponent<MonoBehaviour>().StopCoroutine(this.TickTimer);
            return new WorldDestructionState(this.m_Overworld, this.m_ActiveWorld);
        }

        public IWorldInstance Overworld => this.m_Overworld;

        public IWorldInstance PlayerWorld => this.m_ActiveWorld;

        protected int TickCounter { get; set; }

        protected bool AutoTurn { get; set; }

        protected bool ManualAutoTurn { get; set; }

        protected bool ExpandConsole { get; set; }
    }
}