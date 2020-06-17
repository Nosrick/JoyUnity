using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using JoyLib.Code.World.Generators;
using JoyLib.Code.World.Generators.Interiors;
using JoyLib.Code.World.Generators.Overworld;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldCreationState : GameState
    {
        private Entity m_Player;

        private WorldInstance m_World;

        protected const int SIMULATION_TICKS = 600;

        protected const int WORLD_SIZE = 20;

        public WorldCreationState(Entity playerRef) : base()
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
            m_World = new WorldInstance(overworldGen.GenerateWorldSpace(WORLD_SIZE, "plains"), new string[] { "overworld", "exterior" }, "Everse");

            //Set the date and time for 1/1/1555, 12:00pm
            m_World.SetDateTime(new DateTime(1555, 1, 1, 12, 0, 0));

            //Do the spawn point
            SpawnPointPlacer spawnPlacer = new SpawnPointPlacer();
            Vector2Int spawnPoint = spawnPlacer.PlaceSpawnPoint(m_World);
            while((spawnPoint.x == -1 && spawnPoint.y == -1))
            {
                spawnPoint = spawnPlacer.PlaceSpawnPoint(m_World);
            }
            m_World.SpawnPoint = spawnPoint;

            //Set up the player
            m_Player.Move(m_World.SpawnPoint);
            m_Player.MyWorld = m_World;
            m_World.AddEntity(m_Player);

            //Begin the first floor of the Naga Pits
            WorldInfo worldInfo = WorldInfoHandler.instance.GetWorldInfo("naga pits")[0];

            WorldInstance dungeon = DungeonGenerator.GenerateDungeon(worldInfo, WORLD_SIZE, 3);

            Vector2Int transitionPoint = spawnPlacer.PlaceTransitionPoint(m_World);
            m_World.AddArea(transitionPoint, dungeon);
            Done = true;

            ItemInstance lightSource = WorldState.ItemHandler.CreateRandomItemOfType(new string[] { "light source" });
            m_Player.AddContents(new ItemInstance(lightSource));
            m_World.Tick();
        }

        protected void SimulateWorld()
        {
            List<WorldInstance> worlds = m_World.GetWorlds(m_World);
            for (int a = 0; a < worlds.Count; a++)
            {
                for (int i = 0; i < SIMULATION_TICKS; i++)
                {
                    worlds[a].Update();
                }
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override GameState GetNextState()
        {
            return new WorldInitialisationState(m_World, m_World);
        }
    }
}