using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using JoyLib.Code.IO;
using JoyLib.Code.Managers;
using JoyLib.Code.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace JoyLib.Code.World
{
    [Serializable]
    public class WorldInstance
    {
        protected WorldTile[,] m_Tiles;
        protected int[,] m_Light;
        protected bool[,] m_Discovered;
        protected int m_PlayerIndex;

        protected readonly WorldType m_Type;

        //Worlds and where to access them
        protected Dictionary<Vector2Int, WorldInstance> m_Areas;

        [NonSerialized]
        protected WorldInstance m_Parent;

        protected List<Entity> m_Entities;
        
        protected List<JoyObject> m_Objects;
        
        protected Vector2Int m_SpawnPoint;

        protected static DateTime s_DateTime;

        protected string m_Name;
        protected int m_GUID;

        public WorldInstance(WorldTile[,] tiles, Dictionary<Vector2Int, WorldInstance> areas, List<Entity> entities,
            List<JoyObject> objects, WorldType type, string name)
        {
            this.Name = name;
            this.m_Type = type;
            m_Tiles = tiles;
            m_Areas = areas;
            m_Entities = entities;
            m_Objects = objects;
            GUID = GUIDManager.AssignGUID();
            m_Discovered = new bool[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];
            m_Type = type;
            CalculatePlayerIndex();
            m_Light = new int[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];
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
                    continue;

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
                for(int j = 0; j < entity.Backpack.Count; j++)
                {
                    ItemInstance item = entity.Backpack[j];
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

            lock (m_Objects)
            {
                Dictionary<Vector2Int, JoyObject> walls = m_Objects.Where(x => x.IsWall).ToDictionary(x => x.WorldPosition, x => x);

                for (int i = 0; i < 360; i++)
                {
                    float x = (float)Math.Cos(i * 0.01745f);
                    float y = (float)Math.Sin(i * 0.01745f);
                    vision = LightTile(x, y, objectRef, walls, vision);
                }
            }

            return vision;
        }

        protected int[,] LightTile(float x, float y, ItemInstance objectRef, Dictionary<Vector2Int, JoyObject> walls, int[,] visionRef)
        {
            int[,] vision = visionRef;
            float oX, oY;

            oX = objectRef.WorldPosition.x + 0.5f;
            oY = objectRef.WorldPosition.y + 0.5f;

            for (int i = 0; i < objectRef.ItemType.LightLevel; i++)
            {

                if (oX < 0.0f || oY < 0.0f || oX >= vision.GetLength(0) || oY >= vision.GetLength(1))
                    return vision;

                /*
                if (m_Light[(int)oX, (int)oY] == 0)
                    return vision;
                */

                vision[(int)oX, (int)oY] = (int)Math.Max(0, (objectRef.ItemType.LightLevel - Vector2.Distance(objectRef.WorldPosition, new Vector2(oX, oY))));
                //vision[(int)oX, (int)oY] = Math.Min(16, vision[(int)oX, (int)oY]);
                if (walls.ContainsKey(new Vector2Int((int)oX, (int)oY)))
                    return vision;

                oX += x;
                oY += y;
            }

            return vision;
        }

        public void AddObject(JoyObject objectRef)
        {
            lock(m_Objects)
            {
                m_Objects.Add(objectRef);
            }
        }

        public void RemoveObject(Vector2Int positionRef)
        {
            lock(m_Objects)
            {
                for (int i = 0; i < m_Objects.Count; i++)
                {
                    if (m_Objects[i].WorldPosition == positionRef)
                    {
                        m_Objects.RemoveAt(i);
                        return;
                    }
                }
            }
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
            if (WorldType != WorldType.Overworld)
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
            Thread thread = new Thread(new ThreadStart(CalculateLightLevels));
            thread.Start();

            foreach (Entity entity in m_Entities)
            {
                Thread childThread = new Thread(new ThreadStart(entity.UpdateMe));
                childThread.Start();
            }
        }

        /// <summary>
        /// TODO: REDO THIS
        /// </summary>
        /// <param name="entityRef"></param>
        /// <param name="objectType"></param>
        /// <param name="intentRef"></param>
        /// <returns></returns>
        public List<NeedAIData> SearchForObjects(Entity entityRef, string objectType, Intent intentRef)
        {
            List<NeedAIData> data = new List<NeedAIData>();

            if (entityRef.Vision.GetLength(0) == 1)
                return data;

            //Special cases
            //Ownable objects
            if(objectType.Equals("Ownable"))
            {
                for(int i = 0; i < m_Objects.Count; i++)
                {
                    if (m_Objects[i].IsWall)
                        continue;

                    if (!entityRef.Vision[m_Objects[i].WorldPosition.x, m_Objects[i].WorldPosition.y])
                        continue;

                    if (!m_Objects[i].GetType().Equals(typeof(ItemInstance)))
                        continue;

                    NeedAIData tempData = new NeedAIData();
                    tempData.intent = intentRef;
                    tempData.target = m_Objects[i];
                    data.Add(tempData);
                }
            }

            for(int i = 0; i < m_Objects.Count; i++)
            {
                if(entityRef.Vision[m_Objects[i].WorldPosition.x, m_Objects[i].WorldPosition.y])
                {
                    if (m_Objects[i].BaseType.Equals(objectType))
                    {
                        NeedAIData tempData = new NeedAIData();
                        tempData.intent = intentRef;
                        tempData.target = m_Objects[i];
                        data.Add(tempData);
                    }
                }
            }

            return data;
        }

        public List<NeedAIData> SearchForEntities(Entity searcher, string entityTypeRef, Intent intentRef)
        {
            List<NeedAIData> data = new List<NeedAIData>();

            //Special cases
            //Non-sentient entities
            if (entityTypeRef.Equals("Entities-ns"))
            {
                List<Entity> entities = m_Entities.Where(x => x.Sentient == false).ToList();

                for (int i = 0; i < entities.Count; i++)
                {
                    if (searcher.Vision[entities[i].WorldPosition.x, entities[i].WorldPosition.y] && searcher.GUID != entities[i].GUID)
                    {
                        NeedAIData tempData = new NeedAIData();
                        tempData.intent = intentRef;
                        tempData.target = entities[i];
                        data.Add(tempData);
                    }
                }
                return data;
            }

            //Sentient entities
            else if (entityTypeRef.Equals("Entities-s"))
            {
                List<Entity> entities = m_Entities.Where(x => x.Sentient).ToList();

                for (int i = 0; i < entities.Count; i++)
                {
                    if (searcher.Vision[entities[i].WorldPosition.x, entities[i].WorldPosition.y] && searcher.GUID != entities[i].GUID)
                    {
                        NeedAIData tempData = new NeedAIData();
                        tempData.intent = intentRef;
                        tempData.target = entities[i];
                        data.Add(tempData);
                    }
                }
                return data;
            }

            //All entities
            else if(entityTypeRef.Equals("Entities"))
            {
                List<NeedAIData> dataList = new List<NeedAIData>();
                for(int i = 0; i < m_Entities.Count; i++)
                {
                    if(searcher.Vision[m_Entities[i].WorldPosition.x, m_Entities[i].WorldPosition.y] && searcher.GUID != m_Entities[i].GUID)
                    {
                        NeedAIData tempData = new NeedAIData();
                        tempData.intent = intentRef;
                        tempData.target = m_Entities[i];
                        dataList.Add(tempData);
                    }
                }
                return dataList;
            }

            else
            {
                for (int i = 0; i < m_Entities.Count; i++)
                {
                    if (searcher.Vision[m_Entities[i].WorldPosition.x, m_Entities[i].WorldPosition.y])
                    {
                        if ((m_Entities[i].BaseType.Equals(entityTypeRef) ||
                            m_Entities[i].CreatureType.Equals(entityTypeRef)) &&
                            searcher.GUID != Entities[i].GUID)
                        {
                            NeedAIData tempData = new NeedAIData();
                            tempData.intent = intentRef;
                            tempData.target = Entities[i];
                            data.Add(tempData);
                        }
                    }
                }
                return data;
            }
        }

        public NeedAIData SearchForEntities(Entity entityRef, List<int> GUIDs, Intent intentRef)
        {
            NeedAIData data = new NeedAIData();
            data.intent = intentRef;

            Dictionary<int, Entity> chosenEntities = m_Entities.Where(x => GUIDs.Contains(x.GUID)).ToDictionary(x => x.GUID, x => x);
            List<Entity> visibleEntities = new List<Entity>();

            foreach(Entity entity in chosenEntities.Values)
            {
                if(entityRef.Vision[entity.WorldPosition.x, entity.WorldPosition.y] && entity.GUID != entityRef.GUID)
                {
                    visibleEntities.Add(entity);
                }
            }
            if (visibleEntities.Count > 0)
            {
                data.target = visibleEntities[RNG.Roll(0, visibleEntities.Count - 1)];
            }
            return data;
        }

        public NeedAIData SearchForMate(Entity entityRef)
        {
            NeedAIData data = new NeedAIData();
            data.intent = Intent.Interact;

            List<Entity> validPartners = new List<Entity>();
            if (entityRef.Sexuality == Sexuality.Heterosexual)
            {
                validPartners = m_Entities.Where(x => x.Gender != entityRef.Gender && x.Sentient == entityRef.Sentient &&
                (x.Sexuality == Sexuality.Heterosexual || x.Sexuality == Sexuality.Bisexual) && x.CreatureType.Equals(entityRef.CreatureType) &&
                x.Needs[NeedIndex.Sex].contributingHappiness == false).ToList();
            }
            else if(entityRef.Sexuality == Sexuality.Homosexual)
            {
                validPartners = m_Entities.Where(x => x.Gender == entityRef.Gender && x.Sentient == entityRef.Sentient &&
                (x.Sexuality == Sexuality.Homosexual || x.Sexuality == Sexuality.Bisexual) && x.CreatureType.Equals(entityRef.CreatureType) &&
                x.Needs[NeedIndex.Sex].contributingHappiness == false).ToList();
            }
            else if(entityRef.Sexuality == Sexuality.Bisexual)
            {
                validPartners = m_Entities.Where(x => x.Sentient == entityRef.Sentient &&
                (x.Sexuality == Sexuality.Heterosexual || x.Sexuality == Sexuality.Bisexual) && x.CreatureType.Equals(entityRef.CreatureType) &&
                x.Needs[NeedIndex.Sex].contributingHappiness == false).ToList();
            }

            List<Entity> visiblePartners = new List<Entity>();
            for(int i = 0; i < validPartners.Count; i++)
            {
                if (validPartners[i].GUID == entityRef.GUID)
                    continue;

                if (entityRef.Vision[validPartners[i].WorldPosition.x, validPartners[i].WorldPosition.y])
                    visiblePartners.Add(validPartners[i]);
            }
            if (visiblePartners.Count > 0)
                data.target = visiblePartners[RNG.Roll(0, visiblePartners.Count - 1)];

            return data;
        }

        public Entity GetRandomSentient()
        {
            List<Entity> sentients = m_Entities.Where(x => x.Sentient).ToList();
            if (sentients.Count > 0)
                return sentients[RNG.Roll(0, sentients.Count - 1)];
            else
                return null;
        }

        public Entity GetRandomSentientWorldWide()
        {
            List<WorldInstance> worlds = GetWorlds(GetOverworld());
            int result = RNG.Roll(0, worlds.Count - 1);
            Entity entity = worlds[result].GetRandomSentient();
            while (entity == null)
            {
                result = RNG.Roll(0, worlds.Count - 1);
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

        public void PickUpObject(Entity entityRef)
        {
            ItemInstance item = (ItemInstance)GetObject(entityRef.WorldPosition);
            if (item != null)
            {
                item.OwnerGUID = entityRef.GUID;
                entityRef.AddItem(item);
                lock (m_Objects)
                {
                    m_Objects.Remove(item);
                }
            }
        }

        public void TradeObjects(Entity leftEntity, ItemInstance leftItem, Entity rightEntity, ItemInstance rightItem)
        {
            if(leftItem != null)
            {
                leftEntity.RemoveItemFromBackpack(leftItem);
                rightEntity.AddItem(leftItem);
            }

            if(rightItem != null)
            {
                rightEntity.RemoveItemFromBackpack(rightItem);
                leftEntity.AddItem(rightItem);
            }
        }

        public bool[,] GetVision(Entity entityRef)
        {
            bool[,] vision = new bool[m_Tiles.GetLength(0), m_Tiles.GetLength(1)];
            vision = DiscoverTiles(entityRef.WorldPosition, entityRef, vision);
            if (entityRef.PlayerControlled)
            {
                lock (m_Discovered)
                {
                    m_Discovered = DiscoverTiles(entityRef.WorldPosition, entityRef, m_Discovered);
                }
            }
            return vision;
        }

        protected bool[,] DiscoverTiles(Vector2Int pointRef, Entity entityRef, bool[,] visionRef)
        {
            bool[,] vision = visionRef;

            float entityPerceptionMod = entityRef.Statistics[StatisticIndex.Perception] / 10 + 1;

            lock(m_Objects)
            {
                Dictionary<Vector2Int, JoyObject> walls = m_Objects.Where(x => x.IsWall).ToDictionary(x => x.WorldPosition, x => x);

                for (int i = 0; i < 360; i++)
                {
                    float x = (float)Math.Cos(i * 0.01745f);
                    float y = (float)Math.Sin(i * 0.01745f);
                    vision = DiscoverTile(x, y, entityRef, entityPerceptionMod, walls, vision);
                }
            }

            return vision;
        }

        protected bool[,] DiscoverTile(float x, float y, Entity entityRef, float perceptionMod, 
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

        public void AddEntity(Entity entityRef)
        {
            m_Entities.Add(entityRef);
            CalculatePlayerIndex();
        }

        public void RemoveEntity(Vector2Int positionRef)
        {
            for (int i = 0; i < m_Entities.Count; i++)
            {
                if (m_Entities[i].WorldPosition.Equals(positionRef))
                {
                    m_Entities.RemoveAt(i);
                    break;
                }
            }
            CalculatePlayerIndex();
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

        public Entity GetEntity(int GUID)
        {
            for(int i = 0; i < m_Entities.Count; i++)
            {
                if(m_Entities[i].GUID == GUID)
                {
                    return m_Entities[i];
                }
            }

            return null;
        }

        public Entity GetEntityRecursive(int GUID)
        {
            for(int i = 0; i < m_Entities.Count; i++)
            {
                if(m_Entities[i].GUID == GUID)
                {
                    return m_Entities[i];
                }
            }

            List<WorldInstance> worlds = m_Areas.Values.ToList();
            for(int i = 0; i < worlds.Count; i++)
            {
                Entity entity = worlds[i].GetEntityRecursive(GUID);
                if(entity != null)
                {
                    return entity;
                }
            }

            return null;
        }

        public WorldInstance GetWorldOfEntity(int GUID)
        {
            for(int i = 0; i < m_Entities.Count; i++)
            {
                if(m_Entities[i].GUID == GUID)
                {
                    return this;
                }
            }

            List<WorldInstance> worlds = m_Areas.Values.ToList();
            for(int i = 0; i < worlds.Count; i++)
            {
                WorldInstance world = worlds[i].GetWorldOfEntity(GUID);
                if(world != null)
                {
                    return world;
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

        public Dictionary<Vector2Int, JoyObject> GetObjectsOfType(string type)
        {
            Dictionary<Vector2Int, JoyObject> objects = new Dictionary<Vector2Int, JoyObject>();
            foreach (JoyObject joyObject in m_Objects)
            {
                if (joyObject.BaseType.Equals(type))
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

        public string GetLocalAreaInfo(Entity entityRef)
        {
            if (m_Type == WorldType.Interior)
            {
                int result = RNG.Roll(0, 100);
                if (result <= 50)
                {
                    int numberOfLevels = 1;
                    numberOfLevels = WorldConversationDataHelper.GetNumberOfFloors(numberOfLevels, this);

                    if (numberOfLevels == 1)
                        return "This place only has " + numberOfLevels + " floor to it.";
                    else
                        return "This place has at least " + numberOfLevels + " floors.";
                }
                else if (result > 50)
                {
                    int exactNumber = WorldConversationDataHelper.GetNumberOfCreatures(entityRef, this);
                    int roughNumber = 0;
                    if (exactNumber % 10 < 6)
                    {
                        roughNumber = exactNumber - (exactNumber % 10);
                    }
                    else
                    {
                        roughNumber = exactNumber + (exactNumber % 10);
                    }
                    return "There are around " + roughNumber + " " + entityRef.CreatureType + "s here.";
                }
            }

            return "I don't know much about this place, sorry.";
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

        public int GUID
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
                    return null;

                return m_Entities[m_PlayerIndex];
            }
        }

        public WorldType WorldType
        {
            get
            {
                return m_Type;
            }
        }
        
        public int[,] LightLevels
        {
            get
            {
                return m_Light;
            }
        }
    }
}