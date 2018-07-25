﻿using JoyLib.Code.Combat;
using JoyLib.Code.Conversation.Subengines;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Helpers;
using JoyLib.Code.IO;
using JoyLib.Code.Physics;
using JoyLib.Code.Quests;
using JoyLib.Code.States.Gameplay;
using JoyLib.Code.World;
using System;
using System.Collections.Generic;
using UnityEngine;

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

        public WorldState(WorldInstance overworldRef, WorldInstance activeWorldRef, GameplayFlags flagsRef) : base()
        {
            m_ActiveWorld = activeWorldRef;
            m_GameplayFlags = flagsRef;
            m_Overworld = overworldRef;
            SetUpUi();

            //MusicHandler.Play("DashRoog");

            m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
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
            m_ActiveWorld.Player.Vision = m_ActiveWorld.GetVision(m_ActiveWorld.Player);
            m_GameplayFlags = GameplayFlags.Moving;
            RumourMill.GenerateRumours(m_ActiveWorld);

            SetEntityWorld(overworld);
        }

        public override void Stop()
        {
            base.Stop();
            WorldSerialiser.Serialise(m_Overworld);
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
            m_ActiveWorld.RemoveEntity(player.WorldPosition);

            m_ActiveWorld = newWorld;
            m_ActiveWorld.AddEntity(player);
            player.MyWorld = m_ActiveWorld;

            player = m_ActiveWorld.Player;

            player.Move(spawnPoint);
            player.Vision = m_ActiveWorld.GetVision(player);

            m_GameplayFlags = GameplayFlags.Moving;
            RumourMill.GenerateRumours(m_ActiveWorld);

            QuestTracker.PerformExploration(player, newWorld);
            Tick();
        }

        public static void TalkToPlayer(Entity entityRef)
        {
        }

        protected void SetToAttack(object sender, EventArgs e)
        {
            m_GameplayFlags = GameplayFlags.Attacking;
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

            bool hasMoved = false;

            Entity player = m_ActiveWorld.Player;

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

            if (Input.GetKeyDown(KeyCode.Return))
            {
                //Going up a level
                if (m_ActiveWorld.Parent != null && player.WorldPosition == m_ActiveWorld.SpawnPoint && !player.HasMoved)
                {
                    ChangeWorld(m_ActiveWorld.Parent, m_ActiveWorld.Parent.TransitionPoint);
                }

                //Going down a level
                else if (m_ActiveWorld.Areas.ContainsKey(player.WorldPosition) && !player.HasMoved)
                { 
                    ChangeWorld(m_ActiveWorld.Areas[player.WorldPosition], m_ActiveWorld.Areas[player.WorldPosition].SpawnPoint);
                }
                return;
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
            else if(Input.GetKeyDown(KeyCode.Return))
            {
                PhysicsResult physicsResult = PhysicsManager.IsCollision(player.WorldPosition, newPlayerPoint, m_ActiveWorld);
                if (physicsResult == PhysicsResult.ObjectCollision)
                {
                    m_ActiveWorld.PickUpObject(player);
                }
            }

            if (hasMoved)
            {
                PhysicsResult physicsResult = PhysicsManager.IsCollision(player.WorldPosition, newPlayerPoint, m_ActiveWorld);

                if (physicsResult == PhysicsResult.EntityCollision)
                {
                    Entity tempEntity = m_ActiveWorld.GetEntity(newPlayerPoint);
                    if (m_GameplayFlags == GameplayFlags.Interacting)
                    {
                        if(tempEntity.Sentient)
                            TalkToPlayer(tempEntity);
                    }
                    else if (m_GameplayFlags == GameplayFlags.Giving)
                    {
                    }
                    else if (m_GameplayFlags == GameplayFlags.Moving)
                    {
                        playerWorld.SwapPosition(player, tempEntity);
                        Tick();
                    }
                    else if(m_GameplayFlags == GameplayFlags.Attacking)
                    {
                        if (tempEntity.GUID != player.GUID)
                        {
                            CombatEngine.PerformCombat(player, tempEntity);
                            tempEntity.InfluenceMe(player.GUID, -50);
                            if (!tempEntity.Alive)
                            {
                                m_ActiveWorld.RemoveEntity(newPlayerPoint);

                                //Find a way to remove the GameObject
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
                    player.Move(newPlayerPoint);
                    player.Vision = m_ActiveWorld.GetVision(player);
                    Tick();
                }
            }
            else if (m_GameplayFlags == GameplayFlags.Targeting)
            {
                if (player.TargetingAbility.targetType == AbilityTarget.Adjacent)
                {
                    if (AdjacencyHelper.IsAdjacent(player.WorldPosition, player.TargetPoint))
                    {
                        Entity tempEntity = m_ActiveWorld.GetEntity(player.TargetPoint);
                        if (tempEntity != null && Input.GetKeyDown(KeyCode.Return))
                        {
                            player.TargetingAbility.Use(player, tempEntity);
                            Tick();
                            m_GameplayFlags = GameplayFlags.Moving;
                        }
                    }
                }
                else if (player.TargetingAbility.targetType == AbilityTarget.Ranged)
                {
                    Entity tempEntity = m_ActiveWorld.GetEntity(player.TargetPoint);
                    if(tempEntity != null && Input.GetKeyDown(KeyCode.Return))
                    {
                        player.TargetingAbility.Use(player, tempEntity);
                        Tick();
                        m_GameplayFlags = GameplayFlags.Moving;
                    }
                }
            }

            if(autoTurn)
            {
                Tick();
            }
            m_Camera.transform.position = new Vector3(player.WorldPosition.x, player.WorldPosition.y, m_Camera.transform.position.z);
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
            m_ActiveWorld.Tick();
            m_ActiveWorld.Update();
            
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
            Color obscured = LightLevelHelper.GetColour(1);

            lock(m_ActiveWorld.Objects)
            {
            }
        }

        protected void DrawEntities()
        {
            lock(m_ActiveWorld.Entities)
            {
            }
        }

        public override void Update()
        {
            base.Update();
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
            return new WorldDestructionState(m_Overworld, m_ActiveWorld);
        }

        public WorldInstance overworld
        {
            get
            {
                return m_Overworld;
            }
        }

        public WorldInstance playerWorld
        {
            get
            {
                return m_ActiveWorld;
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