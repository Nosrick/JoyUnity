using System;
using System.Collections;
using System.Globalization;
using System.Text;
using JoyLib.Code.Conversation;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Events;
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
            m_WorldSerialiser = new WorldSerialiser();

            m_ActiveWorld = activeWorldRef;
            m_Overworld = overworldRef;

            m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            m_FogOfWarHolder = GameObject.Find("WorldFog");

            GameManager = GlobalConstants.GameManager;
            PhysicsManager = GameManager.PhysicsManager;
            RelationshipHandler = GameManager.RelationshipHandler;
            ConversationEngine = GameManager.ConversationEngine;
            GUIManager = GameManager.GUIManager;

            TickTimer = TickEvent();
            GameManager.MyGameObject.GetComponent<MonoBehaviour>().StartCoroutine(TickTimer);

            GlobalConstants.GameManager.Player = m_ActiveWorld.Player;

            m_ActiveWorld.Tick();
        }

        public override void LoadContent()
        { }

        public override void SetUpUi()
        {
            foreach (IJoyObject joyObject in m_ActiveWorld.Objects)
            {
                joyObject.MonoBehaviourHandler.OnMouseOverEvent += OnMouseOverJoyObject;
                joyObject.MonoBehaviourHandler.OnMouseExitEvent += OnMouseExitJoyObject;
            }

            foreach (IJoyObject joyObject in m_ActiveWorld.Entities)
            {
                joyObject.MonoBehaviourHandler.OnMouseOverEvent += OnMouseOverJoyObject;
                joyObject.MonoBehaviourHandler.OnMouseExitEvent += OnMouseExitJoyObject;
            }

            GUIManager.CloseAllOtherGUIs();
            GUIManager.OpenGUI(GUINames.NEEDSRECT);
            GUIManager.OpenGUI(GUINames.DERIVED_VALUES);

            GUIManager.GetGUI(GUINames.INVENTORY).GetComponent<ItemContainer>().Owner = this.PlayerWorld.Player;
            GUIManager.GetGUI(GUINames.EQUIPMENT).GetComponent<ItemContainer>().Owner = this.PlayerWorld.Player;

            EquipmentHandler equipmentHandler = GUIManager.GetGUI(GUINames.EQUIPMENT).GetComponent<EquipmentHandler>();
            equipmentHandler.SetPlayer(m_ActiveWorld.Player);
        }

        public override void Start()
        {
            m_ActiveWorld.Player.Tick();

            SetEntityWorld(Overworld);

            SetUpUi();
        }

        public override void Stop()
        {
            m_WorldSerialiser.Serialise(m_Overworld);
        }

        protected void SetEntityWorld(IWorldInstance world)
        {
            for (int i = 0; i < world.Entities.Count; i++)
            {
                world.Entities[i].MyWorld = world;
            }

            foreach (WorldInstance nextWorld in world.Areas.Values)
            {
                SetEntityWorld(nextWorld);
            }
        }

        protected void ChangeWorld(IWorldInstance newWorld, Vector2Int spawnPoint)
        {
            Done = true;

            IWorldInstance oldWorld = m_ActiveWorld;
            IEntity player = oldWorld.Player;

            player.FetchAction("enterworldaction")
                .Execute(
                    new IJoyObject[] {player},
                    new[] {"exploration", "world change"},
                    new object[] {newWorld});
            m_ActiveWorld = newWorld;

            player = m_ActiveWorld.Player;

            player.Move(spawnPoint);
            player.Tick();
            
            Tick();
        }

        protected IEnumerator TickEvent()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                if (AutoTurn)
                {
                    Tick();
                }
            }
        }

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

            Tooltip tooltip = GUIManager.GetGUI(GUINames.TOOLTIP).GetComponent<Tooltip>();
            if (PrimaryTarget is IEntity entity)
            {
                string relationshipName = "You";
                StringBuilder builder = new StringBuilder(entity.Description);
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

                builder.AppendLine(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(relationshipName));

                GUIManager.OpenGUI(GUINames.TOOLTIP);
                tooltip.Show(
                    entity.JoyName,
                    builder.ToString(),
                    entity.Sprite);
            }
            else if (PrimaryTarget is IItemInstance item)
            {
                GUIManager.OpenGUI(GUINames.TOOLTIP);
                tooltip.Show(
                    item.JoyName,
                    item.DisplayDescription,
                    item.Sprite);
            }
        }

        protected void SetUpContextMenu()
        {
            GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            JoyLib.Code.Unity.GUI.ContextMenu contextMenu = GUIManager.GetGUI(GUINames.CONTEXT_MENU)
                .GetComponent<JoyLib.Code.Unity.GUI.ContextMenu>();
            
            contextMenu.Clear();

            if (PrimaryTarget.GUID != m_ActiveWorld.Player.GUID)
            {
                if (AdjacencyHelper.IsAdjacent(m_ActiveWorld.Player.WorldPosition, PrimaryTarget.WorldPosition))
                {
                    if (PrimaryTarget is IEntity actor)
                    {
                        contextMenu.AddMenuItem("Talk", TalkToPlayer);
                    }
                }
                else
                {
                    if (PrimaryTarget is IEntity actor)
                    {
                        contextMenu.AddMenuItem("Call Over", CallOver);
                    }
                }

                if (contextMenu.GetComponentsInChildren<MenuItem>(true).Length > 1)
                {
                    GUIManager.OpenGUI(GUINames.CONTEXT_MENU);
                }
            }
        }

        protected void TalkToPlayer()
        {
            if (!(PrimaryTarget is IEntity entity))
            {
                return;
            }

            GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            GUIManager.OpenGUI(GUINames.CONVERSATION);
            ConversationEngine.SetActors(m_ActiveWorld.Player, entity);
            ConversationEngine.Converse();
        }

        protected void CallOver()
        {
            if (!(PrimaryTarget is IEntity entity))
            {
                return;
            }

            entity.FetchAction("seekaction")
                .Execute(
                    new IJoyObject[] {entity, m_ActiveWorld.Player},
                    new[] {"call over"},
                    new object[] {"friendship"});
        }

        public override void HandleInput(object data, InputActionChange change)
        {
            bool hasMoved = false;

            IEntity player = this.m_ActiveWorld.Player;

            if (player.FulfillmentData.Counter <= 0 && AutoTurn)
            {
                ManualAutoTurn = false;
                AutoTurn = true;
            }

            if (AutoTurn)
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

            if (data is InputAction inputAction)
            {
                if (change != InputActionChange.ActionStarted)
                {
                    return;
                }

                Vector2Int newPlayerPoint = this.m_ActiveWorld.Player.WorldPosition;

                if (inputAction.name.Equals("close all windows", StringComparison.OrdinalIgnoreCase))
                {
                    this.GUIManager.CloseAllOtherGUIs(GUINames.NEEDSRECT);
                    this.GUIManager.OpenGUI(GUINames.DERIVED_VALUES);
                }

                if (inputAction.name.Equals("toggle inventory", StringComparison.OrdinalIgnoreCase))
                {
                    this.GUIManager.ToggleGUI(GUINames.INVENTORY);
                }
                else if (inputAction.name.Equals("toggle equipment", StringComparison.OrdinalIgnoreCase))
                {
                    GUIManager.ToggleGUI(GUINames.EQUIPMENT);
                }
                else if (inputAction.name.Equals("toggle journal", StringComparison.OrdinalIgnoreCase))
                {
                    GUIManager.ToggleGUI(GUINames.QUEST_JOURNAL);
                }
                else if (inputAction.name.Equals("toggle job management", StringComparison.OrdinalIgnoreCase))
                {
                    GUIManager.ToggleGUI(GUINames.JOB_MANAGEMENT);
                }
                else if (inputAction.name.Equals("toggle character sheet", StringComparison.OrdinalIgnoreCase))
                {
                    GUIManager.ToggleGUI(GUINames.CHARACTER_SHEET);
                }

                if (inputAction.name.Equals("interact", StringComparison.OrdinalIgnoreCase))
                {
                    IJoyObject joyObject = this.m_ActiveWorld.GetObject(this.m_ActiveWorld.Player.WorldPosition);
                    if (joyObject is null == false && joyObject.MonoBehaviourHandler is ItemBehaviourHandler itemBehaviourHandler)
                    {
                        itemBehaviourHandler.PickupItems();
                    }
                    //Going up a level
                    else if (m_ActiveWorld.Parent != null && player.WorldPosition == m_ActiveWorld.SpawnPoint &&
                        !player.HasMoved)
                    {
                        ChangeWorld(m_ActiveWorld.Parent, m_ActiveWorld.GetTransitionPointForParent());
                        return;
                    }
                    //Going down a level
                    else if (m_ActiveWorld.Areas.ContainsKey(player.WorldPosition) && !player.HasMoved)
                    {
                        ChangeWorld(m_ActiveWorld.Areas[player.WorldPosition],
                            m_ActiveWorld.Areas[player.WorldPosition].SpawnPoint);
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
                    PhysicsResult physicsResult =
                        PhysicsManager.IsCollision(player.WorldPosition, newPlayerPoint, m_ActiveWorld);

                    if (physicsResult == PhysicsResult.EntityCollision)
                    {
                        IEntity tempEntity = m_ActiveWorld.GetEntity(newPlayerPoint);
                        PlayerWorld.SwapPosition(player, tempEntity);
                        Tick();
                        
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
                        if (newPlayerPoint.x >= 0 && newPlayerPoint.x < m_ActiveWorld.Tiles.GetLength(0) &&
                            newPlayerPoint.y >= 0 && newPlayerPoint.y < m_ActiveWorld.Tiles.GetLength(1))
                        {
                            player.Move(newPlayerPoint);
                            Tick();
                        }
                    }
                }
                
                if (inputAction.name.Equals("right mouse", StringComparison.OrdinalIgnoreCase))
                {
                    Vector3 temp = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                    Vector2Int position = new Vector2Int((int) Math.Ceiling(temp.x), (int) Math.Ceiling(temp.y));
    
                    PrimaryTarget = m_ActiveWorld.GetEntity(position);
                    if (PrimaryTarget is null)
                    {
                        PrimaryTarget = m_ActiveWorld.GetObject(position);
                        if (PrimaryTarget is null)
                        {
                            return;
                        }
                    }
    
                    SetUpContextMenu();
                }

                if (GUIManager.AreAnyOpen() == false)
                {
                    GUIManager.OpenGUI(GUINames.NEEDSRECT);
                    GUIManager.OpenGUI(GUINames.DERIVED_VALUES);
                }

                if (GUIManager.RemovesControl())
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

                m_Camera.transform.position = new Vector3(player.WorldPosition.x, player.WorldPosition.y,
                    m_Camera.transform.position.z);
            }
        }

        protected void Tick()
        {
            m_ActiveWorld.Tick();

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

        protected void DrawTargetCursor()
        {
            
        }

        protected void DrawTiles()
        {
            lock (m_ActiveWorld.Tiles)
            { }
        }

        protected void DrawStairs()
        { }

        protected void DrawObjects()
        {
            IEntity player = m_ActiveWorld.Player;
            for (int i = 0; i < m_FogOfWarHolder.transform.childCount; i++)
            {
                GameObject fog = m_FogOfWarHolder.transform.GetChild(i).gameObject;
                var transformPosition = fog.transform.position;
                Vector2Int position = new Vector2Int((int) transformPosition.x, (int) transformPosition.y);

                bool visible = player.VisionProvider.HasVisibility(player, m_ActiveWorld, position);
                int lightLevel = visible ? m_ActiveWorld.LightCalculator.Light.GetLight(position) : 0;

                fog.GetComponent<SpriteRenderer>().color = LightLevelHelper.GetColour(
                    lightLevel,
                    player.VisionProvider.MinimumLightLevel,
                    player.VisionProvider.MaximumLightLevel);
            }
        }

        protected void DrawEntities()
        { }

        public override void Update()
        {
            IEntity player = m_ActiveWorld.Player;

            if (!AutoTurn && player.FulfillmentData.Counter > 0)
            {
                AutoTurn = true;
            }
            else if (AutoTurn && player.FulfillmentData.Counter <= 0 && !ManualAutoTurn)
            {
                AutoTurn = false;
            }

            DrawObjects();
        }

        private void GainFocus()
        {
            InFocus = true;
        }

        private void LoseFocus()
        {
            InFocus = false;
        }

        public override GameState GetNextState()
        {
            GameManager.MyGameObject.GetComponent<MonoBehaviour>().StopCoroutine(TickTimer);
            return new WorldDestructionState(m_Overworld, m_ActiveWorld);
        }

        public IWorldInstance Overworld => m_Overworld;

        public IWorldInstance PlayerWorld => m_ActiveWorld;

        protected int TickCounter { get; set; }

        protected bool AutoTurn { get; set; }

        protected bool ManualAutoTurn { get; set; }

        protected bool ExpandConsole { get; set; }
    }
}