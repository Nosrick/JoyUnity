using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.World.Generators.Interiors
{
    public delegate GeneratorTileType[,] PlaceRooms();

    public class DungeonRoomGenerator
    {
        protected const int LOOP_BREAK = 50;

        protected const int MIN_ROOM_SIZE = 5;
        protected const int MAX_ROOM_SIZE = 11;
        protected const int MIN_MAP_SIZE = 19;

        protected readonly int m_Size;
        protected readonly GeneratorTileType[,] m_Tiles;

        protected readonly int m_NumberOfRooms;
        protected int m_NumberRoomsPlaced;

        protected readonly List<Rect> m_Rooms;
        
        protected RNG Roller { get; set; }

        public DungeonRoomGenerator(int size, RNG roller)
        {
            m_Size = size;
            Roller = roller;

            if (m_Size < MIN_MAP_SIZE)
            {
                m_Size = MIN_MAP_SIZE;
            }

            m_Tiles = new GeneratorTileType[m_Size, m_Size];
            m_NumberOfRooms = CalculateRooms();
            m_NumberRoomsPlaced = 0;

            m_Rooms = new List<Rect>(m_NumberOfRooms);
        }

        public GeneratorTileType[,] GenerateRooms()
        {
            int loopCounter = 0;

            while (m_NumberRoomsPlaced <= m_NumberOfRooms && loopCounter < LOOP_BREAK)
            {
                Vector2Int topLeft = new Vector2Int();
                topLeft.x = Roller.Roll(1, m_Size);
                if (topLeft.x % 2 == 1)
                {
                    topLeft.x += 1;
                }

                topLeft.y = Roller.Roll(1, m_Size);
                if (topLeft.y % 2 == 1)
                {
                    topLeft.y += 1;
                }

                if (PlaceRoom(topLeft))
                    OpenRoom(m_Rooms[m_Rooms.Count - 1]);

                loopCounter += 1;
            }

            return m_Tiles;
        }

        private int CalculateRooms()
        {
            int dungeonArea = m_Size * m_Size;
            const int roomArea = MAX_ROOM_SIZE * MAX_ROOM_SIZE;

            return (dungeonArea / roomArea);
        }

        private bool PlaceRoom(Vector2Int topLeft)
        {
            int loopCounter = 0;
            while (loopCounter < LOOP_BREAK)
            {
                loopCounter += 1;

                if (topLeft.x < 1 || topLeft.y < 1 || topLeft.x > m_Size - 1 || topLeft.y > m_Size - 1)
                    return false;

                if (m_Tiles[topLeft.x, topLeft.y] == GeneratorTileType.Floor)
                    return false;

                if (m_NumberOfRooms == m_NumberRoomsPlaced)
                    return false;

                Vector2Int bottomRight = new Vector2Int(topLeft.x + Roller.Roll(MIN_ROOM_SIZE, MAX_ROOM_SIZE),
                                                topLeft.y + Roller.Roll(MIN_ROOM_SIZE, MAX_ROOM_SIZE));
                
                bottomRight.Clamp(Vector2Int.one, new Vector2Int(m_Size - 1, m_Size - 1));

                if (!ValidateRoom(topLeft, bottomRight))
                    return false;

                if (CheckForRoom(topLeft, bottomRight))
                    return false;

                for (int i = topLeft.x; i <= bottomRight.x; i++)
                {
                    for (int j = topLeft.y; j <= bottomRight.y; j++)
                    {
                        if (i == topLeft.x || i == bottomRight.x || j == topLeft.y || j == bottomRight.y)
                        {
                            m_Tiles[i, j] = GeneratorTileType.Perimeter;
                        }
                        else
                        {
                            m_Tiles[i, j] = GeneratorTileType.Floor;
                        }
                    }
                }

                m_Rooms.Add(new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x,
                                          bottomRight.y - topLeft.y));
                m_NumberRoomsPlaced += 1;
                return true;
            }
            return false;
        }

        private void OpenRoom(Rect room)
        {
            int doors = CalculateDoors(room);

            List<Vector2Int> validDoors = new List<Vector2Int>();
            for (int i = (int)room.xMin; i < (int)room.xMax + 1; i++)
            {
                for (int j = (int)room.yMin; j < (int)room.yMax + 1; j++)
                {
                    if (m_Tiles[i, j] != GeneratorTileType.Perimeter)
                        continue;

                    if (((i != (int)room.xMin && i != (int)room.xMax) || (j != (int)room.yMin && j != (int)room.yMax)) &&
                        i > 1 && i < m_Size - 1 && j > 1 && j < m_Size - 1)
                    {
                        validDoors.Add(new Vector2Int(i, j));
                    }
                }
            }

            for (int i = 0; i < doors; i++)
            {
                int index = Roller.Roll(0, validDoors.Count);

                Vector2Int point = new Vector2Int(validDoors[index].x, validDoors[index].y);

                if (m_Tiles[point.x - 1, point.y] == GeneratorTileType.Entrance)
                    continue;

                if (m_Tiles[point.x + 1, point.y] == GeneratorTileType.Entrance)
                    continue;

                if (m_Tiles[point.x, point.y - 1] == GeneratorTileType.Entrance)
                    continue;

                if (m_Tiles[point.x, point.y + 1] == GeneratorTileType.Entrance)
                    continue;

                if (m_Tiles[point.x, point.y] == GeneratorTileType.Perimeter)
                {
                    m_Tiles[point.x, point.y] = GeneratorTileType.Entrance;
                }
                else
                {
                    i -= 1;
                }
            }
        }

        private int CalculateDoors(Rect room)
        {
            int roomArea = (int)(room.width * room.height);
            int maxDoors = (int)Math.Sqrt(roomArea);

            return Roller.Roll(1, maxDoors);
        }

        private bool CheckForRoom(Vector2Int topLeft, Vector2Int bottomRight)
        {
            for (int i = topLeft.x; i <= bottomRight.x; i++)
            {
                for (int j = topLeft.y; j <= bottomRight.y; j++)
                {
                    if (m_Tiles[i, j] != GeneratorTileType.None)
                        return true;
                }
            }

            return false;
        }

        private bool ValidateRoom(Vector2Int topLeft, Vector2Int bottomRight)
        {
            if (bottomRight.x >= m_Size)
                return false;

            if (topLeft.x < 1)
                return false;

            if (bottomRight.y >= m_Size)
                return false;

            if (topLeft.y < 1)
                return false;

            return true;
        }

        public List<Rect> rooms
        {
            get
            {
                return m_Rooms;
            }
        }

        public Vector2Int PlaceEndPoint()
        {
            List<Vector2Int> validPoints = new List<Vector2Int>();
            for (int i = 0; i < m_Rooms.Count; i++)
            {
                for (int j = (int)m_Rooms[i].xMin; j < (int)m_Rooms[i].xMax + 1; j++)
                {
                    for (int k = (int)m_Rooms[i].yMin; k < (int)m_Rooms[i].yMax + 1; k++)
                    {
                        if (m_Tiles[j, k] == GeneratorTileType.Floor)
                        {
                            validPoints.Add(new Vector2Int(j, k));
                        }
                    }
                }
            }

            if (validPoints.Count == 0)
                return new Vector2Int(m_Tiles.GetLength(0) / 2, m_Tiles.GetLength(1) / 2);

            int index = Roller.Roll(0, validPoints.Count - 1);
            return validPoints[index];
        }
    }
}
