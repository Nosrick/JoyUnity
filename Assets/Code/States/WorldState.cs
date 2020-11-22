using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Helpers;
using JoyLib.Code.IO;
using JoyLib.Code.Physics;
using JoyLib.Code.States.Gameplay;
using JoyLib.Code.World;
using System;
using System.Collections;
using DevionGames.UIWidgets;
using JoyLib.Code.Conversation;
using JoyLib.Code.Unity;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using ContextMenu = DevionGames.UIWidgets.ContextMenu;
using EquipmentHandler = JoyLib.Code.Unity.GUI.EquipmentHandler;

namespace JoyLib.Code.States
{
    public class WorldState : GameState
    {
        protected WorldInstance m_ActiveWorld;

        protected WorldInstance m_Overworld;

        protected GameplayFlags m_GameplayFlags;

        protected Camera m_Camera;

        protected DateTime m_DateTime;

        protected const int TICK_TIMER = 50;
        protected double m_TickTimer;

        protected GameObject m_FogOfWarHolder;
        protected GameObject m_EntitiesHolder;

        protected static GUIManager s_GUIManager;

        protected readonly WorldSerialiser m_WorldSerialiser;

        protected GameObject m_GameManager;
        protected PhysicsManager m_PhysicsManager;
        protected EntityRelationshipHandler m_RelationshipHandler;
        protected ConversationEngine m_ConversationEngine;
        
        protected IEnumerator TickTimer { get; set; }
        
        protected IJoyObject PrimaryTarget { get; set; }

        private const string NEEDSRECT = "NeedsRect";
        private const string INVENTORY = "Inventory";
        private const string EQUIPMENT = "Equipment";
        private const string CONVERSATION = "Conversation Window";
        private const string CONTEXT_MENU = "Context Menu";
        private const string TRADE = "Trade";

        protected static bool SetUpGUI
        {
            get;
            set;
        }

