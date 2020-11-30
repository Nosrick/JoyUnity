using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using JoyLib.Code.World.Generators;
using JoyLib.Code.World.Generators.Interiors;
using JoyLib.Code.World.Generators.Overworld;
using System;
using System.Collections.Generic;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldCreationState : GameState
    {
        protected EntityPlayer m_Player;

        protected IWorldInstance m_World;
        protected IWorldInstance m_ActiveWorld;

        protected IWorldInfoHandler m_WorldInfoHandler = GlobalConstants.GameManager.WorldInfoHandler;

        protected const int SIMULATION_TICKS = 600;

        protected const int WORLD_SIZE = 20;

        public WorldCreationState(EntityPlayer playerRef) : base()
        {
            m_Player = playerRef;
        }

        public override void Start()
        {
            base.Start();
            SetUpUi();
            CreateWorld();
        }

        protected override void SetUpUi()
        {
            base.SetUpUi();
        }

        public override void OnGui()
        {
        }

        private void CreateWorld()
        {
            //Make a new overworld generator
            OverworldGenerator overworldGen = new OverworldGenerator();

            //Generate the basic overworld
            m_World = new WorldInstance(
                overworldGen.GenerateWorldSpace(WORLD_SIZE, "plains"), 
                new string[] { "overworld", "exterior" }, 
                "Everse",
                GlobalConstants.GameManager.EntityHandler,
                GlobalConstants.GameManager.Roller);

            //Set the date and time for 1/1/1555, 12:00pm
            m_World.SetDateTime(new DateTime(1555, 1, 1, 12, 0, 0));

            //Do the spawn point
            SpawnPointPlacer spawnPlacer = new SpawnPointPlacer(GlobalConstants.GameManager.Roller);
            Vector2Int spawnPoint = spawnPlacer.PlaceSpawnPoint(m_World);
            while((spawnPoint.x == -1 && spawnPoint.y == -1))
            {
                spawnPoint = spawnPlacer.PlaceSpawnPoint(m_World);
            }
            m_World.SpawnPoint = spawnPoint;

            //Set up the player
            //m_Player.Move(m_World.SpawnPoint);
            //m_World.AddEntity(m_Player);

            //Begin the first floor of the Naga Pits
            WorldInfo worldInfo = m_WorldInfoHandler.GetWorldInfo("naga pits")[0];

            DungeonGenerator dungeonGenerator = new DungeonGenerator();
            WorldInstance dungeon = dungeonGenerator.GenerateDungeon(
                worldInfo, 
                WORLD_SIZE, 
                3, 
                GlobalConstants.GameManager, 
                GlobalConstants.GameManager.Roller);

            Vector2Int transitionPoint = spawnPlacer.PlaceTransitionPoint(m_World);
            m_World.AddArea(transitionPoint, dungeon);
            Done = true;

            m_ActiveWorld = dungeon;
            m_Player.Move(dungeon.SpawnPoint);
            dungeon.AddEntity(m_Player);

            GlobalConstants.GameManager.EntityHandler.AddEntity(m_Player);
            
            IItemInstance lightSource = GlobalConstants.GameManager.ItemFactory.CreateRandomItemOfType(new string[] { "light source" });
            IJoyAction addItemAction = m_Player.FetchAction("additemaction");
            addItemAction.Execute(
                new IJoyObject[] {m_Player, lightSource},
                new []{"pickup"},
                new object[] { true });

            for (int i = 0; i < 4; i++)
            {
                addItemAction.Execute(
                    new IJoyObject[] {m_Player, GlobalConstants.GameManager.ItemFactory.CreateCompletelyRandomItem(true)},
                    new []{"pickup"},
                    new object[] { true });
            }
            
            m_World.Tick();
        }

        protected void SimulateWorld()
        {
            List<IWorldInstance> worlds = m_World.GetWorlds(m_World);
            for (int a = 0; a < worlds.Count; a++)
            {
                for (int i = 0; i < SIMULATION_TICKS; i++)
                {
                    worlds[a].Tick();
                }
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override GameState GetNextState()
        {
            //return new WorldInitialisationState(m_World, m_World);
            return new WorldInitialisationState(m_World, m_ActiveWorld);
        }
    }
}