﻿using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using JoyLib.Code.Managers;
using JoyLib.Code.Rollers;
using JoyLib.Code.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.World
{
    [Serializable]
    public class WorldInstance : IWorldInstance
    {
        protected WorldTile[,] m_Tiles;
        protected byte[,] m_Costs;
        protected int[,] m_Light;

        [NonSerialized] protected int m_PlayerIndex;

        protected readonly Vector2Int m_Dimensions;

        //Worlds and where to access them
        protected Dictionary<Vector2Int, IWorldInstance> m_Areas;

        [NonSerialized] protected IWorldInstance m_Parent;

        protected List<IEntity> m_Entities;

        protected List<IJoyObject> m_Objects;
        protected Dictionary<Vector2Int, IJoyObject> m_Walls;

        protected Vector2Int m_SpawnPoint;

        protected static DateTime s_DateTime;

        protected string m_Name;
        protected long m_GUID;

        [NonSerialized] protected GameObject m_FogOfWarHolder;
        [NonSerialized] protected GameObject m_WallHolder;
        [NonSerialized] protected GameObject m_ObjectHolder;
        [NonSerialized] protected GameObject m_EntityHolder;

        [NonSerialized] protected ILiveEntityHandler EntityHandler;
        [NonSerialized] protected RNG Roller;

        /// <summary>
        /// A template for adding stuff to later. A blank WorldInstance.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="tags"></param>
        /// <param name="name"></param>
        public WorldInstance(WorldTile[,] tiles, 
            string[] tags, 
            string name, 
            ILiveEntityHandler entityHandler,
            RNG roller)
        {
            this.EntityHandler = entityHandler;
            this.Roller = roller;
            
            m_Dimensions = new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));

            this.Name = name;
            this.Tags = new List<string>(tags);
            m_Tiles = tiles;
            m_Areas = new Dictionary<Vector2Int, IWorldInstance>();
            m_Entities = new List<IEntity>();
            m_Objects = new List<IJoyObject>();
            m_Walls = new Dictionary<Vector2Int, IJoyObject>();
            GUID = GUIDManager.Instance.AssignGUID();
            m_Light = new int[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];

            m_Costs = new byte[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];
            for (int x = 0; x < m_Costs.GetLength(0); x++)
            {
                for (int y = 0; y < m_Costs.GetLength(1); y++)
                {
                    m_Costs[x, y] = 1;
                }
            }

            m_FogOfWarHolder = GameObject.Find("WorldFog");
            m_WallHolder = GameObject.Find("WorldWalls");
            m_ObjectHolder = GameObject.Find("WorldObjects");
            m_EntityHolder = GameObject.Find("WorldEntities");
        }

        /// <summary>
        /// For creating a well-established WorldInstance
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="areas"></param>
        /// <param name="entities"></param>
        /// <param name="objects"></param>
        /// <param name="tags"></param>
        /// <param name="name"></param>
        public WorldInstance(WorldTile[,] tiles, Dictionary<Vector2Int, IWorldInstance> areas, List<IEntity> entities,
            List<IJoyObject> objects, Dictionary<Vector2Int, IJoyObject> walls, string[] tags, string name)
        {
            this.Name = name;
            this.Tags = new List<string>(tags);
            m_Tiles = tiles;
            m_Areas = areas;
            m_Entities = entities;
            m_Objects = objects;
            m_Walls = walls;
            GUID = GUIDManager.Instance.AssignGUID();
            CalculatePlayerIndex();
            m_Light = new int[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];

            m_FogOfWarHolder = GameObject.Find("WorldFog");
            m_WallHolder = GameObject.Find("WorldWalls");
            m_ObjectHolder = GameObject.Find("WorldObjects");
            m_EntityHolder = GameObject.Find("WorldEntities");

            m_Costs = new byte[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];
            for (int x = 0; x < m_Costs.GetLength(0); x++)
            {
                for (int y = 0; y < m_Costs.GetLength(1); y++)
                {
                    m_Costs[x, y] = 1;
                }
            }

            foreach (Vector2Int position in m_Walls.Keys)
            {
                m_Costs[position.x, position.y] = byte.MaxValue;
            }
        }

        public void SetDateTime(DateTime dateTime)
        {
            s_DateTime = dateTime;
        }

        protected void CalculatePlayerIndex()
        {
            m_PlayerIndex = m_Entities.FindIndex(entity => entity.PlayerControlled);
            if (m_PlayerIndex > 0 && m_PlayerIndex < m_Entities.Count)
            {
                EntityHandler.SetPlayer(m_Entities[m_PlayerIndex]);
            }
        }

        protected void CalculateLightLevels()
        {
            m_Light = new int[m_Light.GetLength(0), m_Light.GetLength(1)];

            //Do objects first
            List<IJoyObject> objects = new List<IJoyObject>(m_Objects);

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is ItemInstance == false)
                {
                    continue;
                }

                ItemInstance item = (ItemInstance) objects[i];

                int xMin, xMax;
                int yMin, yMax;

                xMin = Math.Max(0, item.WorldPosition.x - (item.ItemType.LightLevel));
                xMax = Math.Min(m_Light.GetLength(0) - 1, item.WorldPosition.x + (item.ItemType.LightLevel));

                yMin = Math.Max(0, item.WorldPosition.y - (item.ItemType.LightLevel));
                yMax = Math.Min(m_Light.GetLength(1) - 1, item.WorldPosition.y + (item.ItemType.LightLevel));

                if (item.ItemType.LightLevel > 0)
                {
                    for (int x = xMin; x < xMax; x++)
                    {
                        for (int y = yMin; y < yMax; y++)
                        {
                            m_Light = LightTiles(new Vector2Int(x, y), item, m_Light);
                        }
                    }
                }
            }

            //Then the objects used by entities
            List<IEntity> entities = m_Entities.ToList();

            for (int i = 0; i < entities.Count; i++)
            {
                IEntity entity = entities[i];
                List<IItemInstance> backpack = entity.Backpack;
                for (int j = 0; j < backpack.Count; j++)
                {
                    IItemInstance item = backpack[j];
                    item.Move(entity.WorldPosition);

                    int xMin, xMax;
                    int yMin, yMax;

                    xMin = Math.Max(0, entity.WorldPosition.x - (item.ItemType.LightLevel));
                    xMax = Math.Min(m_Light.GetLength(0) - 1, entity.WorldPosition.x + (item.ItemType.LightLevel));

                    yMin = Math.Max(0, entity.WorldPosition.y - (item.ItemType.LightLevel));
                    yMax = Math.Min(m_Light.GetLength(1) - 1, entity.WorldPosition.y + (item.ItemType.LightLevel));

                    if (item.ItemType.LightLevel > 0)
                    {
                        for (int x = xMin; x < xMax; x++)
                        {
                            for (int y = yMin; y < yMax; y++)
                            {
                                m_Light = LightTiles(new Vector2Int(x, y), item, m_Light);
                            }
                        }
                    }
                }
            }
        }

        protected int[,] LightTiles(Vector2Int pointRef, IItemInstance objectRef, int[,] lightRef)
        {
            int[,] vision = lightRef;

            int lightLevel = objectRef.ItemType.LightLevel;
            Rect lightRect = new Rect(objectRef.WorldPosition.x - lightLevel, objectRef.WorldPosition.y - lightLevel,
                lightLevel * 2, lightLevel * 2);

            for (int i = 0; i < 360; i++)
            {
                float x = (float) Math.Cos(i * 0.01745f);
                float y = (float) Math.Sin(i * 0.01745f);
                vision = LightTile(x, y, objectRef, vision);
            }

            return vision;
        }

        protected int[,] LightTile(float x, float y, IItemInstance objectRef, int[,] visionRef)
        {
            int[,] vision = visionRef;
            float oX, oY;

            oX = objectRef.WorldPosition.x + 0.5f;
            oY = objectRef.WorldPosition.y + 0.5f;

            int itemPosX = objectRef.WorldPosition.x;
            int itemPosY = objectRef.WorldPosition.y;

            for (int i = 0; i < objectRef.ItemType.LightLevel; i++)
            {
                int posX = (int) oX;
                int posY = (int) oY;
                if (oX < 0.0f || oY < 0.0f || oX >= vision.GetLength(0) || oY >= vision.GetLength(1))
                {
                    return vision;
                }

                /*
                if (Walls.ContainsKey(new Vector2Int(posX, posY)))
                {
                    return vision;
                }
                */

                int lightLevel = (int) Math.Max(objectRef.ItemType.LightLevel,
                    Math.Sqrt(((itemPosX - posX) * (itemPosX - posX) + (itemPosY - posY) * (itemPosY - posY))));
                vision[posX, posY] = lightLevel;

                oX += x;
                oY += y;
            }

            return vision;
        }

        public void AddObject(IJoyObject objectRef)
        {
            if (objectRef.IsWall)
            {
                AddWall(objectRef);
            }
            else
            {
                m_Objects.Add(objectRef);
            }

            IsDirty = true;
        }

        protected void AddWall(IJoyObject wallRef)
        {
            m_Walls.Add(wallRef.WorldPosition, wallRef);
            m_Costs[wallRef.WorldPosition.x, wallRef.WorldPosition.y] = byte.MaxValue;
        }

        public bool RemoveObject(Vector2Int positionRef, IItemInstance itemRef)
        {
            bool removed = false;

            if (m_Objects.Any(o => o.WorldPosition.Equals(positionRef) && itemRef.GUID.Equals(o.GUID)) == false)
            {
                return false;
            }

            IJoyObject seek =
                m_Objects.First(obj => obj.WorldPosition.Equals(positionRef) && itemRef.GUID.Equals(obj.GUID));
            removed = m_Objects.Remove(seek);

            if (removed)
            {
                IsDirty = true;

                seek.MyWorld = null;

                itemRef.Move(new Vector2Int(-1, -1));
            }

            return removed;
        }

        public IJoyObject GetObject(Vector2Int WorldPosition)
        {
            for (int i = 0; i < m_Objects.Count; i++)
            {
                if (m_Objects[i].WorldPosition == WorldPosition)
                {
                    return m_Objects[i];
                }
            }

            return null;
        }

        public void Tick()
        {
            DateTime oldTime = s_DateTime;
            if (HasTag("overworld"))
            {
                s_DateTime = s_DateTime.AddSeconds(6.0);
            }
            else
            {
                s_DateTime = s_DateTime.AddHours(1.0);
            }

            foreach (Entity entity in m_Entities)
            {
                entity.Tick();
            }

            IsDirty = false;
        }

        /// <summary>
        /// Searches for any objects that match the tags specified, which the entity can see.
        /// </summary>
        /// <param name="entityRef"></param>
        /// <param name="tags"></param>
        /// <param name="objectType"></param>
        /// <param name="intentRef"></param>
        /// <returns></returns>
        public IEnumerable<IJoyObject> SearchForObjects(IEntity entityRef, IEnumerable<string> tags)
        {
            List<IJoyObject> data = new List<IJoyObject>();

            if (entityRef.VisionProvider.Vision.GetLength(0) == 1)
            {
                return data;
            }

            List<IJoyObject> inSight = m_Objects
                .Where(obj => entityRef.VisionProvider.CanSee(entityRef, this, obj.WorldPosition) == true).ToList();
            foreach (IJoyObject obj in inSight)
            {
                IEnumerable<string> intersect = obj.Tags.Intersect(tags);
                if (tags.Any() == false || intersect.SequenceEqual(tags))
                {
                    data.Add(obj);
                }
            }

            return data;
        }

        public IEnumerable<IEntity> SearchForEntities(IEntity actor, IEnumerable<string> searchCriteria)
        {
            List<IEntity> searchEntities = new List<IEntity>();

            foreach (Entity entity in m_Entities)
            {
                if (actor.GUID == entity.GUID
                    || !actor.VisionProvider.CanSee(actor, this, entity.WorldPosition))
                {
                    continue;
                }

                IEnumerable<Tuple<string, int>> data = entity.GetData(searchCriteria.ToArray());
                IEnumerable<string> tags = data.Select(x => x.Item1);
                if (tags.SequenceEqual(searchCriteria))
                {
                    searchEntities.Add(entity);
                }
            }

            return searchEntities;
        }

        public IEntity GetRandomSentient()
        {
            if (m_Entities.Count == 0)
            {
                return null;
            }

            List<IEntity> sentients = m_Entities.Where(x => x.Sentient).ToList();

            if (!(this.Player is null))
            {
                sentients = sentients.Where(entity => entity.GUID.Equals(Player.GUID) == false).ToList();
            }

            return sentients.Count > 0 ? sentients[Roller.Roll(0, sentients.Count)] : null;
        }

        public IEntity GetRandomSentientWorldWide()
        {
            List<IWorldInstance> worlds = GetWorlds(GetOverworld());
            int result = Roller.Roll(0, worlds.Count);
            IEntity entity = worlds[result].GetRandomSentient();

            int breakout = 0;
            while (entity == null && breakout < 100)
            {
                result = Roller.Roll(0, worlds.Count);
                entity = worlds[result].GetRandomSentient();
                breakout++;
            }

            return entity;
        }

        public List<IWorldInstance> GetWorlds(IWorldInstance parent)
        {
            List<IWorldInstance> worlds = new List<IWorldInstance>();
            worlds.Add(parent);
            for (int i = 0; i < worlds.Count; i++)
            {
                foreach (IWorldInstance world in worlds[i].Areas.Values)
                {
                    List<IWorldInstance> newWorlds = GetWorlds(world);
                    for (int j = 0; j < newWorlds.Count; j++)
                    {
                        if (!worlds.Contains(newWorlds[j]))
                            worlds.Add(newWorlds[j]);
                    }
                }
            }

            return worlds;
        }

        public Vector2Int GetTransitionPointForParent()
        {
            foreach (KeyValuePair<Vector2Int, IWorldInstance> pair in Parent.Areas)
            {
                if (pair.Value.GUID == this.GUID)
                {
                    return pair.Key;
                }
            }

            return new Vector2Int(-1, -1);
        }

        public IWorldInstance GetOverworld()
        {
            if (Parent == null)
            {
                return this;
            }
            else
            {
                return Parent.GetOverworld();
            }
        }

        public IWorldInstance GetPlayerWorld(IWorldInstance parent)
        {
            if (parent.Entities.Any(x => x.PlayerControlled))
                return parent;

            foreach (IWorldInstance world in parent.Areas.Values)
            {
                return GetPlayerWorld(world);
            }

            return null;
        }

        public void SwapPosition(IEntity left, IEntity right)
        {
            Vector2Int tempPosition = right.WorldPosition;
            right.Move(left.WorldPosition);
            left.Move(tempPosition);
        }

        public IItemInstance PickUpObject(IEntity entityRef)
        {
            if (GetObject(entityRef.WorldPosition) is IItemInstance item)
            {
                List<string> tags = new List<string> {"pick up"};
                bool newOwner = true;
                if (item.OwnerGUID != default && item.OwnerGUID != entityRef.GUID)
                {
                    tags.Add("theft");
                    newOwner = false;
                }

                entityRef.FetchAction("additemaction")
                    .Execute(new IJoyObject[] {entityRef, item},
                        tags.ToArray(),
                        new object[] {newOwner});

                m_Objects.Remove(item);

                return item;
            }

            return null;
        }

        public void AddEntity(IEntity entityRef)
        {
            m_Entities.Add(entityRef);
            EntityHandler.AddEntity(entityRef);

            //Initialise a new GameObject here at some point

            CalculatePlayerIndex();
            IsDirty = true;

            entityRef.MyWorld = this;
        }

        public void RemoveEntity(Vector2Int positionRef)
        {
            long entityGUID = -1;
            for (int i = 0; i < m_Entities.Count; i++)
            {
                if (m_Entities[i].WorldPosition.Equals(positionRef))
                {
                    entityGUID = m_Entities[i].GUID;
                    m_Entities.RemoveAt(i);
                    break;
                }
            }

            CalculatePlayerIndex();
            EntityHandler.Remove(entityGUID);

            for (int i = 0; i < m_EntityHolder.transform.childCount; i++)
            {
                GameObject temp = m_EntityHolder.transform.GetChild(i).gameObject;
                if (temp.name.Contains(entityGUID.ToString()))
                {
                    GameObject.Destroy(temp);
                    break;
                }
            }

            IsDirty = true;
        }

        public IEntity GetEntity(Vector2Int positionRef)
        {
            return m_Entities.FirstOrDefault(t => t.WorldPosition.Equals(positionRef));
        }

        public Sector GetSectorFromPoint(Vector2Int point)
        {
            int xCentreBegin, xRightBegin;
            int yCentreBegin, yBottomBegin;

            xCentreBegin = m_Tiles.GetLength(0) / 3;
            xRightBegin = xCentreBegin * 2;

            yCentreBegin = m_Tiles.GetLength(1) / 3;
            yBottomBegin = yCentreBegin * 2;

            int sectorX;
            int sectorY;

            if (point.x < xCentreBegin)
                sectorX = 0;
            else if (point.x < xRightBegin)
                sectorX = 1;
            else
                sectorX = 2;

            if (point.y < yCentreBegin)
                sectorY = 0;
            else if (point.y < yBottomBegin)
                sectorY = 1;
            else
                sectorY = 2;

            switch (sectorX)
            {
                case 0:
                    switch (sectorY)
                    {
                        case 0:
                            return Sector.NorthWest;

                        case 1:
                            return Sector.North;

                        case 2:
                            return Sector.NorthEast;
                    }

                    break;

                case 1:
                    switch (sectorY)
                    {
                        case 0:
                            return Sector.West;

                        case 1:
                            return Sector.Centre;

                        case 2:
                            return Sector.East;
                    }

                    break;

                case 2:
                    switch (sectorY)
                    {
                        case 0:
                            return Sector.SouthWest;

                        case 1:
                            return Sector.South;

                        case 2:
                            return Sector.SouthEast;
                    }

                    break;
            }

            return Sector.Centre;
        }

        public List<Vector2Int> GetVisibleWalls(IEntity viewer)
        {
            List<Vector2Int> visibleWalls = this.Walls
                .Where(wall => viewer.VisionProvider.CanSee(viewer, this, wall.Key))
                .ToDictionary(wall => wall.Key, wall => wall.Value).Keys.ToList();
            return visibleWalls;
        }

        public WorldTile[,] Tiles
        {
            get { return m_Tiles; }
        }

        public Dictionary<Vector2Int, IJoyObject> GetObjectsOfType(string[] tags)
        {
            Dictionary<Vector2Int, IJoyObject> objects = new Dictionary<Vector2Int, IJoyObject>();
            foreach (IJoyObject joyObject in m_Objects)
            {
                int matches = 0;
                foreach (string tag in tags)
                {
                    if (joyObject.HasTag(tag))
                    {
                        matches++;
                    }
                }

                if (matches == tags.Length || (tags.Length < joyObject.Tags.Count && matches > 0))
                {
                    objects.Add(joyObject.WorldPosition, joyObject);
                }
            }

            return objects;
        }

        public void AddArea(Vector2Int key, IWorldInstance value)
        {
            value.Parent = this;
            m_Areas.Add(key, value);
        }

        public bool HasTag(string tag)
        {
            return Tags.Contains(tag.ToLower());
        }

        public bool AddTag(string tag)
        {
            if (Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
            {
                string match = Tags.First(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
                return Tags.Remove(match);
            }

            return false;
        }

        public List<string> Tags { get; protected set; }

        public int[,] Light => m_Light;

        public Dictionary<Vector2Int, IWorldInstance> Areas
        {
            get { return m_Areas; }
        }

        public List<IEntity> Entities
        {
            get { return m_Entities; }
        }

        public List<IJoyObject> Objects
        {
            get { return m_Objects; }
        }

        public Dictionary<Vector2Int, IJoyObject> Walls
        {
            get { return m_Walls; }
        }

        public Vector2Int SpawnPoint
        {
            get { return m_SpawnPoint; }
            set { m_SpawnPoint = value; }
        }

        public IWorldInstance Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public long GUID
        {
            get { return m_GUID; }
            protected set { m_GUID = value; }
        }

        public string Name
        {
            get { return m_Name; }
            protected set { m_Name = value; }
        }

        public IEntity Player
        {
            get
            {
                if (!m_Entities.Any(x => x.PlayerControlled))
                {
                    return null;
                }

                return m_Entities[m_PlayerIndex];
            }
        }

        public Vector2Int Dimensions
        {
            get { return m_Dimensions; }
        }

        public bool IsDirty { get; protected set; }

        public byte[,] Costs
        {
            get { return m_Costs; }
        }
    }
}