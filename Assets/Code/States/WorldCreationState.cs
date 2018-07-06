using JoyLib.Code.Entities;
using JoyLib.Code.States.Gameplay;
using JoyLib.Code.World;
using JoyLib.Code.World.Generators;
using JoyLib.Code.World.Generators.Interiors;
using JoyLib.Code.World.Generators.Overworld;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class WorldCreationState : GameState
    {
        private Entity m_Player;

        private WorldInstance m_World;

        protected const int SIMULATION_TICKS = 600;

        protected bool m_Finished = false;

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
            OverworldGenerator overworldGen = new OverworldGenerator();
            m_World = new WorldInstance(overworldGen.GenerateWorldSpace(50), new Dictionary<string, WorldInstance>(), new List<Entity>(),
                new List<JoyObject>(), WorldType.Overworld, "Everse");
            m_World.SetDateTime(new DateTime(1555, 1, 1, 12, 0, 0));

            SpawnPointPlacer spawnPlacer = new SpawnPointPlacer();
            Vector2Int transition = spawnPlacer.PlaceTransitionPoint(m_World);
            while((transition.x == -1 && transition.y == -1))
            {
                transition = spawnPlacer.PlaceSpawnPoint(m_World);
            }
            m_World.SpawnPoint = transition;
            m_Player.Move(m_World.SpawnPoint);
            m_Player.MyWorld = m_World;
            m_World.AddEntity(m_Player);

            List<string> dungeonTypes = new List<string>();
            dungeonTypes.Add("Naga");
            dungeonTypes.Add("Slime");
            
            DungeonInteriorGenerator dunGen = new DungeonInteriorGenerator();
            WorldInstance firstFloor = new WorldInstance(dunGen.GenerateWorldSpace(50), new Dictionary<string, WorldInstance>(),
                new List<Entity>(), dunGen.GenerateWalls(), WorldType.Interior, "Naga Pits 1");
            firstFloor.SpawnPoint = spawnPlacer.PlaceSpawnPoint(firstFloor);
            firstFloor.Entities = DungeonEntityPlacer.PlaceEntities(firstFloor, dungeonTypes);

            transition = spawnPlacer.PlaceTransitionPoint(firstFloor);
            while ((transition.x == -1 && transition.y == -1) || transition == m_World.SpawnPoint)
            {
                transition = spawnPlacer.PlaceTransitionPoint(firstFloor);
            }

            firstFloor.Objects.AddRange(DungeonItemPlacer.PlaceItems(firstFloor));
            m_World.AddArea(transition.ToString(), firstFloor);
            
            WorldInstance secondFloor = new WorldInstance(dunGen.GenerateWorldSpace(50), new Dictionary<string, WorldInstance>(),
                new List<Entity>(), dunGen.GenerateWalls(), WorldType.Interior, "Naga Pits 2");
            secondFloor.SpawnPoint = spawnPlacer.PlaceSpawnPoint(secondFloor);

            transition = spawnPlacer.PlaceTransitionPoint(secondFloor);
            while ((transition.x == -1 && transition.y == -1) || transition == secondFloor.SpawnPoint)
            {
                transition = spawnPlacer.PlaceTransitionPoint(secondFloor);
            }

            firstFloor.AddArea(transition.ToString(), secondFloor);
            
            WorldInstance thirdFloor = new WorldInstance(dunGen.GenerateWorldSpace(50), new Dictionary<string, WorldInstance>(),
                new List<Entity>(), dunGen.GenerateWalls(), WorldType.Interior, "Naga Pits 3");
            thirdFloor.SpawnPoint = spawnPlacer.PlaceSpawnPoint(thirdFloor);
            secondFloor.AddArea(transition.ToString(), thirdFloor);

            //SimulateWorld();
            m_Finished = true;
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

            m_Finished = true;
        }

        public override void Update()
        {
            base.Update();

            if(m_Finished)
            {
                Done = true;
            }
        }

        public override GameState GetNextState()
        {
            return new WorldState(m_World, m_World, GameplayFlags.Moving);
        }
    }
}