        public WorldState(WorldInstance overworldRef, WorldInstance activeWorldRef, GameplayFlags flagsRef) : base()
        {
            m_WorldSerialiser = new WorldSerialiser();

            m_ActiveWorld = activeWorldRef;
            m_GameplayFlags = flagsRef;
            m_Overworld = overworldRef;

            m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            m_FogOfWarHolder = GameObject.Find("WorldFog");
            m_EntitiesHolder = GameObject.Find("WorldEntities");

            m_GameManager = GameObject.Find("GameManager");
            m_PhysicsManager = m_GameManager.GetComponent<PhysicsManager>();
            m_RelationshipHandler = m_GameManager.GetComponent<EntityRelationshipHandler>();
            m_ConversationEngine = m_GameManager.GetComponent<ConversationEngine>();
            s_GUIManager = m_GameManager.GetComponent<GUIManager>();

            TickTimer = TickEvent();
            m_GameManager.GetComponent<MonoBehaviour>().StartCoroutine(TickTimer);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void SetUpUi()
        {
            base.SetUpUi();

            if (!SetUpGUI)
            {
                GameObject needsGUIPrefab = GameObject.Find(NEEDSRECT);
                GameObject inventoryGUIPrefab = GameObject.Find(INVENTORY);
                GameObject equipmentGUIPrefab = GameObject.Find(EQUIPMENT);
                GameObject conversationWindow = GameObject.Find(CONVERSATION);
                GameObject contextMenu = GameObject.Find(CONTEXT_MENU);
                GameObject tradeWindow = GameObject.Find(TRADE);
    
                s_GUIManager.AddGUI(needsGUIPrefab, false, false);
                s_GUIManager.AddGUI(inventoryGUIPrefab, true, false);
                s_GUIManager.AddGUI(equipmentGUIPrefab, true, false);
                s_GUIManager.AddGUI(conversationWindow, true, true);
                s_GUIManager.AddGUI(contextMenu, false, false);
                s_GUIManager.AddGUI(tradeWindow, true, true);

                SetUpGUI = true;
            }
            
            s_GUIManager.CloseAllOtherGUIs();
            s_GUIManager.OpenGUI(NEEDSRECT);

            EquipmentHandler equipmentHandler = WidgetUtility.Find<MutableItemContainer>("Equipment").gameObject.GetComponent<EquipmentHandler>();
            equipmentHandler.SetPlayer(m_ActiveWorld.Player);
        }

        public override void Start()
        {
            base.Start();
            m_ActiveWorld.Player.UpdateMe();
            m_GameplayFlags = GameplayFlags.Moving;

            SetEntityWorld(Overworld);

            SetUpUi();
        }

        public override void Stop()
        {
            base.Stop();
            m_WorldSerialiser.Serialise(m_Overworld);
        }

        protected void SetEntityWorld(WorldInstance world)
        {
            for(int i = 0; i < world.Entities.Count; i++)
            {
                world.Entities[i].MyWorld = world;
            }

            foreach(WorldInstance nextWorld in world.Areas.Values)
            {
                SetEntityWorld(nextWorld);
            }
        }

        protected void ChangeWorld(WorldInstance newWorld, Vector2Int spawnPoint)
        {
            Done = true;

            WorldInstance oldWorld = m_ActiveWorld;
            Entity player = oldWorld.Player;

            player.FetchAction("enterworldaction")
                .Execute(
                    new IJoyObject[] {player},
                    new[] {"exploration", "world change"},
                    new object[] {newWorld});
            m_ActiveWorld = newWorld;

            player = m_ActiveWorld.Player;

            player.Move(spawnPoint);
            player.UpdateMe();

            m_GameplayFlags = GameplayFlags.Moving;
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

        protected void SetUpContextMenu()
        {
            ContextMenu contextMenu = s_GUIManager.GetGUI(CONTEXT_MENU).GetComponent<ContextMenu>();

            contextMenu.Clear();

            if (PrimaryTarget.GUID == m_ActiveWorld.Player.GUID)
            {
                
            }
            else
            {
                if (AdjacencyHelper.IsAdjacent(m_ActiveWorld.Player.WorldPosition, PrimaryTarget.WorldPosition))
                {
                    contextMenu.AddMenuItem("Talk", TalkToPlayer);
                }
                else
                {
                    contextMenu.AddMenuItem("Call Over", CallOver);
                }
            }

            if (contextMenu.GetComponentsInChildren<MenuItem>(true).Length > 1)
            {
                s_GUIManager.OpenGUI(CONTEXT_MENU);
                contextMenu.Show();
            }
        }

        protected void TalkToPlayer()
        {
            if (!(PrimaryTarget is Entity entity))
            {
                return;
            }

            ContextMenu contextMenu = s_GUIManager.GetGUI(CONTEXT_MENU).GetComponent<ContextMenu>();
            s_GUIManager.CloseGUI(CONTEXT_MENU);
            contextMenu.Close();
            s_GUIManager.OpenGUI(CONVERSATION);
            m_ConversationEngine.SetActors(m_ActiveWorld.Player, entity);
            m_ConversationEngine.Converse();
        }

        protected void CallOver()
        {
            if (!(PrimaryTarget is Entity entity))
            {
                return;
            }

            entity.FetchAction("seekaction")
                .Execute(
                new IJoyObject[] {entity, m_ActiveWorld.Player},
                new[] {"call over"},
                new object[] {"friendship"});
        }

        public override void HandleInput()
        {
            base.HandleInput();

            bool hasMoved = false;

            Entity player = m_ActiveWorld.Player;

            if (Input.GetKeyUp(KeyCode.Space))
            {
                ManualAutoTurn = !ManualAutoTurn;
                AutoTurn = !AutoTurn;
            }

            if (AutoTurn)
            {
                return;
            }

            /*
            if (m_Input.currentMouseState.ScrollWheelValue > m_Input.lastMouseState.ScrollWheelValue)
            {
                m_Camera.zoom += 0.05f;
            }

            if (m_Input.currentMouseState.ScrollWheelValue < m_Input.lastMouseState.ScrollWheelValue)
            {
                m_Camera.zoom -= 0.05f;
            }

            if(m_Input.IsOldPress(MouseButtons.RightButton))
            {
                m_GUIManager.Screen.Desktop.Children.Remove(m_ContextMenu);
                m_ContextMenu.MoveTo(m_Input.currentMouseState.Position);
                m_GUIManager.Screen.Desktop.Children.Add(m_ContextMenu);

                Vector2 mouseVector = (m_Input.currentMouseState.Position - (new Point(m_Renderer.m_ScreenWidth / 2, m_Renderer.m_ScreenHeight / 2))).ToVector2();
                m_MenuTile = new Point((int)Math.Floor(mouseVector.X / (ObjectIcons.SPRITE_SIZE * m_Camera.zoom)), (int)Math.Floor(mouseVector.Y / (ObjectIcons.SPRITE_SIZE * m_Camera.zoom))) + s_ActiveWorld.player.position;
            }

            if(m_Input.IsOldPress(Keys.Space))
            {
                autoTurn = !autoTurn;
            }

            if(m_Input.IsOldPress(Keys.L))
            {
                s_ActiveWorld.player.LevelUp();
            }
            
            */

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
            if (Input.GetKeyDown(KeyCode.I))
            {
                s_GUIManager.ToggleGUI(INVENTORY);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                s_GUIManager.ToggleGUI(EQUIPMENT);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                if (s_GUIManager.IsActive(CONVERSATION))
                {
                    s_GUIManager.CloseGUI(CONVERSATION);
                }
                else
                {
                    //TODO: Make a targeting system or something
                    Entity listener = this.m_ActiveWorld.GetRandomSentient();

                    if (!(listener is null))
                    {
                        s_GUIManager.OpenGUI(CONVERSATION);
                        m_ConversationEngine.SetActors(this.PlayerWorld.Player, listener);
                        m_ConversationEngine.Converse(); 
                    }
                }
            }

            if (s_GUIManager.RemovesControl())
            {
                return;
            }


            if (Input.GetMouseButtonUp(1))
            {
                Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2Int position = new Vector2Int((int)Math.Ceiling(temp.x), (int)Math.Ceiling(temp.y));

                PrimaryTarget = m_ActiveWorld.GetEntity(position);
                if (PrimaryTarget is null)
                {
                    return;
                }
                
                SetUpContextMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                //Going up a level
                if (m_ActiveWorld.Parent != null && player.WorldPosition == m_ActiveWorld.SpawnPoint && !player.HasMoved)
                {
                    ChangeWorld(m_ActiveWorld.Parent, m_ActiveWorld.GetTransitionPointForParent());
                    return;
                }

                //Going down a level
                else if (m_ActiveWorld.Areas.ContainsKey(player.WorldPosition) && !player.HasMoved)
                { 
                    ChangeWorld(m_ActiveWorld.Areas[player.WorldPosition], m_ActiveWorld.Areas[player.WorldPosition].SpawnPoint);
                    return;
                }

                /*
                PhysicsResult physicsResult = m_PhysicsManager.IsCollision(player.WorldPosition, player.WorldPosition, m_ActiveWorld);
                if (physicsResult == PhysicsResult.ObjectCollision)
                {
                    //Get the item picked up
                    ItemInstance pickUp = m_ActiveWorld.PickUpObject(player);

                    //And try to destroy the corresponding GameObject
                    if (pickUp != null)
                    {
                        GameObject.Destroy(GameObject.Find(pickUp.JoyName + ":" + pickUp.GUID));
                    }
                }
                */
            }
            Vector2Int newPlayerPoint = m_ActiveWorld.Player.WorldPosition;

            //North
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    player.TargetPoint = new Vector2Int(player.TargetPoint.x, player.TargetPoint.y - 1);
                }
                else
                {
                    newPlayerPoint.y += 1;
                    hasMoved = true;
                }
            }
            //North east
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    player.TargetPoint = new Vector2Int(player.TargetPoint.x + 1, player.TargetPoint.y - 1);
                }
                else
                {
                    newPlayerPoint.x += 1;
                    newPlayerPoint.y += 1;
                    hasMoved = true;
                }
            }
            //East
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    player.TargetPoint = new Vector2Int(player.TargetPoint.x + 1, player.TargetPoint.y);
                }
                else
                {
                    newPlayerPoint.x += 1;
                    hasMoved = true;
                }
            }
            //South east
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    player.TargetPoint = new Vector2Int(player.TargetPoint.x + 1, player.TargetPoint.y + 1);
                }
                else
                {
                    newPlayerPoint.x += 1;
                    newPlayerPoint.y -= 1;
                    hasMoved = true;
                }
            }
            //South
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    player.TargetPoint = new Vector2Int(player.TargetPoint.x, player.TargetPoint.y + 1);
                }
                else
                {
                    newPlayerPoint.y -= 1;
                    hasMoved = true;
                }
            }
            //South west
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    player.TargetPoint = new Vector2Int(player.TargetPoint.x - 1, player.TargetPoint.y + 1);
                }
                else
                {
                    newPlayerPoint.x -= 1;
                    newPlayerPoint.y -= 1;
                    hasMoved = true;
                }
            }
            //West
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    player.TargetPoint = new Vector2Int(player.TargetPoint.x - 1, player.TargetPoint.y);
                }
                else
                {
                    newPlayerPoint.x -= 1;
                    hasMoved = true;
                }
            }
            //North west
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                if (m_GameplayFlags == GameplayFlags.Targeting)
                {
                    player.TargetPoint = new Vector2Int(player.TargetPoint.x - 1, player.TargetPoint.y - 1);
                }
                else
                {
                    newPlayerPoint.x -= 1;
                    newPlayerPoint.y += 1;
                    hasMoved = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                Tick();
                return;
            }

            if (hasMoved)
            {
                PhysicsResult physicsResult = m_PhysicsManager.IsCollision(player.WorldPosition, newPlayerPoint, m_ActiveWorld);

                if (physicsResult == PhysicsResult.EntityCollision)
                {
                    Entity tempEntity = m_ActiveWorld.GetEntity(newPlayerPoint);
                    if (m_GameplayFlags == GameplayFlags.Interacting)
                    {
                    }
                    else if (m_GameplayFlags == GameplayFlags.Giving)
                    {
                    }
                    else if (m_GameplayFlags == GameplayFlags.Moving)
                    {
                        PlayerWorld.SwapPosition(player, tempEntity);
                        Tick();
                    }
                    else if(m_GameplayFlags == GameplayFlags.Attacking)
                    {
                        if (tempEntity.GUID != player.GUID)
                        {
                            //TODO: REDO COMBAT ENGINE
                            //CombatEngine.SwingWeapon(player, tempEntity);
                            IRelationship[] relationships = m_RelationshipHandler.Get(new JoyObject[] { tempEntity, player });
                            foreach(IRelationship relationship in relationships)
                            {
                                relationship.ModifyValueOfParticipant(player.GUID, tempEntity.GUID, -50);
                            }

                            if (!tempEntity.Alive)
                            {
                                m_ActiveWorld.RemoveEntity(newPlayerPoint);

                                //Find a way to remove the GameObject
                                for(int i = 0; i < m_EntitiesHolder.transform.childCount; i++)
                                {
                                    if(m_EntitiesHolder.transform.GetChild(i).name.Contains(tempEntity.GUID.ToString()))
                                    {
                                        GameObject.Destroy(m_EntitiesHolder.transform.GetChild(i).gameObject);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    Tick();
                }
                else if (physicsResult == PhysicsResult.WallCollision)
                {
                    //Do nothing!
                }
                else
                {
                    if (newPlayerPoint.x >= 0 && newPlayerPoint.x < m_ActiveWorld.Tiles.GetLength(0) && newPlayerPoint.y >= 0 && newPlayerPoint.y < m_ActiveWorld.Tiles.GetLength(1))
                    {
                        player.Move(newPlayerPoint);
                        Tick();
                    }
                }
            }
            else if (m_GameplayFlags == GameplayFlags.Targeting)
            {
                if (player.TargetingAbility.TargetType == AbilityTarget.Adjacent)
                {
                    if (AdjacencyHelper.IsAdjacent(player.WorldPosition, player.TargetPoint))
                    {
                        Entity tempEntity = m_ActiveWorld.GetEntity(player.TargetPoint);
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
                    Entity tempEntity = m_ActiveWorld.GetEntity(player.TargetPoint);
                    if(tempEntity != null && Input.GetKeyDown(KeyCode.Return))
                    {
                        player.TargetingAbility.OnUse(player, tempEntity);
                        Tick();
                        m_GameplayFlags = GameplayFlags.Moving;
                    }
                }
            }
            m_Camera.transform.position = new Vector3(player.WorldPosition.x, player.WorldPosition.y, m_Camera.transform.position.z);
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

        public override void OnGui()
        {
            base.OnGui();

            GameObject needsText = s_GUIManager.GetGUI("NeedsText");
            //needsText.GetComponent<TextMeshProUGUI>();
        }

        public override void Draw()
        {
        }

        protected void DrawTargetCursor()
        {
            if (m_GameplayFlags != GameplayFlags.Targeting)
                return;
        }

        protected void DrawTiles()
        {
            lock(m_ActiveWorld.Tiles)
            {
            }
        }

        protected void DrawStairs()
        {
        }

        protected void DrawObjects()
        {
            Entity player = m_ActiveWorld.Player;
            bool[,] vision = player.Vision;
            for (int i = 0; i < m_FogOfWarHolder.transform.childCount; i++)
            {
                GameObject fog = m_FogOfWarHolder.transform.GetChild(i).gameObject;
                var transformPosition = fog.transform.position;
                Vector2Int position = new Vector2Int((int)transformPosition.x, (int)transformPosition.y);

                bool visible = vision[position.x, position.y];
                int lightLevel = visible ? 16 : 0;

                fog.GetComponent<SpriteRenderer>().color = LightLevelHelper.GetColour(lightLevel);
            }
        }

        protected void DrawEntities()
        {
        }

        public override void Update()
        {
            Entity player = m_ActiveWorld.Player;
            
            if (!AutoTurn && player.FulfillmentData.Counter > 0)
            {
                AutoTurn = true;
            }
            else if (AutoTurn && player.FulfillmentData.Counter <= 0 && !ManualAutoTurn)
            {
                AutoTurn = false;
            }
            
            base.Update();
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
            m_GameManager.GetComponent<MonoBehaviour>().StopCoroutine(TickTimer);
            return new WorldDestructionState(m_Overworld, m_ActiveWorld);
        }

        public WorldInstance Overworld => m_Overworld;

        public WorldInstance PlayerWorld => m_ActiveWorld;

        protected int TickCounter
        {
            get;
            set;
        }

        protected bool AutoTurn
        {
            get;
            set;
        }

        protected bool ManualAutoTurn
        {
            get;
            set;
        }

        protected bool ExpandConsole
        {
            get;
            set;
        }
    }
}