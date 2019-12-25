using JoyLib.Code.Collections;
using JoyLib.Code.Combat;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.AI.LOS;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Statistics.Formulas;
using JoyLib.Code.Helpers;
using JoyLib.Code.Physics;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
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
        protected BasicValueContainer<IRollableValue> m_Statistics;
        protected BasicValueContainer<IGrowingValue> m_Skills;
        protected BasicValueContainer<INeed> m_Needs;
        protected List<IAbility> m_Abilities;
        protected NonUniqueDictionary<string, ItemInstance> m_Equipment;
        protected List<ItemInstance> m_Backpack;
        protected ItemInstance m_NaturalWeapons;
        protected ISexuality m_Sexuality;

        protected List<string> m_IdentifiedItems;

        protected JobType m_CurrentJob;
        protected Dictionary<string, int> m_JobLevels;

        protected List<string> m_Slots;

        protected List<CultureType> m_Cultures;

        protected int m_Size;

        protected IGrowingValue m_Level;

        protected bool[,] m_Vision;
        protected string m_VisionType;

        protected FulfillmentData m_FulfillmentData;

        protected NeedAIData m_CurrentTarget;

        protected IPathfinder m_Pathfinder;

        protected Queue<Vector2Int> m_PathfindingData;

        protected IFOVHandler m_FOVHandler;

        protected const int XP_PER_LEVEL = 100;
        protected const int NEED_FULFILMENT_COUNTER = 5;
        protected const int REGEN_TICK_TIME = 10;

        protected const int ATTACK_THRESHOLD = -50;

        protected static readonly Vector2Int NO_TARGET = new Vector2Int(-1, -1);

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
        public Entity(
            EntityTemplate template,
            BasicValueContainer<INeed> needs,
            List<CultureType> cultures,
            IGrowingValue level,
            float experience,
            JobType job,
            IBioSex sex,
            ISexuality sexuality,
            Vector2Int position,
            Sprite[] sprites,
            ItemInstance naturalWeapons,
            NonUniqueDictionary<string, ItemInstance> equipment,
            List<ItemInstance> backpack,
            List<string> identifiedItems,
            Dictionary<string, int> jobLevels,
            WorldInstance world) :
            base("",
                EntityDerivedValue.GetDefault(
                    template.Statistics[EntityStatistic.ENDURANCE],
                    template.Statistics[EntityStatistic.FOCUS],
                    template.Statistics[EntityStatistic.WIT]),
                position,
                template.Tileset,
                sprites,
                template.Tags)
        {
            this.CreatureType = template.CreatureType;
            this.m_Slots = template.Slots.ToList();

            this.m_Size = template.Size;

            this.m_JobLevels = jobLevels;
            this.m_Sexuality = sexuality;
            this.m_IdentifiedItems = identifiedItems;
            this.m_Statistics = template.Statistics;

            if (template.Skills.Collection.Count == 0)
            {
                this.m_Skills = EntitySkillHandler.GetDefaultSkillBlock(needs);
            }
            else
            {
                this.m_Skills = template.Skills;
            }
            this.m_Needs = needs;
            this.m_Abilities = template.Abilities.ToList();
            this.m_Level = level;
            for (int i = 1; i < level.Value; i++)
            {
                this.LevelUp();
            }
            this.m_CurrentJob = job;

            if (template.Sentient)
            {
                this.AddTag("sentient");
            }
            else
            {
                this.AddTag("non-sentient");
            }

            this.m_NaturalWeapons = naturalWeapons;
            this.m_Equipment = equipment;
            this.m_Backpack = backpack;
            this.Sex = sex;
            this.m_VisionType = template.VisionType;

            this.m_Cultures = cultures;

            this.CalculateDerivatives();

            this.m_Vision = new bool[1, 1];

            this.m_Pathfinder = ScriptingEngine.GetProvidedPathFinder();
            this.m_PathfindingData = new Queue<Vector2Int>();

            this.m_FulfillmentData = new FulfillmentData(
                "none",
                0,
                new JoyObject[0]);

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
        public Entity(EntityTemplate template, BasicValueContainer<INeed> needs, List<CultureType> cultures, IGrowingValue level, JobType job, IBioSex sex, ISexuality sexuality,
            Vector2Int position, Sprite[] sprites, WorldInstance world) :
            this(template, needs, cultures, level, 0, job, sex, sexuality, position, sprites,
                NaturalWeaponHelper.MakeNaturalWeapon(template.Size), new NonUniqueDictionary<string, ItemInstance>(),
                new List<ItemInstance>(), new List<string>(), new Dictionary<string, int>(), world)
        {
        }

        protected string GetNameFromMultipleCultures()
        {
            List<string> nameList = new List<string>();
            int maxNames = m_Cultures.SelectMany(x => x.NameData)
                                    .SelectMany(y => y.chain)
                                    .Max(z => z);
            for (int i = 0; i <= maxNames; i++)
            {
                CultureType random = m_Cultures[RNG.Roll(0, m_Cultures.Count - 1)];
                while (random.NameData.SelectMany(x => x.chain).Max(y => y) < maxNames)
                {
                    random = m_Cultures[RNG.Roll(0, m_Cultures.Count - 1)];
                }

                nameList.Add(random.GetNameForChain(i, this.Sex.Name));
            }
            return String.Join(" ", nameList);
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
            if (m_FulfillmentData.Counter > 0)
            {
                m_FulfillmentData.DecrementCounter();
            }

            RegenTicker += 1;
            if (RegenTicker == REGEN_TICK_TIME)
            {
                m_DerivedValues[EntityDerivedValue.HITPOINTS].ModifyValue(1);
                m_DerivedValues[EntityDerivedValue.CONCENTRATION].ModifyValue(1);
                m_DerivedValues[EntityDerivedValue.COMPOSURE].ModifyValue(1);
                m_DerivedValues[EntityDerivedValue.MANA].ModifyValue(1);

                RegenTicker = 0;

                foreach (INeed need in m_Needs.Collection)
                {
                    need.Tick();
                }
            }

            UpdateMe();
        }

        public void AddQuest(Quest quest)
        {
            List<QuestStep> steps = quest.steps;
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i].action == QuestAction.Deliver && steps[i].objects.Count != 0)
                {
                    for (int j = 0; j < steps[i].objects.Count; j++)
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

            //Check statistics
            foreach (string tag in tags)
            {
                if (m_Statistics.Has(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Statistics[tag].Value));
                }
            }

            //Check skills
            foreach (string tag in tags)
            {
                if (m_Skills.Has(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Skills[tag].Value));
                }
            }

            //Check needs
            foreach (string tag in tags)
            {
                if (m_Needs.Has(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Needs[tag].Value));
                }
            }

            //Check equipment
            List<ItemInstance> items = m_Equipment.Values;

            foreach (string tag in tags)
            {
                int result = m_Equipment.KeyCount(tag);
                if (result > 0)
                {
                    data.Add(new Tuple<string, int>(tag, result));
                }

                foreach (ItemInstance item in items)
                {
                    if (item.HasTag(tag))
                    {
                        data.Add(new Tuple<string, int>(tag, 1));
                    }
                }
            }

            //Check backpack
            foreach (ItemInstance item in m_Backpack)
            {
                foreach (string tag in tags)
                {
                    if (tag.Equals(item.IdentifiedName) || tag.Equals(item.ItemType))
                    {
                        data.Add(new Tuple<string, int>(tag, 1));
                    }
                }
            }

            return data.ToArray();
        }

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
                    List<INeed> needs = m_Needs.Collection.OrderByDescending(x => x.Priority).ToList();
                    //Act on first need

                    bool idle = true;
                    foreach (INeed need in needs)
                    {
                        if (need.ContributingHappiness)
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
                if (CurrentTarget.searching && CurrentTarget.targetPoint == NO_TARGET)
                {
                    List<Vector2Int> visibleSpots = new List<Vector2Int>();
                    List<Vector2Int> visibleWalls = MyWorld.GetVisibleWalls(this);
                    //Check what we can see
                    for (int x = 0; x < this.Vision.GetLength(0); x++)
                    {
                        for (int y = 0; y < this.Vision.GetLength(0); y++)
                        {
                            Vector2Int newPos = new Vector2Int(x, y);
                            if (CanSee(x, y) && visibleWalls.Contains(newPos) == false && WorldPosition != newPos)
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
                if (WorldPosition != CurrentTarget.targetPoint || (CurrentTarget.target != null && AdjacencyHelper.IsAdjacent(WorldPosition, CurrentTarget.target.WorldPosition)))
                {
                    MoveToTarget(CurrentTarget);
                }
                //If we've arrived at our destination, then we do our thing
                if (WorldPosition == CurrentTarget.targetPoint || (CurrentTarget.target != null && AdjacencyHelper.IsAdjacent(WorldPosition, CurrentTarget.target.WorldPosition)))
                {
                    //If we have a target
                    if (CurrentTarget.target != null)
                    {
                        if (CurrentTarget.intent == Intent.Attack)
                        {
                            //CombatEngine.SwingWeapon(this, CurrentTarget.target);
                        }
                        else if (CurrentTarget.intent == Intent.Interact)
                        {
                            INeed need = this.Needs[CurrentTarget.need];

                            need.Interact(this, CurrentTarget.target);
                        }
                    }
                    //If we do not, we were probably wandering
                    else
                    {
                        if (CurrentTarget.searching == true)
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
                    if (physicsResult != PhysicsResult.EntityCollision && physicsResult != PhysicsResult.WallCollision)
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
                else if (data.targetPoint != NO_TARGET)
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
            if (item == null)
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
            if (m_Backpack.Contains(item))
            {
                m_Backpack.Remove(item);
                return true;
            }
            return false;
        }

        public bool RemoveItemFromPerson(ItemInstance item)
        {
            //Check slots first
            foreach (Tuple<string, ItemInstance> tuple in m_Equipment)
            {
                if (tuple.Item2 == null)
                {
                    continue;
                }

                foreach (string slot in this.Slots)
                {
                    if (tuple.Item2.GUID == item.GUID)
                    {
                        return RemoveEquipment(slot);
                    }
                }
            }
            //Then the backpack
            return RemoveItemFromBackpack(item);
        }

        public bool RemoveEquipment(string slot, ItemInstance item = null)
        {
            foreach (Tuple<string, ItemInstance> tuple in m_Equipment)
            {
                if (tuple.Item1.Equals(slot) && (item != null && tuple.Item2.GUID == item.GUID))
                {
                    m_Equipment.Add(slot, null);
                    m_Equipment.Remove(tuple.Item1, tuple.Item2);
                    return true;
                }
            }
            return false;
        }

        public ItemInstance[] SearchBackpackForItemType(string[] tags)
        {
            try
            {
                List<ItemInstance> matchingItems = new List<ItemInstance>();
                foreach (ItemInstance item in m_Backpack)
                {
                    int matches = 0;
                    foreach (string tag in tags)
                    {
                        if (item.HasTag(tag))
                        {
                            matches++;
                        }
                    }
                    if (matches == tags.Length || (tags.Length < item.TotalTags && matches > 0))
                    {
                        matchingItems.Add(item);
                    }
                }

                return matchingItems.ToArray();
            }
            catch (Exception ex)
            {
                ActionLog.WriteToLog("ERROR WHEN SEARCHING BACKPACK OF " + this.ToString());
                ActionLog.WriteToLog(ex.Message);
                ActionLog.WriteToLog(ex.StackTrace);
                return new List<ItemInstance>().ToArray();
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
                need = need.ToLower()
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

        public bool EquipItem(string slotRef, ItemInstance itemRef)
        {
            bool contains = false;
            Tuple<string, ItemInstance> firstEmptySlot = null;
            foreach (Tuple<string, ItemInstance> tuple in m_Equipment)
            {
                if (tuple.Item1.Equals(slotRef))
                {
                    contains = true;
                    if (tuple.Item2 == null)
                    {
                        firstEmptySlot = tuple;
                    }
                    break;
                }
            }

            if (contains == false)
            {
                return false;
            }

            if (firstEmptySlot != null)
            {
                m_Backpack.Add(firstEmptySlot.Item2);
            }

            m_Equipment.Remove(firstEmptySlot.Item1, firstEmptySlot.Item2);
            m_Equipment.Add(firstEmptySlot.Item1, itemRef);
            m_Backpack.Remove(itemRef);
            return true;
        }

        public bool UnequipItem(string slot)
        {
            foreach (Tuple<string, ItemInstance> tuple in m_Equipment)
            {
                if (tuple.Item1.Equals(slot))
                {
                    AddItem(tuple.Item2);
                    //TODO: Make this better
                    m_Equipment.Remove(tuple.Item1, tuple.Item2);
                    m_Equipment.Add(tuple.Item1, null);
                    return true;
                }
            }
            return false;
        }

        public void DecreaseMana(int value)
        {
            m_DerivedValues[EntityDerivedValue.MANA].ModifyValue(-value);
        }

        public void IncreaseMana(int value)
        {
            m_DerivedValues[EntityDerivedValue.MANA].ModifyValue(value);
        }

        public void DecreaseComposure(int value)
        {
            m_DerivedValues[EntityDerivedValue.COMPOSURE].ModifyValue(-value);
        }

        public void IncreaseComposure(int value)
        {
            m_DerivedValues[EntityDerivedValue.COMPOSURE].ModifyValue(value);
        }

        public void DecreaseConcentration(int value)
        {
            m_DerivedValues[EntityDerivedValue.CONCENTRATION].ModifyValue(-value);
        }

        public void IncreaseConcentration(int value)
        {
            m_DerivedValues[EntityDerivedValue.CONCENTRATION].ModifyValue(value);
        }

        public void AddExperience(float value)
        {
            float previousXP = m_Level.Experience;

            if (previousXP < m_Level.AddExperience(value))
            {
                LevelUp();
            }
        }

        public void LevelUp()
        {
            if (m_JobLevels.ContainsKey(m_CurrentJob.Name))
            {
                m_JobLevels[m_CurrentJob.Name] += 1;
            }
            else
            {
                m_JobLevels.Add(m_CurrentJob.Name, 1);
            }

            IAbility[] newAbilities = m_CurrentJob.GetAbilitiesForLevel(m_Level.Value);

            foreach (IAbility ability in newAbilities)
            {
                if (m_Abilities.Contains(ability) == false)
                {
                    m_Abilities.Add(ability);
                }
            }

            CalculateDerivatives();
        }

        public void DamageMe(int value, Entity source)
        {
            int damage = value;

            foreach (IAbility ability in m_Abilities)
            {
                if (damage == 0)
                    return;

                if (ability.AbilityTrigger == AbilityTrigger.OnTakeHit)
                {
                    damage = ability.OnTakeHit(source, this, damage);
                }
            }

            base.DamageMe(damage);
        }

        public void DirectDVModification(int value, string index = EntityDerivedValue.HITPOINTS)
        {
            m_DerivedValues[index].ModifyValue(value);
        }

        protected void CalculateDerivatives()
        {
            IDerivedValue hitpoints = m_DerivedValues[EntityDerivedValue.HITPOINTS];
            int lastHP = hitpoints.Value;

            IDerivedValue concentration = m_DerivedValues[EntityDerivedValue.CONCENTRATION];
            int lastConc = concentration.Value;

            IDerivedValue composure = m_DerivedValues[EntityDerivedValue.COMPOSURE];
            int lastComp = composure.Value;

            IDerivedValue mana = m_DerivedValues[EntityDerivedValue.MANA];
            int lastMana = mana.Value;

            IValueFormula valueFormula = new BasicDerivedValueFormula();
            IValueFormula manaFormula = new ManaFormula();

            hitpoints.SetMaximum(valueFormula.Calculate(
                new IBasicValue[] { m_Statistics[EntityStatistic.ENDURANCE] }));

            concentration.SetMaximum(valueFormula.Calculate(
                new IBasicValue[] { m_Statistics[EntityStatistic.FOCUS] }));

            composure.SetMaximum(valueFormula.Calculate(
                new IBasicValue[] { m_Statistics[EntityStatistic.WIT] }));

            mana.SetMaximum(valueFormula.Calculate(
                new IBasicValue[] { m_Statistics[EntityStatistic.ENDURANCE],
                                    m_Statistics[EntityStatistic.FOCUS],
                                    m_Statistics[EntityStatistic.WIT] }));

            hitpoints.ModifyValue(hitpoints.Maximum - lastHP);
            concentration.ModifyValue(concentration.Maximum - lastConc);
            composure.ModifyValue(composure.Maximum - lastComp);
            mana.ModifyValue(mana.Maximum - lastMana);
        }

        public ItemInstance GetEquipment(string slotRef)
        {
            foreach (Tuple<string, ItemInstance> tuple in m_Equipment)
            {
                if (tuple.Item1.Equals(slotRef))
                {
                    if (slotRef.StartsWith("Hand") && tuple.Item2 == null)
                    {
                        return m_NaturalWeapons;
                    }
                }
                else
                {
                    return tuple.Item2;
                }
            }
            return null;
        }

        public void FulfillNeed(string need, int value, JoyObject[] targets, int minutes = NEED_FULFILMENT_COUNTER)
        {
            m_Needs[need].Fulfill(value);

            m_FulfillmentData = new FulfillmentData(need, minutes, targets);
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

        public List<Tuple<string, ItemInstance>> Equipment
        {
            get
            {
                return new List<Tuple<string, ItemInstance>>(m_Equipment);
            }
        }

        public BasicValueContainer<IRollableValue> Statistics
        {
            get
            {
                return m_Statistics;
            }
        }

        public BasicValueContainer<IGrowingValue> Skills
        {
            get
            {
                return m_Skills;
            }
        }

        public BasicValueContainer<INeed> Needs
        {
            get
            {
                return m_Needs;
            }
        }

        public IAbility[] Abilities
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
                return m_CurrentJob.Name;
            }
        }

        public bool Sentient
        {
            get
            {
                return m_Tags.Contains("sentient");
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

        public FulfillmentData FulfillmentData
        {
            get
            {
                return m_FulfillmentData;
            }
        }

        public ISexuality Sexuality
        {
            get
            {
                return m_Sexuality;
            }
        }

        public IAbility TargetingAbility
        {
            get;
            set;
        }

        public int Mana
        {
            get
            {
                return m_DerivedValues[EntityDerivedValue.MANA].Maximum;
            }
        }

        public int ManaRemaining
        {
            get
            {
                return m_DerivedValues[EntityDerivedValue.MANA].Value;
            }
        }

        public int ComposureRemaining
        {
            get
            {
                return m_DerivedValues[EntityDerivedValue.COMPOSURE].Value;
            }
        }

        public int Composure
        {
            get
            {
                return m_DerivedValues[EntityDerivedValue.COMPOSURE].Maximum;
            }
        }

        public int Concentration
        {
            get
            {
                return m_DerivedValues[EntityDerivedValue.CONCENTRATION].Maximum;
            }
        }

        public int ConcentrationRemaining
        {
            get
            {
                return m_DerivedValues[EntityDerivedValue.CONCENTRATION].Value;
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
                return m_Level.Value;
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

        public string VisionType
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
                return m_Statistics[EntityStatistic.CUNNING].Value + GlobalConstants.MINIMUM_VISION_DISTANCE;
            }
        }
    }
}
