using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public class DungeonCorridorGenerator
    {
        protected const int LOOP_BREAK = 50;

        protected GeneratorTileType[,] m_Tiles;
        protected List<InteriorRoom> m_Rooms;
        protected readonly int m_CorridorBend;
        
        protected RNG Roller { get; set; }

        protected const int PASSES = 3;

        public DungeonCorridorGenerator(GeneratorTileType[,] tilesRef, List<Rect> roomsRef, int bendRef, RNG roller)
        {
            Roller = roller;
            m_Tiles = tilesRef;
            m_Rooms = new List<InteriorRoom>();

            for (int i = 0; i < roomsRef.Count; i++)
            {
                m_Rooms.Add(new InteriorRoom(roomsRef[i], new List<InteriorRoom>()));
            }

            m_CorridorBend = bendRef;
        }

        public GeneratorTileType[,] GenerateCorridors()
        {
            for (int a = 0; a < PASSES; a++)
            {
                for (int i = 0; i < m_Rooms.Count; i++)
                {
                    for (int j = (int)m_Rooms[i].sizes.xMin; j < (int)m_Rooms[i].sizes.xMax; j++)
                    {
                        for (int k = (int)m_Rooms[i].sizes.yMin; k < (int)m_Rooms[i].sizes.yMax; k++)
                        {
                            if (HasFlag(m_Tiles[j, k], GeneratorTileType.Wall) ||
                                HasFlag(m_Tiles[j, k], GeneratorTileType.Perimeter) ||
                                HasFlag(m_Tiles[j, k], GeneratorTileType.Entrance))
                                continue;

                            Tunnel(new Vector2Int(j, k), (FacingDirection)(Roller.Roll(0, 4) * 2));
                        }
                    }
                }
            }

            ClipDeadEnds();

            return m_Tiles;
        }

        private void Tunnel(Vector2Int point, FacingDirection lastDirection)
        {
            while (true)
            {
                Vector2Int newPoint = point;
                FacingDirection newDirection = ChooseTunnelDirection(lastDirection);

                for (int i = 0; i < 2; i++)
                {
                    switch (newDirection)
                    {
                        case FacingDirection.North:
                            newPoint.y -= 1;
                            break;

                        case FacingDirection.East:
                            newPoint.x += 1;
                            break;

                        case FacingDirection.South:
                            newPoint.y += 1;
                            break;

                        case FacingDirection.West:
                            newPoint.x -= 1;
                            break;
                    }

                    if (!ValidateTunnel(newPoint))
                        return;

                    m_Tiles[newPoint.x, newPoint.y] = GeneratorTileType.Corridor;
                }

                point = newPoint;
                lastDirection = newDirection;
            }
        }

        public void ClipDeadEnds()
        {
            while (true)
            {
                List<Vector2Int> deadEnds = new List<Vector2Int>();

                for (int i = 1; i < m_Tiles.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j < m_Tiles.GetLength(1) - 1; j++)
                    {
                        if (m_Tiles[i, j] != GeneratorTileType.Corridor)
                            continue;

                        int neighbours = 0;
                        //North
                        if (m_Tiles[i, j - 1] == GeneratorTileType.Corridor)
                        {
                            neighbours += 1;
                        }
                        //East
                        if (m_Tiles[i + 1, j] == GeneratorTileType.Corridor)
                        {
                            neighbours += 1;
                        }
                        //South
                        if (m_Tiles[i, j + 1] == GeneratorTileType.Corridor)
                        {
                            neighbours += 1;
                        }
                        //West
                        if (m_Tiles[i - 1, j] == GeneratorTileType.Corridor)
                        {
                            neighbours += 1;
                        }

                        if (neighbours <= 1)
                            deadEnds.Add(new Vector2Int(i, j));
                    }
                }

                for (int i = 0; i < deadEnds.Count; i++)
                {
                    m_Tiles[deadEnds[i].x, deadEnds[i].y] = GeneratorTileType.Wall;
                }

                if (deadEnds.Count == 0)
                    return;
            }
        }

        private bool ValidateTunnel(Vector2Int point)
        {
            if (point.x < 1 || point.x > m_Tiles.GetLength(0) - 2)
                return false;

            if (point.y < 1 || point.y > m_Tiles.GetLength(1) - 2)
                return false;

            if (HasFlag(m_Tiles[point.x, point.y], GeneratorTileType.Corridor))
                return false;

            return true;
        }

        private FacingDirection ChooseTunnelDirection(FacingDirection lastDirection)
        {
            int roll = Roller.Roll(0, 100);

            if (roll >= m_CorridorBend)
                return lastDirection;

            roll = Roller.Roll(0, 4) * 2;
            while (roll == (int)lastDirection)
            {
                roll = Roller.Roll(0, 4) * 2;
            }
            return (FacingDirection)roll;
        }

        private bool HasFlag(GeneratorTileType left, GeneratorTileType right)
        {
            return ((left & right) == right);
        }
    }
}