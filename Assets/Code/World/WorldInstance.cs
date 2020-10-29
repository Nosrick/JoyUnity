using JoyLib.Code.Entities;
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
    public class WorldInstance
    {
        protected WorldTile[,] m_Tiles;
        protected byte[,] m_Costs;
        protected int[,] m_Light;
        protected bool[,] m_Discovered;

        [NonSerialized]
        protected int m_PlayerIndex;

        protected readonly Vector2Int m_Dimensions;

        protected readonly string[] m_Tags;

        //Worlds and where to access them
        protected Dictionary<Vector2Int, WorldInstance> m_Areas;

        [NonSerialized]
        protected WorldInstance m_Parent;

        protected List<Entity> m_Entities;
        
        protected List<JoyObject> m_Objects;
        protected Dictionary<Vector2Int, JoyObject> m_Walls;
        
        protected Vector2Int m_SpawnPoint;

        protected static DateTime s_DateTime;

        protected string m_Name;
        protected long m_GUID;

        [NonSerialized]
        protected GameObject m_FogOfWarHolder;
        [NonSerialized]
        protected GameObject m_WallHolder;
        [NonSerialized]
        protected GameObject m_ObjectHolder;
        [NonSerialized]
        protected GameObject m_EntityHolder;

        [NonSerialized]
        protected static LiveEntityHandler s_EntityHandler = GameObject.Find("GameManager")
                                                                .GetComponent<LiveEntityHandler>();

        /// <summary>
        /// A template for adding stuff to later. A blank WorldInstance.
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="tags"></param>
        /// <param name="name"></param>
        public WorldInstance(WorldTile[,] tiles, string[] tags, string name)
        {
            m_Dimensions = new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));

            this.Name = name;
            this.m_Tags = tags;
            m_Tiles = tiles;
            m_Areas = new Dictionary<Vector2Int, WorldInstance>();
            m_Entities = new List<Entity>();
            m_Objects = new List<JoyObject>();
            m_Walls = new Dictionary<Vector2Int, JoyObject>();
            GUID = GUIDManager.Instance.AssignGUID();
            m_Discovered = new bool[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];
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
        public WorldInstance(WorldTile[,] tiles, Dictionary<Vector2Int, WorldInstance> areas, List<Entity> entities,
            List<JoyObject> objects, Dictionary<Vector2Int, JoyObject> walls, string[] tags, string name)
        {
            this.Name = name;
            this.m_Tags = tags;
            m_Tiles = tiles;
            m_Areas = areas;
            m_Entities = entities;
            m_Objects = objects;
            m_Walls = walls;
            GUID = GUIDManager.Instance.AssignGUID();
            m_Discovered = new bool[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];
            CalculatePlayerIndex();
            m_Light = new int[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];

            m_FogOfWarHolder = GameObject.Find("WorldFog");
            m_WallHolder = GameObject.Find("WorldWalls");
            m_ObjectHolder = GameObject.Find("WorldObjects");
            m_EntityHolder = GameObject.Find("WorldEntities");

            m_Costs = new byte[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];
            for(int x = 0; x < m_Costs.GetLength(0); x++)
            {
                for(int y = 0; y < m_Costs.GetLength(1); y++)
                {
                    m_Costs[x, y] = 1;
                }
            }

            foreach(Vector2Int position in m_Walls.Keys)
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
            for (int i = 0; i < m_Entities.Count; i++)
            {
                if (m_Entities[i].PlayerControlled)
                {
                    m_PlayerIndex = i;
                    return;
                }
            }
        }

        protected void CalculateLightLevels()
        {
            m_Light = new int[m_Light.GetLength(0), m_Light.GetLength(1)];

            //Do objects first
            List<JoyObject> objects = m_Objects.ToList();

            for(int i = 0; i < objects.Count; i++)
            {
                if (objects[i] is ItemInstance == false)
                {
                    continue;
                }

                ItemInstance item = (ItemInstance)objects[i];

                int xMin, xMax;
                int yMin, yMax;

                xMin = Math.Max(0, item.WorldPosition.x - (item.ItemType.LightLevel));
                xMax = Math.Min(m_Light.GetLength(0) - 1, item.WorldPosition.x + (item.ItemType.LightLevel));

                yMin = Math.Max(0, item.WorldPosition.y - (item.ItemType.LightLevel));
                yMax = Math.Min(m_Light.GetLength(1) - 1, item.WorldPosition.y + (item.ItemType.LightLevel));

                if(item.ItemType.LightLevel > 0)
                {
                    for(int x = xMin; x < xMax; x++)
                    {
                        for(int y = yMin; y < yMax; y++)
                        {
                            m_Light = LightTiles(new Vector2Int(x, y), item, m_Light);
                        }
                    }
                }
            }

            //Then the objects used by entities
            List<Entity> entities = m_Entities.ToList();

            for(int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                ItemInstance[] backpack = entity.Backpack;
                for(int j = 0; j < backpack.Length; j++)
                {
                    ItemInstance item = backpack[j];
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

        protected int[,] LightTiles(Vector2Int pointRef, ItemInstance objectRef, int[,] lightRef)
        {
            int[,] vision = lightRef;

            int lightLevel = objectRef.ItemType.LightLevel;
            Rect lightRect = new Rect(objectRef.WorldPosition.x - lightLevel, objectRef.WorldPosition.y - lightLevel, lightLevel * 2, lightLevel * 2);

            for (int i = 0; i < 360; i++)
            {
                float x = (float)Math.Cos(i * 0.01745f);
                float y = (float)Math.Sin(i * 0.01745f);
                vision = LightTile(x, y, objectRef, vision);
            }

            return vision;
        }

        protected int[,] LightTile(float x, float y, ItemInstance objectRef, int[,] visionRef)
        {
            int[,] vision = visionRef;
            float oX, oY;

            oX = objectRef.WorldPosition.x + 0.5f;
            oY = objectRef.WorldPosition.y + 0.5f;

            int itemPosX = objectRef.WorldPosition.x;
            int itemPosY = objectRef.WorldPosition.y;

            for (int i = 0; i < objectRef.ItemType.LightLevel; i++)
            {

                int posX = (int)oX;
                int posY = (int)oY;
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

                int lightLevel = (int)Math.Max(objectRef.ItemType.LightLevel, Math.Sqrt(((itemPosX - posX) * (itemPosX - posX) + (itemPosY - posY) * (itemPosY - posY))));
                vision[posX, posY] = lightLevel;

                oX += x;
                oY += y;
            }

            return vision;
        }

        public void AddObject(JoyObject objectRef)
        {
            if(objectRef.IsWall)
            {
                AddWall(objectRef);
            }
            else
            {
                m_Objects.Add(objectRef);
            }
            IsDirty = true;
        }

        protected void AddWall(JoyObject wallRef)
        {
            m_Walls.Add(wallRef.WorldPosition, wallRef);
            m_Costs[wallRef.WorldPosition.x, wallRef.WorldPosition.y] = byte.MaxValue;
        }

        public bool RemoveObject(Vector2Int positionRef, ItemInstance itemRef)
        {
            List<long> objectGUIDs = new List<long>();
            bool removed = false;
            for (int i = 0; i < m_Objects.Count; i++)
            {
                if (m_Objects[i].WorldPosition == positionRef && itemRef.GUID == m_Objects[i].GUID)
                {
                    objectGUIDs.Add(m_Objects[i].GUID);
                    m_Objects.RemoveAt(i);
                    IsDirty = true;
                    removed = true;
                    break;
                }
            }

            for(int i = 0; i < m_ObjectHolder.transform.childCount; i++)
            {
                GameObject temp = m_ObjectHolder.transform.GetChild(i).gameObject;
                for (int j = 0; j < objectGUIDs.Count; j++)
                {
                    if (temp.name.Contains(objectGUIDs[j].ToString()))
                    {
                        GameObject.Destroy(temp);
                        break;
                    }
                }
            }

            return removed;
        }

        public JoyObject GetObject(Vector2Int WorldPosition)
        {
            for(int i = 0; i < m_Objects.Count; i++)
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
        }

        public void Update()
        {
            //CalculateLightLevels();

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
        /// <param name="objectType"></param>
        /// <param name="intentRef"></param>
        /// <returns></returns>
        public IEnumerable<JoyObject> SearchForObjects(Entity entityRef, IEnumerable<string> tags)
        {
            List<JoyObject> data = new List<JoyObject>();

            if (entityRef.Vision.GetLength(0) == 1)
            {
                return data;
            }

            List<JoyObject> inSight = m_Objects.Where(obj => entityRef.VisionProvider.CanSee(entityRef, this, obj.WorldPosition) == true).ToList();
            foreach(JoyObject obj in inSight)
            {
                IEnumerable<string> intersect = obj.Tags.Intersect(tags);
                if(tags.Any() == false || intersect.SequenceEqual(tags))
                {
                    data.Add(obj);
                }
            }

            return data;
        }

        public IEnumerable<Entity> SearchForEntities(Entity actor, IEnumerable<string> searchCriteria)
        {
            List<Entity> searchEntities = new List<Entity>();

            foreach(Entity entity in m_Entities)
            {
                if(!actor.VisionProvider.CanSee(actor, this, entity.WorldPosition))
                {
                    continue;
                }

                IEnumerable<Tuple<string, int>> data = entity.GetData(searchCriteria.ToArray());
                IEnumerable<string> tags = data.Select(x => x.Item1);
                if(tags.SequenceEqual(searchCriteria))
                {
                    searchEntities.Add(entity);
                }
            }

            return searchEntities;
        }

        public Entity GetRandomSentient()
        {
            List<Entity> sentients = m_Entities.Where(x => x.Sentient).ToList();
            if (sentients.Count > 0)
                return sentients[RNG.instance.Roll(0, sentients.Count - 1)];
            else
                return null;
        }

        public Entity GetRandomSentientWorldWide()
        {
            List<WorldInstance> worlds = GetWorlds(GetOverworld());
            int result = RNG.instance.Roll(0, worlds.Count - 1);
            Entity entity = worlds[result].GetRandomSentient();
            while (entity == null)
            {
                result = RNG.instance.Roll(0, worlds.Count - 1);
                entity = worlds[result].GetRandomSentient();
            }
            return entity;
        }

        public List<WorldInstance> GetWorlds(WorldInstance parent)
        {
            List<WorldInstance> worlds = new List<WorldInstance>();
            worlds.Add(parent);
            for(int i = 0; i < worlds.Count; i++)
            {
                foreach(WorldInstance world in worlds[i].m_Areas.Values)
                {
                    List<WorldInstance> newWorlds = GetWorlds(world);
                    for(int j = 0; j < newWorlds.Count; j++)
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
            foreach(KeyValuePair<Vector2Int, WorldInstance> pair in Parent.m_Areas)
            {
                if(pair.Value.GUID == this.GUID)
                {
                    return pair.Key;
                }
            }

            return new Vector2Int(-1, -1);
        }

        public WorldInstance GetOverworld()
        {
            if(Parent == null)
            {
                return this;
            }
            else
            {
                return Parent.GetOverworld();
            }
        }

        public WorldInstance GetPlayerWorld(WorldInstance parent)
        {
            if (parent.Entities.Any(x => x.PlayerControlled))
                return parent;

            foreach(WorldInstance world in parent.Areas.Values)
            {
                return GetPlayerWorld(world);
            }

            return null;
        }

        public void SwapPosition(Entity left, Entity right)
        {
            Vector2Int tempPosition = right.WorldPosition;
            right.Move(left.WorldPosition);
            left.Move(tempPosition);
        }

        public ItemInstance PickUpObject(Entity entityRef)
        {
            ItemInstance item = (ItemInstance)GetObject(entityRef.WorldPosition);
            if (item != null)
            {
                item.OwnerGUID = entityRef.GUID;
                entityRef.AddContents(item);
                m_Objects.Remove(item);

                return item;
            }

            return null;
        }

        public void TradeObjects(Entity leftEntity, ItemInstance leftItem, Entity rightEntity, ItemInstance rightItem)
        {
            if(leftItem != null)
            {
                leftEntity.RemoveItemFromBackpack(leftItem);
                rightEntity.AddContents(leftItem);
            }

            if(rightItem != null)
            {
                rightEntity.RemoveItemFromBackpack(rightItem);
                leftEntity.AddContents(rightItem);
            }
        }

        protected bool[,] DiscoverTiles(Vector2Int pointRef, Entity entityRef, bool[,] visionRef)
        {
            bool[,] vision = visionRef;

            int entityPerception = entityRef.VisionMod;
            for(int i = 0; i < 360; i++)
            {
                float x = (float)Math.Cos(i * 0.01745f);
                float y = (float)Math.Sin(i * 0.01745f);
                vision = DiscoverTile(x, y, entityRef, entityPerception, vision);
            }
            return vision;
        }

        protected bool[,] DiscoverTile(float x, float y, Entity entityRef, int perception, bool[,] visionRef)
        {
            bool[,] vision = visionRef;

            float oX = entityRef.WorldPosition.x + 0.5f;
            float oY = entityRef.WorldPosition.y + 0.5f;

            int arrayX = vision.GetLength(0);
            int arrayY = vision.GetLength(1);

            for (int i = 0; i < perception; i++)
            {
                if(oX < 0.0f || oY < 0.0f || oX >= arrayX || oY >= arrayY)
                {
                    return vision;
                }

                int posX = (int)oX;
                int posY = (int)oY;

                vision[posX, posY] = true;
                if(Walls.ContainsKey(new Vector2Int(posX, posY)))
                {
                    return vision;
                }

                oX += x;
                oY += y;
            }

            return vision;
        }

        /*
        protected bool[,] DiscoverTilesOLD(Vector2Int pointRef, Entity entityRef, bool[,] visionRef)
        {
            bool[,] vision = visionRef;

            float entityPerceptionMod = entityRef.VisionMod;
            
            Dictionary<Vector2Int, JoyObject> walls = m_Objects.Where(x => x.IsWall).ToDictionary(x => x.WorldPosition, x => x);

            for (int i = 0; i < 360; i++)
            {
                float x = (float)Math.Cos(i * 0.01745f);
                float y = (float)Math.Sin(i * 0.01745f);
                vision = DiscoverTileOLD(x, y, entityRef, entityPerceptionMod, walls, vision);
            }

            return vision;
        }

        protected bool[,] DiscoverTileOLD(float x, float y, Entity entityRef, float perceptionMod, 
            Dictionary<Vector2Int, JoyObject> walls, bool[,] visionRef)
        {
            bool[,] vision = visionRef;
            float oX, oY;

            oX = entityRef.WorldPosition.x + 0.5f;
            oY = entityRef.WorldPosition.y + 0.5f;

            for (int i = 0; i < perceptionMod; i++)
            {

                if (oX < 0.0f || oY < 0.0f || oX >= vision.GetLength(0) || oY >= vision.GetLength(1))
                    return vision;

                if (m_Light[(int)oX, (int)oY] == 0 && entityRef.VisionType != VisionType.Nocturnal)
                    return vision;

                vision[(int)oX, (int)oY] = true;
                if (walls.ContainsKey(new Vector2Int((int)oX, (int)oY)))
                    return vision;

                oX += x;
                oY += y;
            }

            return vision;
        }
        */

        public void AddEntity(Entity entityRef)
        {
            m_Entities.Add(entityRef);

            //Initialise a new GameObject here at some point

            CalculatePlayerIndex();
            IsDirty = true;
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
            s_EntityHandler.Remove(entityGUID);
            
            for (int i = 0; i < m_EntityHolder.transform.childCount; i++)
            {
                GameObject temp = m_EntityHolder.transform.GetChild(i).gameObject;
                if(temp.name.Contains(entityGUID.ToString()))
                {
                    GameObject.Destroy(temp);
                    break;
                }
            }

            IsDirty = true;
        }

        public Entity GetEntity(Vector2Int positionRef)
        {
            for (int i = 0; i < m_Entities.Count; i++)
            {
                if (m_Entities[i].WorldPosition.Equals(positionRef))
                {
                    return m_Entities[i];
                }
            }
            return null;
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

            switch(sectorX)
            {
                case 0:
                    switch(sectorY)
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
                    switch(sectorY)
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

        public List<Vector2Int> GetVisibleWalls(Entity viewer)
        {
            List<Vector2Int> visibleWalls = this.Walls.Where(wall => viewer.VisionProvider.CanSee(viewer, this, wall.Key)).ToDictionary(wall => wall.Key, wall => wall.Value).Keys.ToList();
            return visibleWalls;
        }

        public WorldTile[,] Tiles
        {
            get
            {
                return m_Tiles;
            }
        }
        
        public bool[,] DiscoveredTiles
        {
            get
            {
                return m_Discovered;
            }
            protected set
            {
                m_Discovered = value;
            }
        }

        public Dictionary<Vector2Int, JoyObject> GetObjectsOfType(string[] tags)
        {
            Dictionary<Vector2Int, JoyObject> objects = new Dictionary<Vector2Int, JoyObject>();
            foreach (JoyObject joyObject in m_Objects)
            {
                int matches = 0;
                foreach(string tag in tags)
                {
                    if(joyObject.HasTag(tag))
                    {
                        matches++;
                    }
                }
                if(matches == tags.Length || (tags.Length < joyObject.TotalTags && matches > 0))
                {
                    objects.Add(joyObject.WorldPosition, joyObject);
                }
            }
            return objects;
        }

        public void AddArea(Vector2Int key, WorldInstance value)
        {
            value.Parent = this;
            m_Areas.Add(key, value);
        }

        public bool HasTag(string tag)
        {
            return m_Tags.Contains(tag.ToLower());
        }
        
        public Dictionary<Vector2Int, WorldInstance> Areas
        {
            get
            {
                return m_Areas;
            }
        }

        public List<Entity> Entities
        {
            get
            {
                return m_Entities;
            }
        }

        public List<JoyObject> Objects
        {
            get
            {
                return m_Objects;
            }
        }

        public Dictionary<Vector2Int, JoyObject> Walls
        {
            get
            {
                return m_Walls;
            }
        }
        
        public Vector2Int SpawnPoint
        {
            get
            {
                return m_SpawnPoint;
            }
            set
            {
                m_SpawnPoint = value;
            }
        }
        
        public WorldInstance Parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                m_Parent = value;
            }
        }

        public long GUID
        {
            get
            {
                return m_GUID;
            }
            protected set
            {
                m_GUID = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
            protected set
            {
                m_Name = value;
            }
        }

        public Entity Player
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
        
        public int[,] LightLevels
        {
            get
            {
                return m_Light;
            }
        }

        public Vector2Int Dimensions
        {
            get
            {
                return m_Dimensions;
            }
        }

        public bool IsDirty
        {
            get;
            protected set;
        }

        public byte[,] Costs
        {
            get
            {
                return m_Costs;
            }
        }
    }

    public enum EntitySentienceSearch
    {
        Sentient,
        NonSentient,
        Any,
        Matching
    }

    public enum EntityTypeSearch
    {
        MatchingAll,
        MatchingBaseType,
        NoMatch,
        Any
    }
}