using JoyLib.Code.Conversation.Subengines;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Helpers;
using JoyLib.Code.IO;
using JoyLib.Code.Quests;
using JoyLib.Code.States.Gameplay;
using JoyLib.Code.World;
using System;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldState : GameState
    {
        protected WorldInstance s_ActiveWorld;

        protected WorldInstance s_Overworld;

        protected GameplayFlags s_GameplayFlags;

        protected DateTime m_DateTime;

        protected const int TICK_TIMER = 50;
        protected double m_TickTimer;

        public WorldState(WorldInstance overworldRef, WorldInstance activeWorldRef, GameplayFlags flagsRef) : base()
        {
            s_ActiveWorld = activeWorldRef;
            s_GameplayFlags = flagsRef;
            s_Overworld = overworldRef;
            SetUpUi();

            //MusicHandler.Play("DashRoog");
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void SetUpUi()
        {
            base.SetUpUi();
        }

        public override void Start()
        {
            base.Start();
            s_ActiveWorld.Player.Vision = s_ActiveWorld.GetVision(s_ActiveWorld.Player);
            s_GameplayFlags = GameplayFlags.Moving;
            RumourMill.GenerateRumours(s_ActiveWorld);

            SetEntityWorld(overworld);
        }

        public override void Stop()
        {
            base.Stop();
            WorldSerialiser.Serialise(s_Overworld);
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
            WorldInstance oldWorld = s_ActiveWorld;
            Entity player = oldWorld.Player;
            s_ActiveWorld.RemoveEntity(player.WorldPosition);

            s_ActiveWorld = newWorld;
            s_ActiveWorld.AddEntity(player);

            s_ActiveWorld.Player.Move(spawnPoint);
            s_ActiveWorld.Player.Vision = s_ActiveWorld.GetVision(s_ActiveWorld.Player);

            player.MyWorld = s_ActiveWorld;

            s_GameplayFlags = GameplayFlags.Moving;
            RumourMill.GenerateRumours(s_ActiveWorld);

            QuestTracker.PerformExploration(player, newWorld);
            Tick();
        }

        public static void TalkToPlayer(Entity entityRef)
        {
        }

        protected void SetToAttack(object sender, EventArgs e)
        {
            s_GameplayFlags = GameplayFlags.Attacking;
        }

        protected void InteractWithCreature(object sender, EventArgs e)
        {
        }

        protected void GiftToCreature(object sender, EventArgs e)
        {
        }

        protected void OpenInventory(object sender, EventArgs e)
        {
        }

        protected void OpenCharacterSheet(object sender, EventArgs e)
        {
        }

        protected void SetToThrow(object sender, EventArgs e)
        {
        }

        public override void HandleInput()
        {
            base.HandleInput();

            if (!InFocus)
                return;

            bool hasMoved = false;

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

            if (m_Input.IsOldPress(Keys.Enter))
            {
                //Going up a level
                if (s_ActiveWorld.parent != null && s_ActiveWorld.player.position == s_ActiveWorld.spawnPoint && !s_ActiveWorld.player.hasMoved)
                {
                    ChangeWorld(s_ActiveWorld.parent, PointStringConverter.StringToPoint(s_ActiveWorld.parent.areas.First(x => x.Value.GUID == s_ActiveWorld.GUID).Key));
                }

                //Going down a level
                else if (s_ActiveWorld.areas.ContainsKey(s_ActiveWorld.player.position.ToString()) && !s_ActiveWorld.player.hasMoved)
                { 
                    ChangeWorld(s_ActiveWorld.areas[s_ActiveWorld.player.position.ToString()], s_ActiveWorld.areas[s_ActiveWorld.player.position.ToString()].spawnPoint);
                }
            }

            Point newPlayerPoint = s_ActiveWorld.player.position;

            //North
            if (m_Input.IsOldPress(Keys.NumPad8))
            {
                if (s_GameplayFlags == GameplayFlags.Targeting)
                {
                    s_ActiveWorld.player.targetPoint = new Point(s_ActiveWorld.player.targetPoint.X, s_ActiveWorld.player.targetPoint.Y - 1);
                }
                else
                {
                    newPlayerPoint.Y -= 1;
                    hasMoved = true;
                }
            }
            //North east
            else if (m_Input.IsOldPress(Keys.NumPad9))
            {
                if (s_GameplayFlags == GameplayFlags.Targeting)
                {
                    s_ActiveWorld.player.targetPoint = new Point(s_ActiveWorld.player.targetPoint.X + 1, s_ActiveWorld.player.targetPoint.Y - 1);
                }
                else
                {
                    newPlayerPoint.X += 1;
                    newPlayerPoint.Y -= 1;
                    hasMoved = true;
                }
            }
            //East
            else if (m_Input.IsOldPress(Keys.NumPad6))
            {
                if (s_GameplayFlags == GameplayFlags.Targeting)
                {
                    s_ActiveWorld.player.targetPoint = new Point(s_ActiveWorld.player.targetPoint.X + 1, s_ActiveWorld.player.targetPoint.Y);
                }
                else
                {
                    newPlayerPoint.X += 1;
                    hasMoved = true;
                }
            }
            //South east
            else if (m_Input.IsOldPress(Keys.NumPad3))
            {
                if (s_GameplayFlags == GameplayFlags.Targeting)
                {
                    s_ActiveWorld.player.targetPoint = new Point(s_ActiveWorld.player.targetPoint.X + 1, s_ActiveWorld.player.targetPoint.Y + 1);
                }
                else
                {
                    newPlayerPoint.X += 1;
                    newPlayerPoint.Y += 1;
                    hasMoved = true;
                }
            }
            //South
            else if (m_Input.IsOldPress(Keys.NumPad2))
            {
                if (s_GameplayFlags == GameplayFlags.Targeting)
                {
                    s_ActiveWorld.player.targetPoint = new Point(s_ActiveWorld.player.targetPoint.X, s_ActiveWorld.player.targetPoint.Y + 1);
                }
                else
                {
                    newPlayerPoint.Y += 1;
                    hasMoved = true;
                }
            }
            //South west
            else if (m_Input.IsOldPress(Keys.NumPad1))
            {
                if (s_GameplayFlags == GameplayFlags.Targeting)
                {
                    s_ActiveWorld.player.targetPoint = new Point(s_ActiveWorld.player.targetPoint.X - 1, s_ActiveWorld.player.targetPoint.Y + 1);
                }
                else
                {
                    newPlayerPoint.X -= 1;
                    newPlayerPoint.Y += 1;
                    hasMoved = true;
                }
            }
            //West
            else if (m_Input.IsOldPress(Keys.NumPad4))
            {
                if (s_GameplayFlags == GameplayFlags.Targeting)
                {
                    s_ActiveWorld.player.targetPoint = new Point(s_ActiveWorld.player.targetPoint.X - 1, s_ActiveWorld.player.targetPoint.Y);
                }
                else
                {
                    newPlayerPoint.X -= 1;
                    hasMoved = true;
                }
            }
            //North west
            else if (m_Input.IsOldPress(Keys.NumPad7))
            {
                if (s_GameplayFlags == GameplayFlags.Targeting)
                {
                    s_ActiveWorld.player.targetPoint = new Point(s_ActiveWorld.player.targetPoint.X - 1, s_ActiveWorld.player.targetPoint.Y - 1);
                }
                else
                {
                    newPlayerPoint.X -= 1;
                    newPlayerPoint.Y -= 1;
                    hasMoved = true;
                }
            }
            else if (m_Input.IsOldPress(Keys.NumPad5))
            {
                Tick();
                return;
            }
            else if (s_ActiveWorld.player.fulfilmentCounter > 0 || autoTurn)
            { 
                if (m_TickTimer % TICK_TIMER == 0)
                {
                    Tick();
                    return;
                }
            }
            else if(m_Input.IsOldPress(Keys.Enter))
            {
                PhysicsResult physicsResult = PhysicsManager.IsCollision(s_ActiveWorld.player.position, newPlayerPoint, s_ActiveWorld);
                if (physicsResult == PhysicsResult.ObjectCollision)
                {
                    s_ActiveWorld.PickUpObject(s_ActiveWorld.player);
                }
            }

            if (hasMoved)
            {
                PhysicsResult physicsResult = PhysicsManager.IsCollision(s_ActiveWorld.player.position, newPlayerPoint, s_ActiveWorld);

                if (physicsResult == PhysicsResult.EntityCollision)
                {
                    Entity tempEntity = s_ActiveWorld.GetEntity(newPlayerPoint);
                    if (s_GameplayFlags == GameplayFlags.Interacting)
                    {
                        if(tempEntity.sentient)
                            TalkToPlayer(tempEntity);
                    }
                    else if (s_GameplayFlags == GameplayFlags.Giving)
                    {
                    }
                    else if (s_GameplayFlags == GameplayFlags.Moving)
                    {
                        playerWorld.SwapPosition(s_ActiveWorld.player, tempEntity);
                        Tick();
                    }
                    else if(s_GameplayFlags == GameplayFlags.Attacking)
                    {
                        if (tempEntity.GUID != s_ActiveWorld.player.GUID)
                        {
                            CombatEngine.PerformCombat(s_ActiveWorld.player, tempEntity);
                            tempEntity.InfluenceMe(s_ActiveWorld.player.GUID, -50);
                            if (!tempEntity.alive)
                            {
                                s_ActiveWorld.RemoveEntity(newPlayerPoint);
                            }
                        }
                    }
                    Tick();
                }
                else if (physicsResult == PhysicsResult.WallCollision)
                {

                }
                else
                {
                    s_ActiveWorld.player.Move(newPlayerPoint);
                    s_ActiveWorld.player.vision = s_ActiveWorld.GetVision(s_ActiveWorld.player);
                    Tick();
                }
            }
            else if (s_GameplayFlags == GameplayFlags.Targeting)
            {
                if (s_ActiveWorld.player.targetingAbility.targetType == AbilityTarget.Adjacent)
                {
                    if (AdjacencyHelper.IsAdjacent(s_ActiveWorld.player.position, s_ActiveWorld.player.targetPoint))
                    {
                        Entity tempEntity = s_ActiveWorld.GetEntity(s_ActiveWorld.player.targetPoint);
                        if (tempEntity != null && m_Input.IsOldPress(Keys.Enter))
                        {
                            s_ActiveWorld.player.targetingAbility.Use(s_ActiveWorld.player, tempEntity);
                            Tick();
                            s_GameplayFlags = GameplayFlags.Moving;
                        }
                    }
                }
                else if (s_ActiveWorld.player.targetingAbility.targetType == AbilityTarget.Ranged)
                {
                    Entity tempEntity = s_ActiveWorld.GetEntity(s_ActiveWorld.player.targetPoint);
                    if(tempEntity != null && m_Input.IsOldPress(Keys.Enter))
                    {
                        s_ActiveWorld.player.targetingAbility.Use(s_ActiveWorld.player, tempEntity);
                        Tick();
                        s_GameplayFlags = GameplayFlags.Moving;
                    }
                }
            }
            m_Camera.position = new Vector2(s_ActiveWorld.player.position.X * ObjectIcons.SPRITE_SIZE, s_ActiveWorld.player.position.Y * ObjectIcons.SPRITE_SIZE);
            */
        }

        public static void HandBack(Ability abilityRef)
        {
            /*
            s_GameplayFlags = GameplayFlags.Targeting;
            s_ActiveWorld.player.targetingAbility = abilityRef;
            s_ActiveWorld.player.targetPoint = s_ActiveWorld.player.position;
            */
        }

        protected void Tick()
        {
            s_ActiveWorld.Tick();
            s_ActiveWorld.Update();
            
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
        }

        public override void Draw()
        {
        }

        protected void DrawTargetCursor()
        {
            if (s_GameplayFlags != GameplayFlags.Targeting)
                return;
        }

        protected void DrawTiles()
        {
            lock(s_ActiveWorld.Tiles)
            {
            }
        }

        protected void DrawStairs()
        {
        }

        protected void DrawObjects()
        {
            Color obscured = LightLevelHelper.GetColour(1);

            lock(s_ActiveWorld.Objects)
            {
            }
        }

        protected void DrawEntities()
        {
            lock(s_ActiveWorld.Entities)
            {
            }
        }

        public override void Update()
        {
            base.Update();

            m_TickTimer = Environment.TickCount;
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
            return new MainMenuState();
        }

        public WorldInstance overworld
        {
            get
            {
                return s_Overworld;
            }
        }

        public WorldInstance playerWorld
        {
            get
            {
                return s_ActiveWorld;
            }
        }

        private int tickCounter
        {
            get;
            set;
        }

        private bool autoTurn
        {
            get;
            set;
        }

        protected bool expandConsole
        {
            get;
            set;
        }
    }
}