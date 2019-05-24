﻿using JoyLib.Code.Combat;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.AI.LOS;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Helpers;
using JoyLib.Code.Physics;
using JoyLib.Code.Quests;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class Entity : JoyObject
    {
        protected Dictionary<string, EntityStatistic> m_Statistics;
        protected Dictionary<string, EntitySkill> m_Skills;
        protected Dictionary<string, INeed> m_Needs;
        protected List<Ability> m_Abilities;
        protected Dictionary<string, ItemInstance> m_Equipment;
        protected List<ItemInstance> m_Backpack;
        protected ItemInstance m_NaturalWeapons;
        protected ISexuality m_Sexuality;

        protected List<string> m_IdentifiedItems;

        //This is going to be handled by the EntityRelationshipHandler
        //protected List<IRelationship> m_Relationships;

        protected JobType m_CurrentJob;
        protected Dictionary<string, int> m_JobLevels;

        protected List<string> m_Slots;

        protected List<CultureType> m_Cultures;

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

        protected string m_FulfillingNeed;
        protected int m_FulfilmentCounter;

        protected string m_Tileset;

        protected NeedAIData m_CurrentTarget;

        protected IPathfinder m_Pathfinder;
        
        protected Queue<Vector2Int> m_PathfindingData;

        protected IFOVHandler m_FOVHandler;

        protected const int XP_PER_LEVEL = 100;
        protected const int NEED_FULFILMENT_COUNTER = 5;
        protected const int REGEN_TICK_TIME = 10;

        protected const int ATTACK_THRESHOLD = -50;

        protected readonly Vector2Int NO_TARGET = new Vector2Int(-1, -1);

        /// <summary>
        /// Create an entity with job levels, equipment, family, etc
        /// </summary>
        /// <param name="template"></param>
        /// <param name="needs"></param>
        /// <param name="level"></param>
        /// <param name="experience"></param>
        /// <param name="job"></param>
        /// <param name="sex"></param>
        /// <param name="sexuality"></param>
        /// <param name="position"></param>
        /// <param name="icons"></param>
        /// <param name="naturalWeapons"></param>
        /// <param name="equipment"></param>
        /// <param name="backpack"></param>
        /// <param name="relationships"></param>
        /// <param name="identifiedItems"></param>
        /// <param name="family"></param>
        /// <param name="jobLevels"></param>
        /// <param name="world"></param>
        /// <param name="tileset"></param>
        public Entity(EntityTemplate template, Dictionary<string, INeed> needs, List<CultureType> cultures, int level, float experience, JobType job, IBioSex sex, ISexuality sexuality,
            Vector2Int position, List<Sprite> sprites, ItemInstance naturalWeapons, Dictionary<string, ItemInstance> equipment, 
            List<ItemInstance> backpack, List<string> identifiedItems, Dictionary<string, int> jobLevels, 
            WorldInstance world, string tileset) : 
            base("", 1, position, sprites, template.JoyType, true)
        {
            this.CreatureType = template.CreatureType;
            this.m_Slots = template.Slots;

            this.m_Size = template.Size;

            this.m_JobLevels = jobLevels;
            this.m_Sexuality = sexuality;
            this.m_IdentifiedItems = identifiedItems;
            this.m_Statistics = template.Statistics;

            if (template.Skills.Count == 0)
            {
                this.m_Skills = EntitySkillHandler.GetSkillBlock(needs);
            }
            else
            {
                this.m_Skills = template.Skills;
            }
            this.m_Needs = needs;
            this.m_Abilities = template.Abilities;
            this.m_Level = level;
            for(int i = 1; i < level; i++)
            {
                this.LevelUp();
            }
            this.m_Experience = experience;
            this.m_CurrentJob = job;
            this.m_Sentient = template.Sentient;
            this.m_NaturalWeapons = naturalWeapons;
            this.m_Equipment = equipment;
            this.m_Backpack = backpack;
            this.Sex = sex;
            this.m_VisionType = template.VisionType;

            this.m_Cultures = cultures;

            this.m_Tileset = tileset;

            this.CalculateDerivatives();

            this.m_Vision = new bool[1, 1];

            this.m_Pathfinder = ScriptingEngine.GetProvidedPathFinder();
            this.m_PathfindingData = new Queue<Vector2Int>();

            this.m_FulfillingNeed = "NONE";
            this.m_FulfilmentCounter = 0;

            this.RegenTicker = RNG.Roll(0, REGEN_TICK_TIME - 1);

            this.MyWorld = world;
            this.JoyName = this.GetNameFromMultipleCultures();

            SetFOVHandler();
            SetCurrentTarget();
        }

        /// <summary>
        /// Create a new entity, naked and squirming
        /// Created with no equipment, knowledge, family, etc
        /// </summary>
        /// <param name="template"></param>
        /// <param name="needs"></param>
        /// <param name="level"></param>
        /// <param name="job"></param>
        /// <param name="sex"></param>
        /// <param name="sexuality"></param>
        /// <param name="position"></param>
        /// <param name="icons"></param>
        /// <param name="world"></param>
        public Entity(EntityTemplate template, Dictionary<string, INeed> needs, List<CultureType> cultures, int level, JobType job, IBioSex sex, ISexuality sexuality,
            Vector2Int position, List<Sprite> sprites, WorldInstance world) :
            base("", 1, position, sprites, template.JoyType, true)
        {
            this.CreatureType = template.CreatureType;

            this.m_Size = template.Size;
            this.m_Slots = template.Slots;

            this.m_JobLevels = new Dictionary<string, int>();
            this.m_Sexuality = sexuality;
            this.m_IdentifiedItems = new List<string>();
            this.m_Statistics = template.Statistics;

            if (template.Skills.Count == 0)
            {
                this.m_Skills = EntitySkillHandler.GetSkillBlock(needs);
            }
            else
            {
                this.m_Skills = template.Skills;
            }
            this.m_Needs = needs;
            this.m_Abilities = template.Abilities;
            this.m_Level = level;
            for (int i = 1; i < level; i++)
            {
                this.LevelUp();
            }
            this.m_Experience = 0;
            this.m_CurrentJob = job;
            this.m_Sentient = template.Sentient;
            this.m_NaturalWeapons = NaturalWeaponHelper.MakeNaturalWeapon(template.Size);
            this.m_Equipment = new Dictionary<string, ItemInstance>();
            this.m_Backpack = new List<ItemInstance>();
            this.Sex = sex;
            this.m_VisionType = template.VisionType;

            this.m_Tileset = template.Tileset;

            this.m_Cultures = cultures;

            this.CalculateDerivatives();

            this.m_Vision = new bool[1, 1];

            this.m_Pathfinder = new CustomPathfinder(); //ScriptingEngine.GetProvidedPathFinder();
            this.m_PathfindingData = new Queue<Vector2Int>();

            this.m_FulfillingNeed = "NONE";
            this.m_FulfilmentCounter = 0;

            this.RegenTicker = RNG.Roll(0, REGEN_TICK_TIME - 1);

            this.MyWorld = world;

            this.JoyName = this.GetNameFromMultipleCultures();
            SetFOVHandler();
            SetCurrentTarget();
        }

        protected string GetNameFromMultipleCultures()
        {
            string name = "";
            for(int i = 0; i < m_Cultures.Count; i++)
            {
                name += m_Cultures[i].GetNameForChain(i, this.Sex.Name) + " ";
            }
            name.TrimEnd();
            return name;
        }

        protected void SetFOVHandler()
        {
            m_FOVHandler = new FOVShadowCasting();
            //m_FOVHandler = new FOVPermissive();
            //m_FOVHandler = new FOVRecursiveShadowcasting();
            //m_FOVHandler = new FOVDirty();
        }

        protected void SetCurrentTarget()
        {
            m_CurrentTarget.idle = true;
            m_CurrentTarget.intent = Intent.Interact;
            m_CurrentTarget.searching = false;
            m_CurrentTarget.target = null;
            m_CurrentTarget.targetPoint = NO_TARGET;
        }

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
            {
                m_FulfilmentCounter -= 1;
            }

            RegenTicker += 1;
            if(RegenTicker == REGEN_TICK_TIME)
            {
                m_HitPointsRemaining = Math.Min(m_HitPoints, m_HitPointsRemaining + 1);
                m_ConcentrationRemaining = Math.Min(m_Concentration, m_ConcentrationRemaining + 1);
                m_ComposureRemaining = Math.Min(m_Composure, m_ComposureRemaining + 1);
                m_ManaRemaining = Math.Min(m_Mana, m_ManaRemaining + 1);
                RegenTicker = 0;

                foreach(INeed need in m_Needs.Values)
                {
                    need.Tick();
                }
            }

            UpdateMe();
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

        public Tuple<string, int>[] GetData(string[] tags)
        {
            List<Tuple<string, int>> data = new List<Tuple<string, int>>();
            List<string> tempTags = new List<string>(tags);

            //Check statistics
            foreach(string tag in tempTags)
            {
                if(m_Statistics.ContainsKey(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Statistics[tag].Value));
                    tempTags.Remove(tag);
                }
            }

            //Check skills
            foreach(string tag in tags)
            {
                if(m_Skills.ContainsKey(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Skills[tag].value));
                    tempTags.Remove(tag);
                }
            }

            //Check needs
            foreach(string tag in tags)
            {
                if(m_Needs.ContainsKey(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Needs[tag].Value));
                    tempTags.Remove(tag);
                }
            }

            //Check equipment
            foreach (ItemInstance item in m_Equipment.Values)
            {
                foreach (string tag in tags)
                {
                    if (tag.Equals(item.IdentifiedName) || tag.Equals(item.ItemType))
                    {
                        data.Add(new Tuple<string, int>(tag, 1));
                        tempTags.Remove(tag);
                    }
                }
            }

            //Check backpack
            foreach(ItemInstance item in m_Backpack)
            {
                foreach(string tag in tags)
                {
                    if(tag.Equals(item.IdentifiedName) || tag.Equals(item.ItemType))
                    {
                        data.Add(new Tuple<string, int>(tag, 1));
                        tempTags.Remove(tag);
                    }
                }
            }

            return data.ToArray();
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

        public override void Update()
        {
            base.Update();
        }

        public void UpdateMe()
        {
            if (this.MyWorld.IsDirty)
            {
                FOVBasicBoard board = (FOVBasicBoard)m_FOVHandler.Do(this.WorldPosition, this.MyWorld.Dimensions, this.VisionMod, this.MyWorld.Walls.Keys.ToList());
                m_Vision = board.Vision;
            }
            else
            {
                FOVBasicBoard board = (FOVBasicBoard)m_FOVHandler.Do(this.WorldPosition, this.VisionMod);
                m_Vision = board.Vision;
            }

            HasMoved = false;

            if (!PlayerControlled)
            {
                //If you're idle
                if (CurrentTarget.idle == true)
                {
                    //Let's find something to do
                    List<INeed> needs = m_Needs.Values.OrderByDescending(x => x.Priority).ToList();
                    //Act on first need

                    bool idle = true;
                    foreach(INeed need in needs)
                    {
                        if(need.ContributingHappiness)
                        {
                            continue;
                        }
                        
                        need.FindFulfilmentObject(this);
                        idle = false;
                        break;
                    }
                    m_CurrentTarget.idle = idle;
                }
                
                //If we're wandering, select a point we can see and wander there
                if(CurrentTarget.searching && CurrentTarget.targetPoint == NO_TARGET)
                {
                    List<Vector2Int> visibleSpots = new List<Vector2Int>();
                    List<Vector2Int> visibleWalls = MyWorld.GetVisibleWalls(this);
                    //Check what we can see
                    for(int x = 0; x < this.Vision.GetLength(0); x++)
                    {
                        for(int y = 0; y < this.Vision.GetLength(0); y++)
                        {
                            Vector2Int newPos = new Vector2Int(x, y);
                            if(CanSee(x, y) && visibleWalls.Contains(newPos) == false && WorldPosition != newPos)
                            {
                                visibleSpots.Add(newPos);
                            }
                        }
                    }

                    //Pick a random spot to wander to
                    int result = RNG.Roll(0, visibleSpots.Count - 1);
                    m_CurrentTarget.targetPoint = visibleSpots[result];
                }

                //If we have somewhere to be, move there
                if(WorldPosition != CurrentTarget.targetPoint || (CurrentTarget.target != null && AdjacencyHelper.IsAdjacent(WorldPosition, CurrentTarget.target.WorldPosition)))
                {
                    MoveToTarget(CurrentTarget);
                }
                //If we've arrived at our destination, then we do our thing
                if(WorldPosition == CurrentTarget.targetPoint || (CurrentTarget.target != null && AdjacencyHelper.IsAdjacent(WorldPosition, CurrentTarget.target.WorldPosition)))
                {
                    //If we have a target
                    if(CurrentTarget.target != null)
                    {
                        if(CurrentTarget.intent == Intent.Attack)
                        {
                            CombatEngine.SwingWeapon(this, CurrentTarget.target);
                        }
                        else if(CurrentTarget.intent == Intent.Interact)
                        {
                            INeed need = this.Needs[CurrentTarget.need];

                            need.Interact(this, CurrentTarget.target);
                        }
                    }
                    //If we do not, we were probably wandering
                    else
                    {
                        if(CurrentTarget.searching == true)
                        {
                            m_CurrentTarget.targetPoint = NO_TARGET;

                            //Set idle to true so we look for stuff when we arrive
                            m_CurrentTarget.idle = true;
                        }
                    }
                }
            }
            else
            {
                if (!HasMoved && m_PathfindingData.Count > 0)
                {
                    Vector2Int nextPoint = m_PathfindingData.Peek();
                    PhysicsResult physicsResult = PhysicsManager.IsCollision(WorldPosition, nextPoint, MyWorld);
                    if(physicsResult != PhysicsResult.EntityCollision)
                    {
                        m_PathfindingData.Dequeue();
                        Move(nextPoint);
                        HasMoved = true;
                    }
                    else if(physicsResult == PhysicsResult.EntityCollision)
                    {
                        MyWorld.SwapPosition(this, MyWorld.GetEntity(nextPoint));

                        m_PathfindingData.Dequeue();
                        Move(nextPoint);
                        HasMoved = true;
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
                }
                else if (physicsResult == PhysicsResult.EntityCollision)
                {
                    MyWorld.SwapPosition(this, MyWorld.GetEntity(nextPoint));
                    m_PathfindingData.Dequeue();
                    Move(nextPoint);
                    HasMoved = true;
                }
            }
            else if (m_PathfindingData.Count == 0)
            {
                if (data.target != null)
                {
                    m_PathfindingData = m_Pathfinder.FindPath(WorldPosition, data.target.WorldPosition, MyWorld.Costs, GetFullVisionRect());
                }
                else if(data.targetPoint != NO_TARGET)
                {
                    m_PathfindingData = m_Pathfinder.FindPath(WorldPosition, data.targetPoint, MyWorld.Costs, GetFullVisionRect());
                }
            }
        }

        public bool CanSee(int x, int y)
        {
            return this.Vision[x, y];
        }

        public bool CanSee(Vector2Int point)
        {
            return CanSee(point.x, point.y);
        }

        protected RectInt GetVisionRect()
        {
            RectInt visionRect = new RectInt(this.WorldPosition, new Vector2Int(this.VisionMod, this.VisionMod));
            return visionRect;
        }

        protected RectInt GetFullVisionRect()
        {
            RectInt visionRect = new RectInt(0, 0, this.Vision.GetLength(0), this.Vision.GetLength(1));
            return visionRect;
        }

        public void SetPath(Queue<Vector2Int> pointsRef)
        {
            m_PathfindingData = pointsRef;
        }

        public bool IsViableMate(Entity entityRef)
        {
            return m_Sexuality.WillMateWith(this, entityRef);
        }

        public void AddItem(ItemInstance itemRef)
        {
            ItemInstance item = itemRef;
            if(item == null)
            {
                return;
            }

            if (m_IdentifiedItems.Contains(item.JoyName))
            {
                item.IdentifyMe();
                m_IdentifiedItems.Add(item.JoyName);
            }

            m_Backpack.Add(item);
        }

        public void AddIdentifiedItem(string nameRef)
        {
            m_IdentifiedItems.Add(nameRef);
        }

        public bool RemoveItemFromBackpack(ItemInstance item)
        {
            if(m_Backpack.Contains(item))
            {
                m_Backpack.Remove(item);
                return true;
            }
            return false;
        }

        public bool RemoveItemFromPerson(ItemInstance item)
        {
            //Check slots first
            foreach(string slot in this.Slots)
            {
                if(m_Equipment[slot].GUID == item.GUID)
                {
                    return RemoveEquipment(slot);
                }
            }
            //Then the backpack
            return RemoveItemFromBackpack(item);
        }

        public bool RemoveEquipment(string slot)
        {
            if (m_Equipment.ContainsKey(slot))
            {
                m_Equipment[slot] = null;
                return true;
            }
            return false;
        }

        public List<ItemInstance> SearchBackpackForItemType(string itemType)
        {
            try
            {
                return Backpack.Where(item => item.BaseType == itemType).ToList();
            }
            catch(Exception ex)
            {
                return new List<ItemInstance>();
            }
        }

        public void Seek(JoyObject obj, string need)
        {
            NeedAIData needAIData = new NeedAIData
            {
                intent = Intent.Interact,
                searching = false,
                target = obj,
                targetPoint = new Vector2Int(-1, -1),
                need = need
            };

            this.CurrentTarget = needAIData;
        }

        public void Wander()
        {
            NeedAIData needAIData = new NeedAIData
            {
                idle = false,
                intent = Intent.Interact,
                searching = true,
                targetPoint = NO_TARGET
            };

            this.CurrentTarget = needAIData;
        }

        public void PlaceItemInWorld(ItemInstance item)
        {
            m_Backpack.Remove(item);
            item.Move(WorldPosition);
            MyWorld.AddObject(item);
        }

        public bool EquipItem(ItemInstance itemRef, string slotRef)
        {
            if (!m_Equipment.ContainsKey(slotRef))
            {
                return false;
            }

            if (m_Equipment[slotRef] != null)
            {
                m_Backpack.Add(m_Equipment[slotRef]);
            }

            m_Equipment[slotRef] = itemRef;
            m_Backpack.Remove(itemRef);
            return true;
        }

        public void UnequipItem(string slot)
        {
            if (m_Equipment.ContainsKey(slot))
            {
                AddItem(m_Equipment[slot]);
                m_Equipment[slot] = null;
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

        protected void CalculateDerivatives()
        {
            int lastHP = m_HitPoints;
            int lastConc = m_Concentration;
            int lastComp = m_Composure;
            int lastMana = m_Mana;

            m_HitPoints = (m_Statistics["Endurance"].Value * 3);
            m_Concentration = (m_Statistics["Focus"].Value * 3);
            m_Composure = (m_Statistics["Wit"].Value * 3);
            m_Mana = (m_Statistics["Focus"].Value + m_Statistics["Endurance"].Value + m_Statistics["Wit"].Value);

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

        public void FulfillNeed(string need, int value, int minutes = NEED_FULFILMENT_COUNTER)
        {
            m_Needs[need].Fulfill(value);
            m_FulfillingNeed = need;
            m_FulfilmentCounter = minutes;
            ActionLog.AddText(this.ToString() + " is fulfilling need " + need);
        }

        public string CreatureType
        {
            get;
            protected set;
        }

        public IBioSex Sex
        {
            get;
            protected set;
        }

        public NeedAIData CurrentTarget
        {
            get
            {
                return m_CurrentTarget;
            }
            set
            {
                m_CurrentTarget = value;
            }
        }

        public Dictionary<string, ItemInstance> Equipment
        {
            get
            {
                return m_Equipment;
            }
        }

        public Dictionary<string, EntityStatistic> Statistics
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

        public Dictionary<string, INeed> Needs
        {
            get
            {
                return m_Needs;
            }
        }

        public Ability[] Abilities
        {
            get
            {
                return m_Abilities.ToArray();
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

        public ItemInstance[] Backpack
        {
            get
            {
                return m_Backpack.ToArray();
            }
        }

        public string[] IdentifiedItems
        {
            get
            {
                return m_IdentifiedItems.ToArray();
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

        public string FulfillingNeed
        {
            get
            {
                return m_FulfillingNeed;
            }
        }

        public ISexuality Sexuality
        {
            get
            {
                return m_Sexuality;
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

        public CultureType[] Cultures
        {
            get
            {
                return m_Cultures.ToArray();
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

        public string[] Slots
        {
            get
            {
                return m_Slots.ToArray();
            }
        }
        public int VisionMod
        {
            get
            {
                return m_Statistics["Cunning"].Value + GlobalConstants.MINIMUM_VISION_DISTANCE;
            }
        }
    }
}
