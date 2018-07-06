using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Helpers;
using JoyLib.Code.Loaders;
using JoyLib.Code.Physics;
using JoyLib.Code.Quests;
using JoyLib.Code.States;
using JoyLib.Code.World;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class Entity : JoyObject
    {
        protected Dictionary<StatisticIndex, int> m_Statistics;
        protected Dictionary<string, EntitySkill> m_Skills;
        protected Dictionary<NeedIndex, EntityNeed> m_Needs;
        protected List<Ability> m_Abilities;
        protected Dictionary<string, ItemInstance> m_Equipment;
        protected List<ItemInstance> m_Backpack;
        protected ItemInstance m_NaturalWeapons;
        protected Sexuality m_Sexuality;

        protected List<string> m_IdentifiedItems;

        protected Dictionary<int, int> m_Relationships;
        protected Dictionary<int, RelationshipStatus> m_Family;

        protected JobType m_CurrentJob;
        protected Dictionary<string, int> m_JobLevels;

        protected int m_Size;

        protected int m_Level;
        protected float m_Experience;

        protected int m_Mana;
        protected int m_ManaRemaining;

        protected int m_Composure;
        protected int m_ComposureRemaining;

        protected int m_Concentration;
        protected int m_ConcentrationRemaining;

        protected bool m_Sentient;

        protected bool[,] m_Vision;
        protected VisionType m_VisionType;

        protected NeedIndex m_FulfillingNeed;
        protected int m_FulfilmentCounter;

        protected string m_Tileset;

        protected NeedAIData m_CurrentTarget;

        protected Pathfinder m_Pathfinder;
        
        protected Queue<Vector2Int> m_PathfindingData;

        protected const int XP_PER_LEVEL = 100;
        protected const int NEED_FULFILMENT_COUNTER = 5;
        protected const int REGEN_TICK_TIME = 10;

        protected static Dictionary<string, CultureType> s_Cultures = CultureLoader.LoadCultures();

        public static Entity Create(EntityTemplate template, Dictionary<NeedIndex, EntityNeed> needs, int level, float experience, JobType job, Gender gender, Sexuality sexuality,
            Vector2Int position, List<Texture2D> icons, ItemInstance naturalWeapons, Dictionary<string, ItemInstance> equipment, 
            List<ItemInstance> backpack, Dictionary<int, int> relationships, List<string> identifiedItems, Dictionary<int, RelationshipStatus> family,
            Dictionary<string, int> jobLevels, WorldInstance world, string tileset)
        {
            Entity entity = Instantiate(Resources.Load<Entity>("Prefabs/Sprite"));

            List<Sprite> sprites = new List<Sprite>();
            for(int i = 0; i < icons.Count; i++)
            {
                sprites.Add(Sprite.Create(icons[i], new Rect(0, 0, 16, 16), Vector2.zero, 16));
            }

            entity.CreatureType = template.CreatureType;

            entity.Initialise(NameProvider.GetRandomName(template.CreatureType, gender), (template.Statistics[StatisticIndex.Endurance] * 2), position, sprites, template.JoyType, true);

            entity.m_Size = template.Size;

            entity.m_JobLevels = jobLevels;
            entity.m_Sexuality = sexuality;
            entity.m_IdentifiedItems = identifiedItems;
            entity.m_Statistics = template.Statistics;
            entity.m_Skills = template.Skills;
            entity.m_Needs = needs;
            entity.m_Abilities = template.Abilities;
            entity.m_Level = level;
            for(int i = 1; i < level; i++)
            {
                entity.LevelUp();
            }
            entity.m_Experience = experience;
            entity.m_CurrentJob = job;
            entity.m_Sentient = template.Sentient;
            entity.m_NaturalWeapons = naturalWeapons;
            entity.m_Equipment = equipment;
            entity.m_Backpack = backpack;
            entity.m_Relationships = relationships;
            entity.Gender = gender;
            entity.m_Family = family;
            entity.m_VisionType = template.VisionType;

            entity.m_Tileset = tileset;

            entity.CalculateDerivatives();

            entity.m_Vision = new bool[1, 1];

            entity.m_Pathfinder = new Pathfinder();
            entity.m_PathfindingData = new Queue<Vector2Int>();

            foreach(EntityNeed need in entity.m_Needs.Values)
            {
                need.SetParent(entity);
            }

            entity.m_FulfillingNeed = (NeedIndex)(-1);
            entity.m_FulfilmentCounter = 0;

            entity.RegenTicker = RNG.Roll(0, REGEN_TICK_TIME - 1);

            entity.MyWorld = world;

            Entity gameEntity = entity.gameObject.GetComponent<Entity>();
            gameEntity = entity;
            
            return entity;
        }

        public static Entity CreateBrandNew(EntityTemplate template, Dictionary<NeedIndex, EntityNeed> needs, int level, JobType job, Gender gender, Sexuality sexuality,
            Vector2Int position, List<Texture2D> icons, WorldInstance world)
        {
            Entity entity = Instantiate(Resources.Load<Entity>("Prefabs/Entity"));

            List<Sprite> sprites = new List<Sprite>();
            for (int i = 0; i < icons.Count; i++)
            {
                sprites.Add(Sprite.Create(icons[i], new Rect(0, 0, 16, 16), Vector2.zero, 16));
            }

            entity.CreatureType = template.CreatureType;

            entity.Initialise(NameProvider.GetRandomName(template.CreatureType, gender), (template.Statistics[StatisticIndex.Endurance] * 2), position, sprites, template.JoyType, true);

            entity.m_Size = template.Size;

            entity.m_JobLevels = new Dictionary<string, int>();
            entity.m_Sexuality = sexuality;
            entity.m_IdentifiedItems = new List<string>();
            entity.m_Statistics = template.Statistics;
            entity.m_Skills = template.Skills;
            entity.m_Needs = needs;
            entity.m_Abilities = template.Abilities;
            entity.m_Level = level;
            for (int i = 1; i < level; i++)
            {
                entity.LevelUp();
            }
            entity.m_Experience = 0;
            entity.m_CurrentJob = job;
            entity.m_Sentient = template.Sentient;
            entity.m_NaturalWeapons = NaturalWeaponHelper.MakeNaturalWeapon(template.Size);
            entity.m_Equipment = new Dictionary<string, ItemInstance>();
            entity.m_Backpack = new List<ItemInstance>();
            entity.m_Relationships = new Dictionary<int, int>();
            entity.Gender = gender;
            entity.m_Family = new Dictionary<int, RelationshipStatus>();
            entity.m_VisionType = template.VisionType;

            entity.m_Tileset = template.Tileset;

            entity.CalculateDerivatives();

            entity.m_Vision = new bool[1, 1];

            entity.m_Pathfinder = new Pathfinder();
            entity.m_PathfindingData = new Queue<Vector2Int>();

            foreach (EntityNeed need in entity.m_Needs.Values)
            {
                need.SetParent(entity);
            }

            entity.m_FulfillingNeed = (NeedIndex)(-1);
            entity.m_FulfilmentCounter = 0;

            entity.RegenTicker = RNG.Roll(0, REGEN_TICK_TIME - 1);

            entity.MyWorld = world;

            Entity gameEntity = entity.gameObject.GetComponent<Entity>();
            gameEntity = entity;

            return entity;
        }

        /*
        public Entity(Dictionary<StatisticIndex, float> statistics, Dictionary<string, EntitySkill> skills, List<Ability> abilities,
            Dictionary<NeedIndex, EntityNeed> needs, int level, float experience, JobType job, Gender gender, string creatureType,
            Vector2Int position, string type, Texture2D[] icons, bool sentient, ItemInstance naturalWeapons,
            Dictionary<string, ItemInstance> equipment, List<ItemInstance> backpack, Dictionary<int, int> relationships,
            List<string> identifiedItems, Sexuality sexuality, Dictionary<int, RelationshipStatus> family,
            Dictionary<string, int> jobLevels, VisionType visionType, WorldInstance world, string tileset, bool initialise)
            :this(statistics, skills, abilities, needs, level, experience, job, gender, creatureType, position, type, icons, sentient,
                 naturalWeapons, equipment, backpack, relationships, identifiedItems, sexuality, family, jobLevels, visionType, world, tileset)
        {
            if (initialise)
            {
                Initialise();
                LevelUp();
            }
        }

        protected void Initialise()
        {
            List<string> keys = new List<string>(m_Skills.Keys);
            foreach(string key in keys)
            {
                if (m_CurrentJob.skillGrowths.ContainsKey(key))
                    m_Skills[key].AddExperience(500);
            }
        }

        public Entity DirectCopy()
        {
            Entity copy = Entity.Create(m_Size, m_Statistics, m_Skills, m_Abilities, m_Needs, m_Level, m_Experience,
                m_CurrentJob, Gender, CreatureType, WorldPosition, BaseType, m_Icons.ToList(), m_Sentient, m_NaturalWeapons, 
                m_Equipment, m_Backpack, m_Relationships, m_IdentifiedItems, m_Sexuality, m_Family, m_JobLevels, m_VisionType, MyWorld,
                m_Tileset);
            copy.GUID = GUIDManager.AssignGUID();
            return copy;
        }

        public Entity CopyWithFullNeeds()
        {
            Entity copy = Entity.Create(m_Size, StatisticVarianceHelper.Get(this), m_Skills, m_Abilities, EntityNeed.GetFullRandomisedNeeds(), m_Level, m_Experience,
                m_CurrentJob, s_Cultures[CreatureType].ChooseGender(), CreatureType, WorldPosition, BaseType, m_Icons.ToList(), m_Sentient, NaturalWeaponHelper.MakeNaturalWeapon(m_Size), 
                new Dictionary<string, ItemInstance>(), new List<ItemInstance>(), new Dictionary<int, int>(), new List<string>(), s_Cultures[CreatureType].ChooseSexuality(),
                new Dictionary<int, RelationshipStatus>(), m_JobLevels, m_VisionType, MyWorld, m_Tileset);
            copy.GUID = GUIDManager.AssignGUID();
            return copy;
        }

        public Entity CopyWithBasicNeeds()
        {
            Entity copy = Entity.Create(m_Size, StatisticVarianceHelper.Get(this), m_Skills, m_Abilities, EntityNeed.GetBasicRandomisedNeeds(), m_Level, m_Experience,
                m_CurrentJob, s_Cultures[CreatureType].ChooseGender(), CreatureType, WorldPosition, BaseType, m_Icons.ToList(), m_Sentient, NaturalWeaponHelper.MakeNaturalWeapon(m_Size), 
                new Dictionary<string, ItemInstance>(), new List<ItemInstance>(), new Dictionary<int, int>(), new List<string>(), s_Cultures[CreatureType].ChooseSexuality(),
                new Dictionary<int, RelationshipStatus>(), m_JobLevels, m_VisionType, MyWorld, m_Tileset);
            copy.GUID = GUIDManager.AssignGUID();
            return copy;
        }
        */

        /*
        public Entity CreateChild(Entity parentRef)
        {
            Dictionary<StatisticIndex, float> childStatistics = s_Cultures[creatureType]
            Entity child = new Entity()
        }
        */

        public void Tick()
        {
            if (m_FulfilmentCounter > 0)
                m_FulfilmentCounter -= 1;

            RegenTicker += 1;
            if(RegenTicker == REGEN_TICK_TIME)
            {
                m_HitPointsRemaining = Math.Min(m_HitPoints, m_HitPointsRemaining + (int)(Statistics[StatisticIndex.Endurance] / 10));
                m_ConcentrationRemaining = Math.Min(m_Concentration, m_ConcentrationRemaining + (int)(Statistics[StatisticIndex.Focus] / 10));
                m_ComposureRemaining = Math.Min(m_Composure, m_ComposureRemaining + (int)(Statistics[StatisticIndex.Wit] / 10));
                m_ManaRemaining = Math.Min(m_Mana, m_ManaRemaining + (int)((Statistics[StatisticIndex.Focus] + Statistics[StatisticIndex.Endurance] + Statistics[StatisticIndex.Wit]) / 30));
                RegenTicker = 0;
            }

            foreach(EntityNeed need in m_Needs.Values)
            {
                need.Tick();
            }
        }

        public void AddQuest(Quest quest)
        {
            List<QuestStep> steps = quest.steps;
            for(int i = 0; i < steps.Count; i++)
            {
                if(steps[i].action == QuestAction.Deliver && steps[i].objects.Count != 0)
                {
                    for(int j = 0; j < steps[i].objects.Count; j++)
                    {
                        AddItem(steps[i].objects[j]);
                    }
                }
            }
            QuestTracker.AddQuest(GUID, quest);
        }

        /*
        public void EquipItem(ItemInstance itemRef)
        {
            if (!m_Equipment.ContainsKey(itemRef.slot) && !itemRef.slot.Equals("Hands"))
                return;

            if(itemRef.slot.Equals("Hands"))
            {
                if (m_Equipment["Hand1"] == null)
                {
                    m_Equipment["Hand1"] = itemRef;
                }
                else if (m_Equipment["Hand2"] == null)
                {
                    m_Equipment["Hand2"] = itemRef;
                }
                else if (m_Equipment["Hand1"] != null && m_Equipment["Hand2"] != null)
                {
                    if (itemRef.weight <= m_Equipment["Hand2"].weight)
                    {
                        m_Backpack.Add(m_Equipment["Hand2"]);
                        m_Equipment["Hand2"] = itemRef;
                    }
                    else
                    {
                        m_Backpack.Add(m_Equipment["Hand1"]);
                        m_Equipment["Hand1"] = itemRef;
                    }
                }
            }
            else if(m_Equipment[itemRef.slot] != null)
            {
                m_Backpack.Add(m_Equipment[itemRef.slot]);
                m_Equipment[itemRef.slot] = itemRef;
            }
            else if(m_Equipment[itemRef.slot] == null)
            {
                m_Equipment[itemRef.slot] = itemRef;
            }
            m_Backpack.Remove(itemRef);
        }
        */

        public void EquipItem(ItemInstance itemRef, string slotRef)
        {
            if (!m_Equipment.ContainsKey(slotRef))
                return;

            if (m_Equipment[slotRef] != null)
                m_Backpack.Add(m_Equipment[slotRef]);

            m_Equipment[slotRef] = itemRef;
            m_Backpack.Remove(itemRef);
        }

        public void Update()
        {
            m_Vision = MyWorld.GetVision(this);

            HasMoved = false;

            if (!PlayerControlled)
            {
                List<NeedAIData> attackData = MyWorld.SearchForEntities(this, "Entities", Intent.Attack);
                List<NeedAIData> validTargets = attackData.Where(x => x.target.GetType().Equals(typeof(Entity))).ToList();
                validTargets = validTargets.Where(x => HasRelationship(x.target.GUID) < -50).ToList();

                if(validTargets.Count > 0 && m_CurrentTarget.target == null)
                {
                    m_CurrentTarget = validTargets[RNG.Roll(0, validTargets.Count - 1)];
                    m_PathfindingData = m_Pathfinder.FindPath(WorldPosition, m_CurrentTarget.target.WorldPosition, MyWorld);
                    if(AdjacencyHelper.IsAdjacent(WorldPosition, m_CurrentTarget.target.WorldPosition))
                    {
                        //CombatEngine.PerformCombat(this, (Entity)m_CurrentTarget.target);
                    }
                    else
                    {
                        MoveToTarget(m_CurrentTarget);
                    }
                    return;
                }
                else if(m_CurrentTarget.target != null)
                {
                    if(m_PathfindingData.Count == 0)
                    {
                        m_PathfindingData = m_Pathfinder.FindPath(WorldPosition, m_CurrentTarget.target.WorldPosition, MyWorld);
                    }
                    if (AdjacencyHelper.IsAdjacent(WorldPosition, m_CurrentTarget.target.WorldPosition))
                    {
                        //CombatEngine.PerformCombat(this, (Entity)m_CurrentTarget.target);
                    }
                    else if(m_PathfindingData.Count > 0)
                    {
                        MoveToTarget(m_CurrentTarget);
                    }
                    return;
                }

                List<EntityNeed> needs = m_Needs.Values.OrderByDescending(x => x.priority).ToList();
                //Act on needs first
                foreach(EntityNeed need in needs)
                {
                    if (need.contributingHappiness)
                        continue;

                    if (m_FulfilmentCounter > 0)
                        continue;

                    if (need.needData.target != null)
                    {
                        MoveToTarget(need.needData);

                        if (need.needData.target.GetType().Equals(typeof(ItemInstance)))
                        {
                            if (WorldPosition == need.needData.target.WorldPosition)
                            {
                                ActionLog.AddText(name + " (" + GUID + ") has arrived at " + need.needData.target.name + " at " + WorldPosition.ToString(), Helpers.LogType.Debug);
                                //TODO: FIX THIS
                                //need.ExecutePythonFunction("Interact", new dynamic[] { need, this, need.needData.target });
                            }
                        }
                        else if(need.needData.target.GetType().Equals(typeof(Entity)))
                        {
                            Entity target = (Entity)need.needData.target;
                            if(AdjacencyHelper.IsAdjacent(WorldPosition, need.needData.target.WorldPosition))
                            {
                                if (need.needData.intent == Intent.Interact && target.FulfilmentCounter <= 0)
                                {
                                    //need.ExecutePythonFunction("Interact", new dynamic[] { need, this, need.needData.target });
                                    if (target.PlayerControlled)
                                    {
                                        WorldState.TalkToPlayer(this);
                                    }
                                    else
                                    {
                                        m_FulfillingNeed = (NeedIndex)Enum.Parse(NeedIndex.Confidence.GetType(), need.name);
                                        m_FulfilmentCounter = NEED_FULFILMENT_COUNTER;
                                    }
                                }
                            }
                            else
                            {

                                if (m_PathfindingData.Count == 0)
                                {
                                    m_PathfindingData = m_Pathfinder.FindPath(WorldPosition, need.needData.target.WorldPosition, MyWorld);
                                }
                            }
                        }
                    }
                    else if(need.needData.target == null)
                    {
                        MoveToTarget(need.needData);

                        if(WorldPosition == need.needData.targetPoint)
                        {
                            ActionLog.AddText(name + " (" + GUID + ") has arrived at " + need.needData.targetPoint + " and is looking for fulfilment of " + need.name + ".", Helpers.LogType.Debug);
                        }
                    }
                }
            }
        }

        private void MoveToTarget(NeedAIData data)
        {
            if (!HasMoved && m_PathfindingData.Count > 0)
            {
                Vector2Int nextPoint = m_PathfindingData.Peek();
                PhysicsResult physicsResult = PhysicsManager.IsCollision(WorldPosition, nextPoint, MyWorld);
                if (physicsResult != PhysicsResult.EntityCollision)
                {
                    m_PathfindingData.Dequeue();
                    Move(nextPoint);
                    HasMoved = true;
                    m_Vision = MyWorld.GetVision(this);
                }
                else if (physicsResult == PhysicsResult.EntityCollision)
                {
                    MyWorld.SwapPosition(this, MyWorld.GetEntity(nextPoint));
                    m_PathfindingData.Dequeue();
                    Move(nextPoint);
                    HasMoved = true;
                    m_Vision = MyWorld.GetVision(this);
                }
            }
            else if (m_PathfindingData.Count == 0)
            {
                if (data.target != null)
                {
                    m_PathfindingData = m_Pathfinder.FindPath(WorldPosition, data.target.WorldPosition, MyWorld);
                }
                else if(data.targetPoint != Vector2Int.zero)
                {
                    m_PathfindingData = m_Pathfinder.FindPath(WorldPosition, data.targetPoint, MyWorld);
                }
            }
        }

        public void SetPath(Queue<Vector2Int> pointsRef)
        {
            m_PathfindingData = pointsRef;
        }

        public bool IsViableMate(Entity entityRef)
        {
            if(m_Sexuality == Sexuality.Bisexual)
            {
                return true;
            }
            else if(m_Sexuality == Sexuality.Heterosexual && (entityRef.Gender != Gender || entityRef.Gender == Gender.Neutral))
            {
                return true;
            }
            else if(m_Sexuality == Sexuality.Homosexual && (entityRef.Gender == Gender || entityRef.Gender == Gender.Neutral))
            {
                return true;
            }

            return false;
        }

        public void AddItem(ItemInstance itemRef)
        {
            ItemInstance item = itemRef;
            if(item == null)
            {
                return;
            }

            if (m_IdentifiedItems.Contains(item.name))
            {
                item.IdentifyMe();
                m_IdentifiedItems.Add(item.name);
            }

            m_Backpack.Add(item);
        }

        public void AddIdentifiedItem(string nameRef)
        {
            m_IdentifiedItems.Add(nameRef);
        }

        public void RemoveItemFromBackpack(ItemInstance item)
        {
            m_Backpack.Remove(item);
        }

        public void RemoveEquipment(string slot)
        {
            if (m_Equipment.ContainsKey(slot))
            {
                m_Equipment[slot] = null;
            }
        }

        public void RemoveEquipmentToBackpack(string slot)
        {
            if (m_Equipment.ContainsKey(slot))
            {
                m_Backpack.Add(m_Equipment[slot]);
                m_Equipment[slot] = null;
            }
        }

        public void PlaceItemInWorld(ItemInstance item)
        {
            m_Backpack.Remove(item);
            item.Move(WorldPosition);
            MyWorld.AddObject(item);
        }

        public void UnequipItem(string slot)
        {
            if (m_Equipment.ContainsKey(slot))
            {
                AddItem(m_Equipment[slot]);
                m_Equipment[slot] = null;
            }
        }

        public void InfluenceMe(int GUID, int value)
        {
            lock(m_Relationships)
            {
                if (m_Relationships.ContainsKey(GUID))
                {
                    m_Relationships[GUID] += value;
                }
                else
                {
                    PerformFirstImpression(GUID);
                }
            }
        }

        public int HasRelationship(int GUID)
        {
            if (!m_Relationships.ContainsKey(GUID))
            {
                PerformFirstImpression(GUID);
            }
            return m_Relationships[GUID];
        }

        private void PerformFirstImpression(int GUID)
        {
            Entity entity = MyWorld.GetEntityRecursive(GUID);
            int firstImpression = (int)((entity.Statistics[StatisticIndex.Personality] + entity.Statistics[StatisticIndex.Suavity] + entity.Statistics[StatisticIndex.Wit]) / 3);
            lock(m_Relationships)
            {
                m_Relationships.Add(GUID, firstImpression);
            }
        }

        public void DecreaseMana(int value)
        {
            m_ManaRemaining = Math.Max(0, m_ManaRemaining - value);
        }

        public void IncreaseMana(int value)
        {
            m_ManaRemaining = Math.Min(m_Mana, m_ManaRemaining + value);
        }

        public void DecreaseComposure(int value)
        {
            m_ComposureRemaining = Math.Max(0, m_ComposureRemaining - value);
        }

        public void IncreaseComposure(int value)
        {
            m_ComposureRemaining = Math.Min(m_Composure, m_ComposureRemaining + value);
        }

        public void DecreaseConcentration(int value)
        {
            m_ConcentrationRemaining = Math.Max(0, m_ConcentrationRemaining - value);
        }

        public void IncreaseConcentration(int value)
        {
            m_ConcentrationRemaining = Math.Min(m_Concentration, m_ConcentrationRemaining + value);
        }

        public void AddExperience(float value)
        {
            m_Experience += value;

            if (m_Experience >= (XP_PER_LEVEL * m_Level))
            {
                m_Level += 1;
                LevelUp();
            }
        }

        public void LevelUp()
        {
            if(m_JobLevels.ContainsKey(m_CurrentJob.name))
            {
                m_JobLevels[m_CurrentJob.name] += 1;
            }
            else
            {
                m_JobLevels.Add(m_CurrentJob.name, 1);
            }

            foreach(Tuple<int, Ability> abilityTuple in m_CurrentJob.abilities)
            {
                if(m_JobLevels[m_CurrentJob.name] == abilityTuple.First)
                {
                    m_Abilities.Add(abilityTuple.Second);
                }
            }

            CalculateDerivatives();
        }

        public void DamageMe(int value, Entity source)
        {
            int damage = value;

            foreach(Ability ability in m_Abilities)
            {
                if (damage == 0)
                    return;

                if(ability.m_AbilityTrigger == AbilityTrigger.OnTakeHit)
                {
                    damage = ability.OnTakeHit(source, this, damage);
                }
            }

            base.DamageMe(damage);
        }

        public void DirectDamage(int value)
        {
            m_HitPointsRemaining -= value;
        }

        public void CalculateDerivatives()
        {
            int lastHP = m_HitPoints;
            int lastConc = m_Concentration;
            int lastComp = m_Composure;
            int lastMana = m_Mana;

            m_HitPoints = (int)(m_Statistics[StatisticIndex.Endurance] * 2);
            m_Concentration = (int)(m_Statistics[StatisticIndex.Focus] * 2);
            m_Composure = (int)(m_Statistics[StatisticIndex.Wit] * 2);
            m_Mana = (int)(m_Statistics[StatisticIndex.Focus] + m_Statistics[StatisticIndex.Endurance] + m_Statistics[StatisticIndex.Wit]) / 3;

            m_HitPointsRemaining += m_HitPoints - lastHP;
            m_ConcentrationRemaining += m_Concentration - lastConc;
            m_ComposureRemaining += m_Composure - lastComp;
            m_ManaRemaining += m_Mana - lastMana;
        }

        public ItemInstance GetEquipment(string slotRef)
        {
            if (m_Equipment.ContainsKey(slotRef))
            {
                if((slotRef.Equals("Hand1") || slotRef.Equals("Hand2")) && m_Equipment[slotRef] == null)
                    return m_NaturalWeapons;
                else
                    return m_Equipment[slotRef];
            }
            return null;
        }

        public void FulfillNeed(NeedIndex need, int value, int minutes = NEED_FULFILMENT_COUNTER)
        {
            m_Needs[need].Fulfill(value);
            m_FulfillingNeed = need;
            m_FulfilmentCounter = minutes;
        }

        public string CreatureType
        {
            get;
            protected set;
        }

        public Gender Gender
        {
            get;
            protected set;
        }

        public Dictionary<int, int> Relationships
        {
            get
            {
                return m_Relationships;
            }
        }

        public Dictionary<string, ItemInstance> Equipment
        {
            get
            {
                return m_Equipment;
            }
        }

        public Dictionary<int, RelationshipStatus> Family
        {
            get
            {
                return m_Family;
            }
        }

        public Dictionary<StatisticIndex, int> Statistics
        {
            get
            {
                return m_Statistics;
            }
        }

        public Dictionary<string, EntitySkill> Skills
        {
            get
            {
                return m_Skills;
            }
        }

        public Dictionary<NeedIndex, EntityNeed> Needs
        {
            get
            {
                return m_Needs;
            }
        }

        public List<Ability> Abilities
        {
            get
            {
                return m_Abilities;
            }
        }

        public string JobName
        {
            get
            {
                return m_CurrentJob.name;
            }
        }

        public bool Sentient
        {
            get
            {
                return m_Sentient;
            }
        }

        public int Size
        {
            get
            {
                return m_Size;
            }
        }
        
        public bool[,] Vision
        {
            get
            {
                return m_Vision;
            }
            set
            {
                m_Vision = value;
            }
        }

        public bool PlayerControlled
        {
            get;
            set;
        }

        public List<ItemInstance> Backpack
        {
            get
            {
                return m_Backpack;
            }
        }

        public List<string> IdentifiedItems
        {
            get
            {
                List<string> returnItems = new List<string>();
                returnItems.AddRange(m_IdentifiedItems);
                return returnItems;
            }
        }

        public bool HasMoved
        {
            get;
            protected set;
        }

        public int FulfilmentCounter
        {
            get
            {
                return m_FulfilmentCounter;
            }
        }

        public NeedIndex FulfillingNeed
        {
            get
            {
                return m_FulfillingNeed;
            }
        }

        public Sexuality Sexuality
        {
            get
            {
                return m_Sexuality;
            }
        }

        public int MatingThreshold
        {
            get
            {
                return 300 - Needs[NeedIndex.Sex].priority;
            }
        }

        public Ability TargetingAbility
        {
            get;
            set;
        }

        public int Mana
        {
            get
            {
                return m_Mana;
            }
        }

        public int ManaRemaining
        {
            get
            {
                return m_ManaRemaining;
            }
        }

        public int ComposureRemaining
        {
            get
            {
                return m_ComposureRemaining;
            }
        }

        public int Composure
        {
            get
            {
                return m_Composure;
            }
        }

        public int Concentration
        {
            get
            {
                return m_Concentration;
            }
        }

        public int ConcentrationRemaining
        {
            get
            {
                return m_ConcentrationRemaining;
            }
        }

        public Vector2Int TargetPoint
        {
            get;
            set;
        }

        protected int RegenTicker
        {
            get;
            set;
        }

        public int Level
        {
            get
            {
                return m_Level;
            }
        }

        public Quest QuestOffered
        {
            get;
            set;
        }

        public CultureType Culture
        {
            get
            {
                return s_Cultures[CreatureType];
            }
        }

        public VisionType VisionType
        {
            get
            {
                return m_VisionType;
            }
        }
        
        public WorldInstance MyWorld
        {
            get;
            set;
        }

        public string Tileset
        {
            get
            {
                return m_Tileset;
            }
        }

        public JobType Job
        {
            get
            {
                return m_CurrentJob;
            }
        }

        public Dictionary<string, int> JobLevels
        {
            get
            {
                return m_JobLevels;
            }
        }

        public List<string> Slots
        {
            get
            {
                return m_Equipment.Keys.ToList();
            }
        }
    }
}
