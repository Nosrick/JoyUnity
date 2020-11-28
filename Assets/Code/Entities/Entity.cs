using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Statistics.Formulas;
using JoyLib.Code.Helpers;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Romance;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class Entity : JoyObject, IItemContainer
    {
        protected BasicValueContainer<IRollableValue> m_Statistics;
        protected BasicValueContainer<IGrowingValue> m_Skills;
        protected BasicValueContainer<INeed> m_Needs;
        protected List<IAbility> m_Abilities;
        protected NonUniqueDictionary<string, IItemInstance> m_Equipment;
        protected List<IItemInstance> m_Backpack;
        protected IItemInstance m_NaturalWeapons;
        protected ISexuality m_Sexuality;
        protected IRomance m_Romance;

        protected List<string> m_IdentifiedItems;

        protected IJob m_CurrentJob;
        protected Dictionary<string, int> m_JobLevels;

        protected List<string> m_Slots;

        protected List<ICulture> m_Cultures;

        protected int m_Size;

        protected IGrowingValue m_Level;
        protected IVision m_VisionProvider;

        protected FulfillmentData m_FulfillmentData;

        protected NeedAIData m_CurrentTarget;

        protected IDriver m_Driver;

        protected IPathfinder m_Pathfinder;

        protected Queue<Vector2Int> m_PathfindingData;

        protected const int XP_PER_LEVEL = 100;
        protected const int NEED_FULFILMENT_COUNTER = 5;
        protected const int REGEN_TICK_TIME = 10;

        protected const int ATTACK_THRESHOLD = -50;

        protected static IEntityRelationshipHandler s_RelationshipHandler;
        protected static IEntitySkillHandler s_SkillHandler;
        protected static IQuestTracker QuestTracker { get; set; }

        protected readonly static string[] STANDARD_ACTIONS = new string[]
        {
            "giveitemaction",
            "fulfillneedaction",
            "seekaction",
            "wanderaction",
            "modifyrelationshippointsaction",
            "enterworldaction",
            "additemaction"
        };

        public Entity()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="needs"></param>
        /// <param name="cultures"></param>
        /// <param name="level"></param>
        /// <param name="experience"></param>
        /// <param name="job"></param>
        /// <param name="sex"></param>
        /// <param name="sexuality"></param>
        /// <param name="position"></param>
        /// <param name="sprites"></param>
        /// <param name="naturalWeapons"></param>
        /// <param name="equipment"></param>
        /// <param name="backpack"></param>
        /// <param name="identifiedItems"></param>
        /// <param name="jobLevels"></param>
        /// <param name="world"></param>
        /// <param name="driver"></param>
        public Entity(
            IEntityTemplate template,
            BasicValueContainer<INeed> needs,
            List<ICulture> cultures,
            IGrowingValue level,
            float experience,
            IJob job,
            IGender gender,
            IBioSex sex,
            ISexuality sexuality,
            IRomance romance,
            Vector2Int position,
            Sprite[] sprites,
            IItemInstance naturalWeapons,
            NonUniqueDictionary<string, IItemInstance> equipment,
            List<IItemInstance> backpack,
            List<string> identifiedItems,
            Dictionary<string, int> jobLevels,
            WorldInstance world,
            IDriver driver) :
            base("",
                EntityDerivedValue.GetDefault(
                    template.Statistics[EntityStatistic.ENDURANCE],
                    template.Statistics[EntityStatistic.FOCUS],
                    template.Statistics[EntityStatistic.WIT]),
                position,
                template.Tileset,
                STANDARD_ACTIONS,
                sprites,
                template.Tags.ToArray())
        {
            if (s_RelationshipHandler is null)
            {
                var test = template.Statistics[EntityStatistic.WIT];
                IGameManager gameManager = GlobalConstants.GameManager;
                s_RelationshipHandler = gameManager.RelationshipHandler;
                s_SkillHandler = gameManager.SkillHandler;
                QuestTracker = gameManager.QuestTracker;
            }

            this.CreatureType = template.CreatureType;
            this.m_Slots = template.Slots.ToList();

            this.m_Size = template.Size;

            this.Gender = gender;

            this.m_JobLevels = jobLevels;
            this.Sexuality = sexuality;
            this.Romance = romance;
            this.m_IdentifiedItems = identifiedItems;
            this.m_Statistics = template.Statistics;

            this.m_Skills = new BasicValueContainer<IGrowingValue>();


            this.m_Needs = needs;

            foreach (IGrowingValue skill in template.Skills)
            {
                this.m_Skills.Add(skill);
            }

            foreach (IGrowingValue skill in s_SkillHandler.GetDefaultSkillBlock(this.m_Needs))
            {
                this.m_Skills.Add(skill);
            }

            this.m_Abilities = template.Abilities.ToList();
            this.m_Level = level;
            for (int i = 1; i < level.Value; i++)
            {
                this.LevelUp();
            }

            this.m_CurrentJob = job;

            this.Tags = template.Tags.ToList();

            this.m_NaturalWeapons = naturalWeapons;
            this.m_Equipment = equipment;
            this.m_Backpack = backpack;
            this.Sex = sex;
            this.m_VisionProvider = template.VisionType;

            this.m_Cultures = cultures;

            this.CalculateDerivatives();

            this.m_Pathfinder = (IPathfinder) ScriptingEngine.instance.FetchAndInitialise("custompathfinder");
            this.m_PathfindingData = new Queue<Vector2Int>();

            this.m_FulfillmentData = new FulfillmentData(
                "none",
                0,
                new JoyObject[0]);

            this.RegenTicker = RNG.instance.Roll(0, REGEN_TICK_TIME - 1);

            this.MyWorld = world;
            this.JoyName = this.GetNameFromMultipleCultures();

            this.m_Driver = driver;

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
        public Entity(
            IEntityTemplate template,
            BasicValueContainer<INeed> needs,
            List<ICulture> cultures,
            IGrowingValue level,
            IJob job,
            IGender gender,
            IBioSex sex,
            ISexuality sexuality,
            IRomance romance,
            Vector2Int position,
            Sprite[] sprites,
            WorldInstance world,
            IDriver driver) :
            this(template, needs, cultures, level, 0, job, gender, sex, sexuality, romance, position, sprites,
                NaturalWeaponHelper.MakeNaturalWeapon(template.Size), new NonUniqueDictionary<string, IItemInstance>(),
                new List<IItemInstance>(), new List<string>(), new Dictionary<string, int>(), world, driver)
        {
        }

        protected Entity(Entity copy) :
            base(
                copy.JoyName,
                copy.DerivedValues,
                copy.WorldPosition,
                copy.TileSet,
                copy.CachedActions.ToArray(),
                copy.Sprites,
                copy.Tags.ToArray())
        {
            this.CreatureType = copy.CreatureType;
            this.m_Slots = copy.m_Slots;

            this.m_Size = copy.Size;

            this.m_JobLevels = copy.JobLevels;
            this.m_IdentifiedItems = copy.m_IdentifiedItems;
            this.m_Statistics = copy.Statistics;

            this.m_Skills = copy.Skills;

            this.m_Needs = copy.Needs;
            this.m_Abilities = copy.m_Abilities;
            this.m_Level = copy.m_Level;
            for (int i = 1; i < this.m_Level.Value; i++)
            {
                this.LevelUp();
            }

            this.m_CurrentJob = copy.Job;

            this.m_NaturalWeapons = copy.m_NaturalWeapons;
            this.m_Equipment = copy.m_Equipment;
            this.m_Backpack = copy.m_Backpack;
            this.Gender = copy.Gender;
            this.Sex = copy.Sex;
            this.Sexuality = copy.Sexuality;
            this.Romance = copy.Romance;
            this.m_VisionProvider = copy.m_VisionProvider;

            this.m_Cultures = copy.m_Cultures;

            this.CalculateDerivatives();

            this.m_Pathfinder = (IPathfinder) ScriptingEngine.instance.FetchAndInitialise("custompathfinder");
            this.m_PathfindingData = new Queue<Vector2Int>();

            this.m_FulfillmentData = new FulfillmentData(
                "none",
                0,
                new JoyObject[0]);

            this.RegenTicker = RNG.instance.Roll(0, REGEN_TICK_TIME - 1);

            this.MyWorld = copy.MyWorld;

            this.m_Driver = copy.m_Driver;

            SetCurrentTarget();
        }

        protected string GetNameFromMultipleCultures()
        {
            const int groupChance = 10;
            
            List<string> nameList = new List<string>();
            int maxNames = m_Cultures.SelectMany(x => x.NameData)
                .SelectMany(y => y.chain)
                .Max(z => z);

            int lastGroup = Int32.MinValue;
            for (int i = 0; i <= maxNames; i++)
            {
                ICulture random = m_Cultures[RNG.instance.Roll(0, m_Cultures.Count)];

                while (random.NameData.SelectMany(x => x.chain).Max(y => y) < maxNames)
                {
                    random = m_Cultures[RNG.instance.Roll(0, m_Cultures.Count)];
                }

                if (lastGroup == int.MinValue && RNG.instance.Roll(0, 100) < groupChance)
                {
                    int[] groups = random.NameData.SelectMany(data => data.groups).Distinct().ToArray();

                    if (groups.Length == 0)
                    {
                        lastGroup = Int32.MinValue;
                    }
                    else
                    {
                        lastGroup = groups[RNG.instance.Roll(0, groups.Length)];
                        if (random.NameData.Any(data => random.NameData.SelectMany(d => d.chain)
                                                            .Min(d => d) == i 
                                                && data.groups.Contains(lastGroup)) == false)
                        {
                            lastGroup = Int32.MinValue;
                        }
                    }
                    
                }
                
                nameList.Add(random.GetNameForChain(i, this.Gender.Name, lastGroup));
            }

            m_Cultures.ForEach(culture => culture.ClearLastGroup());
            return String.Join(" ", nameList).Trim();
        }

        protected void SetCurrentTarget()
        {
            m_CurrentTarget.idle = true;
            m_CurrentTarget.intent = Intent.Interact;
            m_CurrentTarget.searching = false;
            m_CurrentTarget.target = null;
            m_CurrentTarget.targetPoint = GlobalConstants.NO_TARGET;
        }

        public void Tick()
        {
            if (m_FulfillmentData.Counter > 0 && m_FulfillmentData.DecrementCounter() == 0)
            {
                MonoBehaviourHandler.SetSpeechBubble(false);
            }

            if (m_FulfillmentData.Counter == 0)
            {
            }

            RegenTicker += 1;
            if (RegenTicker == REGEN_TICK_TIME)
            {
                DerivedValues[EntityDerivedValue.HITPOINTS].ModifyValue(1);
                DerivedValues[EntityDerivedValue.CONCENTRATION].ModifyValue(1);
                DerivedValues[EntityDerivedValue.COMPOSURE].ModifyValue(1);
                DerivedValues[EntityDerivedValue.MANA].ModifyValue(1);

                RegenTicker = 0;

                foreach (INeed need in m_Needs.Collection)
                {
                    need.Tick(this);
                }
            }

            UpdateMe();
        }

        public void AddQuest(IQuest quest)
        {
            quest.StartQuest(this);
            QuestTracker.AddQuest(GUID, quest);
        }

        public Tuple<string, int>[] GetData(string[] tags, params object[] args)
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

            //Fetch all statistics
            if (tags.Any(tag => tag.Equals("statistics", StringComparison.OrdinalIgnoreCase)))
            {
                data.AddRange(m_Statistics.Select(statistic =>
                    new Tuple<string, int>(statistic.Name, statistic.Value)));
            }

            //Check skills
            foreach (string tag in tags)
            {
                if (m_Skills.Has(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Skills[tag].Value));
                }
            }

            //Fetch all skills
            if (tags.Any(tag => tag.Equals("skills", StringComparison.OrdinalIgnoreCase)))
            {
                data.AddRange(from EntitySkill skill in m_Skills
                    select new Tuple<string, int>(skill.Name, skill.Value));
            }

            //Check needs
            foreach (string tag in tags)
            {
                if (m_Needs.Has(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Needs[tag].Value));
                }
            }

            //Fetch all needs
            if (tags.Any(tag => tag.Equals("needs", StringComparison.OrdinalIgnoreCase)))
            {
                data.AddRange(from INeed need in m_Needs select new Tuple<string, int>(need.Name, need.Value));
            }

            //Check equipment
            List<IItemInstance> items = m_Equipment.Values;

            foreach (string tag in tags)
            {
                int result = m_Equipment.KeyCount(tag);
                if (result > 0)
                {
                    data.Add(new Tuple<string, int>(tag, result));
                }

                result = items.Count(item => item.HasTag(tag));

                if (result > 0)
                {
                    data.Add(new Tuple<string, int>(tag, result));
                }
            }

            //Check backpack
            foreach (string tag in tags)
            {
                int identifiedNames = m_Backpack.Count(item =>
                    item.IdentifiedName.Equals(tag, StringComparison.OrdinalIgnoreCase));

                int unidentifiedNames = m_Backpack.Count(item =>
                    item.ItemType.UnidentifiedName.Equals(tag, StringComparison.OrdinalIgnoreCase));

                if (identifiedNames > 0)
                {
                    data.Add(new Tuple<string, int>(tag, identifiedNames));
                }

                if (unidentifiedNames > 0)
                {
                    data.Add(new Tuple<string, int>(tag, unidentifiedNames));
                }
            }

            //Check job levels
            foreach (string tag in tags)
            {
                KeyValuePair<string, int> jobLevel = new KeyValuePair<string, int>(tag, 0);

                try
                {
                    jobLevel = m_JobLevels.First(job => job.Key.Equals(tag, StringComparison.OrdinalIgnoreCase));
                    data.Add(new Tuple<string, int>(jobLevel.Key, jobLevel.Value));
                }
                catch (Exception e)
                {
                    //suppress this
                }
            }

            //Fetch all job levels
            if (tags.Any(tag => tag.Equals("jobs", StringComparison.OrdinalIgnoreCase)))
            {
                data.AddRange(from int level in m_JobLevels select new Tuple<string, int>("jobs", level));
            }

            //Fetch gender data
            if (tags.Any(tag => tag.Equals(this.Gender.Name, StringComparison.OrdinalIgnoreCase))
                || tags.Any(tag => tag.Equals("gender", StringComparison.OrdinalIgnoreCase)))
            {
                data.Add(new Tuple<string, int>(this.Gender.Name, 1));
            }

            //Fetch sex data
            if (tags.Any(tag => tag.Equals(this.Sex.Name, StringComparison.OrdinalIgnoreCase))
                || tags.Any(tag => tag.Equals("sex")))
            {
                data.Add(new Tuple<string, int>(this.Sex.Name, 1));
            }

            if (tags.Any(tag => tag.Equals("can birth", StringComparison.OrdinalIgnoreCase)))
            {
                data.Add(new Tuple<string, int>("can birth", this.Sex.CanBirth == true ? 1 : 0));
            }

            //Fetch sexuality data
            if (tags.Any(tag => tag.Equals(this.Sexuality.Name, StringComparison.OrdinalIgnoreCase))
                || tags.Any(tag => tag.Equals("sexuality", StringComparison.OrdinalIgnoreCase)))
            {
                data.Add(new Tuple<string, int>(this.Sexuality.Name, 1));
            }

            //Fetch romance data
            if (tags.Any(tag => tag.Equals(this.Romance.Name, StringComparison.OrdinalIgnoreCase))
                || tags.Any(tag => tag.Equals("romance", StringComparison.OrdinalIgnoreCase)))
            {
                data.Add(new Tuple<string, int>(this.Romance.Name, 1));
            }

            if (args is null || args.Length <= 0)
            {
                return data.ToArray();
            }

            foreach (object obj in args)
            {
                if (!(obj is Entity other))
                {
                    continue;
                }

                IRelationship[] relationships = s_RelationshipHandler.GetAllForObject(this);

                if (tags.Any(tag => tag.Equals("will mate", StringComparison.OrdinalIgnoreCase)))
                {
                    data.Add(new Tuple<string, int>(
                        other.JoyName,
                        this.Sexuality.WillMateWith(this, other, relationships) == true ? 1 : 0));
                }

                if (tags.Any(tag => tag.Equals("will romance", StringComparison.OrdinalIgnoreCase)))
                {
                    data.Add(new Tuple<string, int>(
                        other.JoyName,
                        this.Romance.Compatible(this, other, relationships) == true ? 1 : 0));
                }

                //Check relationships
                foreach (IRelationship relationship in relationships)
                {
                    foreach (string tag in tags)
                    {
                        if (relationship.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                        {
                            int relationshipValue = relationship.GetRelationshipValue(this.GUID, other.GUID);
                            data.Add(new Tuple<string, int>(tag, 1));
                            data.Add(new Tuple<string, int>("relationship", relationshipValue));
                        }
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
            HasMoved = false;

            VisionProvider.Update(this, this.MyWorld);
            m_Driver.Locomotion(this);
        }

        public void SetPath(Queue<Vector2Int> pointsRef)
        {
            m_PathfindingData = pointsRef;
        }

        public bool IsViableMate(Entity entityRef, IRelationship[] relationships)
        {
            return m_Sexuality.WillMateWith(this, entityRef, relationships);
        }

        public void AddIdentifiedItem(string nameRef)
        {
            m_IdentifiedItems.Add(nameRef);
        }

        public virtual bool RemoveContents(IItemInstance item)
        {
            if (m_Backpack.Contains(item))
            {
                m_Backpack.Remove(item);
                return true;
            }

            return false;
        }

        public virtual bool RemoveItemFromPerson(IItemInstance item)
        {
            //Check slots first
            foreach (Tuple<string, IItemInstance> tuple in m_Equipment)
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
            return RemoveContents(item);
        }

        public virtual bool RemoveEquipment(string slot, IItemInstance item = null)
        {
            foreach (Tuple<string, IItemInstance> tuple in m_Equipment)
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

        public IItemInstance[] SearchBackpackForItemType(string[] tags)
        {
            try
            {
                List<IItemInstance> matchingItems = new List<IItemInstance>();
                foreach (IItemInstance item in m_Backpack)
                {
                    int matches = 0;
                    foreach (string tag in tags)
                    {
                        if (item.HasTag(tag))
                        {
                            matches++;
                        }
                    }

                    if (matches == tags.Length || (tags.Length < item.Tags.Count && matches > 0))
                    {
                        matchingItems.Add(item);
                    }
                }

                return matchingItems.ToArray();
            }
            catch (Exception ex)
            {
                ActionLog.instance.AddText("ERROR WHEN SEARCHING BACKPACK OF " + this.ToString());
                ActionLog.instance.AddText(ex.Message);
                ActionLog.instance.AddText(ex.StackTrace);
                return new List<IItemInstance>().ToArray();
            }
        }

        public void PlaceItemInWorld(IItemInstance item)
        {
            m_Backpack.Remove(item);
            item.Move(WorldPosition);
            MyWorld.AddObject(item);
        }

        public virtual bool EquipItem(string slotRef, IItemInstance itemRef)
        {
            bool contains = false;
            Tuple<string, IItemInstance> firstEmptySlot = null;
            foreach (Tuple<string, IItemInstance> tuple in m_Equipment)
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

        public virtual bool UnequipItem(string slot)
        {
            foreach (Tuple<string, IItemInstance> tuple in m_Equipment)
            {
                if (tuple.Item1.Equals(slot))
                {
                    AddContents(tuple.Item2);
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
            DerivedValues[EntityDerivedValue.MANA].ModifyValue(-value);
        }

        public void IncreaseMana(int value)
        {
            DerivedValues[EntityDerivedValue.MANA].ModifyValue(value);
        }

        public void DecreaseComposure(int value)
        {
            DerivedValues[EntityDerivedValue.COMPOSURE].ModifyValue(-value);
        }

        public void IncreaseComposure(int value)
        {
            DerivedValues[EntityDerivedValue.COMPOSURE].ModifyValue(value);
        }

        public void DecreaseConcentration(int value)
        {
            DerivedValues[EntityDerivedValue.CONCENTRATION].ModifyValue(-value);
        }

        public void IncreaseConcentration(int value)
        {
            DerivedValues[EntityDerivedValue.CONCENTRATION].ModifyValue(value);
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

            base.DamageValue(EntityDerivedValue.HITPOINTS, damage);
        }

        public void DirectDVModification(int value, string index = EntityDerivedValue.HITPOINTS)
        {
            DerivedValues[index].ModifyValue(value);
        }

        protected void CalculateDerivatives()
        {
            IDerivedValue hitpoints = DerivedValues[EntityDerivedValue.HITPOINTS];
            int lastHP = hitpoints.Value;

            IDerivedValue concentration = DerivedValues[EntityDerivedValue.CONCENTRATION];
            int lastConc = concentration.Value;

            IDerivedValue composure = DerivedValues[EntityDerivedValue.COMPOSURE];
            int lastComp = composure.Value;

            IDerivedValue mana = DerivedValues[EntityDerivedValue.MANA];
            int lastMana = mana.Value;

            IValueFormula valueFormula = new BasicDerivedValueFormula();
            IValueFormula manaFormula = new ManaFormula();

            hitpoints.SetMaximum(valueFormula.Calculate(
                new IBasicValue[] {m_Statistics[EntityStatistic.ENDURANCE]}));

            concentration.SetMaximum(valueFormula.Calculate(
                new IBasicValue[] {m_Statistics[EntityStatistic.FOCUS]}));

            composure.SetMaximum(valueFormula.Calculate(
                new IBasicValue[] {m_Statistics[EntityStatistic.WIT]}));

            mana.SetMaximum(valueFormula.Calculate(
                new IBasicValue[]
                {
                    m_Statistics[EntityStatistic.ENDURANCE],
                    m_Statistics[EntityStatistic.FOCUS],
                    m_Statistics[EntityStatistic.WIT]
                }));

            hitpoints.ModifyValue(hitpoints.Maximum - lastHP);
            concentration.ModifyValue(concentration.Maximum - lastConc);
            composure.ModifyValue(composure.Maximum - lastComp);
            mana.ModifyValue(mana.Maximum - lastMana);
        }

        public IItemInstance GetEquipment(string slotRef)
        {
            foreach (Tuple<string, IItemInstance> tuple in m_Equipment)
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

        public bool PerformAction(IJoyAction action, IJoyObject[] participants, string[] tags = null,
            params object[] args)
        {
            ActionLog.instance.AddText(this.JoyName + "is performing " + action.ActionString);
            return action.Execute(participants, tags, args);
        }

        public List<IItemInstance> Contents
        {
            get
            {
                return m_Backpack;
            }
        }

        public virtual bool AddContents(IItemInstance actor)
        {
            if (m_IdentifiedItems.Any(i => i.Equals(actor.JoyName, StringComparison.OrdinalIgnoreCase)))
            {
                actor.IdentifyMe();
            }

            m_Backpack.Add(actor);
            return true;
        }

        public virtual bool AddContents(IEnumerable<IItemInstance> actors)
        {
            foreach (ItemInstance actor in actors)
            {
                if (m_IdentifiedItems.Any(i => i.Equals(actor.JoyName, StringComparison.OrdinalIgnoreCase)))
                {
                    actor.IdentifyMe();
                }
            }

            m_Backpack.AddRange(actors);
            return true;
        }

        public virtual void Clear()
        {
            m_Backpack.Clear();
        }

        public string ContentString { get; }

        public string CreatureType { get; protected set; }

        public IBioSex Sex { get; protected set; }

        public IGender Gender { get; protected set; }

        public NeedAIData CurrentTarget
        {
            get { return m_CurrentTarget; }
            set { m_CurrentTarget = value; }
        }

        public NonUniqueDictionary<string, IItemInstance> Equipment
        {
            get { return new NonUniqueDictionary<string, IItemInstance>(m_Equipment); }
        }

        public BasicValueContainer<IRollableValue> Statistics
        {
            get { return m_Statistics; }
        }

        public BasicValueContainer<IGrowingValue> Skills
        {
            get { return m_Skills; }
        }

        public BasicValueContainer<INeed> Needs
        {
            get { return m_Needs; }
        }

        public IAbility[] Abilities
        {
            get { return m_Abilities.ToArray(); }
        }

        public string JobName
        {
            get { return m_CurrentJob.Name; }
        }

        public bool Sentient
        {
            get { return Tags.Any(tag => tag.Equals("sentient", StringComparison.OrdinalIgnoreCase)); }
        }

        public int Size
        {
            get { return m_Size; }
        }

        public bool[,] Vision
        {
            get { return m_VisionProvider.Vision; }
        }

        public bool PlayerControlled { get; set; }

        public IItemInstance[] Backpack
        {
            get { return m_Backpack.ToArray(); }
        }

        public string[] IdentifiedItems
        {
            get { return m_IdentifiedItems.ToArray(); }
        }

        public bool HasMoved { get; set; }

        public FulfillmentData FulfillmentData
        {
            get => m_FulfillmentData;
            set
            {
                m_FulfillmentData = value;
                if (m_FulfillmentData.Name.Equals("none", StringComparison.OrdinalIgnoreCase) == false &&
                    m_FulfillmentData.Name.IsNullOrEmpty() == false)
                {
                    MonoBehaviourHandler.SetSpeechBubble(m_FulfillmentData.Counter > 0,
                        m_Needs[m_FulfillmentData.Name].FulfillingSprite);
                }
            }
        }

        public ISexuality Sexuality
        {
            get => m_Sexuality;
            set => m_Sexuality = value;
        }

        public IRomance Romance
        {
            get => m_Romance;
            set => m_Romance = value;
        }

        public IAbility TargetingAbility { get; set; }

        public int Mana
        {
            get { return DerivedValues[EntityDerivedValue.MANA].Maximum; }
        }

        public int ManaRemaining
        {
            get { return DerivedValues[EntityDerivedValue.MANA].Value; }
        }

        public int ComposureRemaining
        {
            get { return DerivedValues[EntityDerivedValue.COMPOSURE].Value; }
        }

        public int Composure
        {
            get { return DerivedValues[EntityDerivedValue.COMPOSURE].Maximum; }
        }

        public int Concentration
        {
            get { return DerivedValues[EntityDerivedValue.CONCENTRATION].Maximum; }
        }

        public int ConcentrationRemaining
        {
            get { return DerivedValues[EntityDerivedValue.CONCENTRATION].Value; }
        }

        public Vector2Int TargetPoint { get; set; }

        protected int RegenTicker { get; set; }

        public int Level
        {
            get { return m_Level.Value; }
        }

        public Quest QuestOffered { get; set; }

        public ICulture[] Cultures => m_Cultures.ToArray();

        public IVision VisionProvider => m_VisionProvider;

        public IJob Job
        {
            get { return m_CurrentJob; }
        }

        public Dictionary<string, int> JobLevels
        {
            get { return m_JobLevels; }
        }

        public string[] Slots
        {
            get { return m_Slots.ToArray(); }
        }

        public int VisionMod
        {
            get { return m_Statistics[EntityStatistic.CUNNING].Value + GlobalConstants.MINIMUM_VISION_DISTANCE; }
        }

        public Queue<Vector2Int> PathfindingData
        {
            get { return m_PathfindingData; }
            set { m_PathfindingData = value; }
        }

        public IPathfinder Pathfinder
        {
            get { return m_Pathfinder; }
        }
    }
}