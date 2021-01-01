using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using JoyLib.Code.Helpers;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class Entity : JoyObject, IEntity
    {
        public event ValueChangedEventHandler DerivedValueChange;
        public event ValueChangedEventHandler DerivedValueMaximumChange;
        public event ValueChangedEventHandler StatisticChange;
        public event ValueChangedEventHandler SkillChange;
        public event ValueChangedEventHandler ExperienceChange;
        public event JobChangedEventHandler JobChange;
        protected IDictionary<string, IRollableValue<int>> m_Statistics;
        protected IDictionary<string, IEntitySkill> m_Skills;
        protected IDictionary<string, INeed> m_Needs;
        protected List<IAbility> m_Abilities;
        protected EquipmentStorage m_Equipment;
        protected List<IItemInstance> m_Backpack;
        protected IItemInstance m_NaturalWeapons;
        protected ISexuality m_Sexuality;
        protected IRomance m_Romance;

        protected List<string> m_IdentifiedItems;

        protected IJob m_CurrentJob;

        protected List<string> m_Slots;

        protected List<ICulture> m_Cultures;

        protected int m_Size;

        protected IVision m_VisionProvider;

        protected FulfillmentData m_FulfillmentData;

        protected NeedAIData m_CurrentTarget;

        protected IDriver m_Driver;

        protected IPathfinder m_Pathfinder;

        protected Queue<Vector2Int> m_PathfindingData;

        protected IWorldInstance m_MyWorld;

        protected const int XP_PER_LEVEL = 100;
        protected const int NEED_FULFILMENT_COUNTER = 5;
        protected const int REGEN_TICK_TIME = 10;

        protected const int ATTACK_THRESHOLD = -50;

        public static IEntityRelationshipHandler RelationshipHandler { get; set; }
        public static IEntitySkillHandler SkillHandler { get; set; }
        public static IQuestTracker QuestTracker { get; set; }
        public static NaturalWeaponHelper NaturalWeaponHelper { get; set; }

        protected readonly static string[] STANDARD_ACTIONS = new string[]
        {
            "giveitemaction",
            "fulfillneedaction",
            "seekaction",
            "wanderaction",
            "modifyrelationshippointsaction",
            "enterworldaction",
            "additemaction",
            "placeiteminworldaction"
        };

        public Entity()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="statistics"></param>
        /// <param name="derivedValues"></param>
        /// <param name="needs"></param>
        /// <param name="skills"></param>
        /// <param name="abilities"></param>
        /// <param name="cultures"></param>
        /// <param name="job"></param>
        /// <param name="gender"></param>
        /// <param name="sex"></param>
        /// <param name="sexuality"></param>
        /// <param name="romance"></param>
        /// <param name="position"></param>
        /// <param name="sprites"></param>
        /// <param name="naturalWeapons"></param>
        /// <param name="equipment"></param>
        /// <param name="backpack"></param>
        /// <param name="identifiedItems"></param>
        /// <param name="jobs"></param>
        /// <param name="world"></param>
        /// <param name="driver"></param>
        /// <param name="roller"></param>
        /// <param name="jobLevels"></param>
        public Entity(
            IEntityTemplate template,
            IDictionary<string, IRollableValue<int>> statistics,
            IDictionary<string, IDerivedValue> derivedValues,
            IDictionary<string, INeed> needs,
            IDictionary<string, IEntitySkill> skills,
            IEnumerable<IAbility> abilities,
            List<ICulture> cultures,
            IJob job,
            IGender gender,
            IBioSex sex,
            ISexuality sexuality,
            IRomance romance,
            Vector2Int position,
            Sprite[] sprites,
            IItemInstance naturalWeapons,
            EquipmentStorage equipment,
            List<IItemInstance> backpack,
            List<string> identifiedItems,
            IEnumerable<IJob> jobs,
            IWorldInstance world,
            IDriver driver,
            RNG roller = null) :
            base("",
                derivedValues,
                position,
                cultures[GlobalConstants.GameManager.Roller.Roll(0, cultures.Count)].Tileset,
                STANDARD_ACTIONS,
                sprites,
                roller,
                template.Tags.ToArray())
        {
            this.CreatureType = template.CreatureType;
            this.m_Slots = template.Slots.ToList();

            this.m_Size = template.Size;

            this.Gender = gender;

            this.Jobs = new List<IJob>(jobs);
            this.Sexuality = sexuality;
            this.Romance = romance;
            this.m_IdentifiedItems = identifiedItems;
            this.m_Statistics = statistics;
            
            this.m_Needs = needs;

            this.m_Skills = skills;

            this.m_Abilities = template.Abilities.ToList();
            this.m_Abilities.AddRange(abilities);

            this.m_CurrentJob = job;

            this.Tags = template.Tags.ToList();

            this.m_NaturalWeapons = naturalWeapons;
            this.m_Equipment = equipment;
            this.m_Backpack = backpack;
            this.Sex = sex;
            this.m_VisionProvider = template.VisionType;

            this.m_Cultures = cultures;

            this.m_Pathfinder = (IPathfinder) ScriptingEngine.instance.FetchAndInitialise("custompathfinder");
            this.m_PathfindingData = new Queue<Vector2Int>();

            this.m_FulfillmentData = new FulfillmentData(
                "none",
                0,
                new JoyObject[0]);

            this.RegenTicker = Roller.Roll(0, REGEN_TICK_TIME);

            this.MyWorld = world;
            this.JoyName = this.GetNameFromMultipleCultures();

            this.m_Driver = driver;
            this.PlayerControlled = driver.PlayerControlled;

            this.SetCurrentTarget();
            this.ConstructDescription();
        }

        /// <summary>
        /// Create a new entity, naked and squirming
        /// Created with no equipment, knowledge, family, etc
        /// </summary>
        /// <param name="template"></param>
        /// <param name="needs"></param>
        /// <param name="statistics"></param>
        /// <param name="derivedValues"></param>
        /// <param name="skills"></param>
        /// <param name="abilities"></param>
        /// <param name="cultures"></param>
        /// <param name="job"></param>
        /// <param name="gender"></param>
        /// <param name="sex"></param>
        /// <param name="sexuality"></param>
        /// <param name="romance"></param>
        /// <param name="position"></param>
        /// <param name="sprites"></param>
        /// <param name="world"></param>
        /// <param name="driver"></param>
        /// <param name="roller"></param>
        /// <param name="icons"></param>
        public Entity(
            IEntityTemplate template,
            IDictionary<string, IRollableValue<int>> statistics,
            IDictionary<string, IDerivedValue> derivedValues,
            IDictionary<string, INeed> needs,
            IDictionary<string, IEntitySkill> skills,
            IEnumerable<IAbility> abilities,
            List<ICulture> cultures,
            IJob job,
            IGender gender,
            IBioSex sex,
            ISexuality sexuality,
            IRomance romance,
            Vector2Int position,
            Sprite[] sprites,
            IWorldInstance world,
            IDriver driver,
            RNG roller = null) :
            this(template, statistics, derivedValues, needs, skills, abilities, cultures, job, gender, sex, sexuality, romance, position, sprites,
                NaturalWeaponHelper?.MakeNaturalWeapon(template.Size), new EquipmentStorage(template.Slots),
                new List<IItemInstance>(), new List<string>(), new List<IJob> { job }, world, driver, roller)
        {
        }

        protected IEnumerable<Tuple<string, string>> ConstructDescription()
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            Tuple<string, string> relationship = null;
            if (this.PlayerControlled)
            {
                relationship = new Tuple<string, string>("", "This is You");
            }
            else
            {
                string relationshipName = "Stranger";
                try
                {
                    relationshipName = RelationshipHandler.GetBestRelationship(this, GlobalConstants.GameManager.Player)
                        .Name;
                }
                catch (Exception e)
                {
                }
                relationship = new Tuple<string, string>(
                    "Relationship:",
                    relationshipName);
            }

            List<Tuple<string, string>> data = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Species:", textInfo.ToTitleCase(this.CreatureType)),
                new Tuple<string, string>("Job:", textInfo.ToTitleCase(this.CurrentJob.Name)),
                new Tuple<string, string>("Gender:", textInfo.ToTitleCase(this.Gender.Name)),
                relationship
            };

            return data;
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
                ICulture random = m_Cultures[Roller.Roll(0, m_Cultures.Count)];

                while (random.NameData.SelectMany(x => x.chain).Max(y => y) < maxNames)
                {
                    random = m_Cultures[Roller.Roll(0, m_Cultures.Count)];
                }

                if (lastGroup == int.MinValue && Roller.Roll(0, 100) < groupChance)
                {
                    int[] groups = random.NameData.SelectMany(data => data.groups).Distinct().ToArray();

                    if (groups.Length == 0)
                    {
                        lastGroup = Int32.MinValue;
                    }
                    else
                    {
                        lastGroup = groups[Roller.Roll(0, groups.Length)];
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
                this.ModifyValue(DerivedValueName.HITPOINTS, 1);
                this.ModifyValue(DerivedValueName.CONCENTRATION, 1);
                this.ModifyValue(DerivedValueName.COMPOSURE, 1);
                this.ModifyValue(DerivedValueName.MANA, 1);

                RegenTicker = 0;

                foreach (INeed need in m_Needs.Values)
                {
                    need.Tick(this);
                }
            }

            UpdateMe();
        }

        public void AddQuest(IQuest quest)
        {
            quest.StartQuest(this);
            QuestTracker?.AddQuest(GUID, quest);
        }

        public bool AddJob(IJob job)
        {
            if (this.Jobs.Any(j => j.Name.Equals(job.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            this.Jobs.Add(job);
            return true;
        }

        public bool ChangeJob(string job)
        {
            if (this.Jobs.Any(j => j.Name.Equals(job, StringComparison.OrdinalIgnoreCase)))
            {
                this.m_CurrentJob = this.Jobs.First(j => j.Name.Equals(job, StringComparison.OrdinalIgnoreCase));
                this.JobChange?.Invoke(this, new JobChangedEventArgs()
                {
                    GUID = this.GUID,
                    NewJob = this.m_CurrentJob
                });
                return true;
            }

            return false;
        }

        public bool ChangeJob(IJob job)
        {
            if (this.Jobs.Any(j => j.Name.Equals(job.Name, StringComparison.OrdinalIgnoreCase)) == false)
            {
                this.Jobs.Add(job);
            }
            this.m_CurrentJob = job;
            this.JobChange?.Invoke(this, new JobChangedEventArgs()
            {
                GUID = this.GUID,
                NewJob = this.m_CurrentJob
            });

            return true;
        }

        public IEnumerable<Tuple<string, int>> GetData(IEnumerable<string> tags, params object[] args)
        {
            List<Tuple<string, int>> data = new List<Tuple<string, int>>();

            //Check statistics
            foreach (string tag in tags)
            {
                if (m_Statistics.ContainsKey(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Statistics[tag].Value));
                }
            }

            //Fetch all statistics
            if (tags.Any(tag => tag.Equals("statistics", StringComparison.OrdinalIgnoreCase)))
            {
                data.AddRange(m_Statistics.Select(pair =>
                    new Tuple<string, int>(pair.Key, pair.Value.Value)));
            }

            //Check skills
            foreach (string tag in tags)
            {
                if (m_Skills.ContainsKey(tag))
                {
                    data.Add(new Tuple<string, int>(tag, m_Skills[tag].Value));
                }
            }

            //Fetch all skills
            if (tags.Any(tag => tag.Equals("skills", StringComparison.OrdinalIgnoreCase)))
            {
                data.AddRange(from IRollableValue<int> skill in m_Skills.Values
                    select new Tuple<string, int>(skill.Name, skill.Value));
            }

            //Check needs
            foreach (string tag in tags)
            {
                if (m_Needs.ContainsKey(tag))
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
            IEnumerable<IItemInstance> items = m_Equipment.Contents;

            foreach (string tag in tags)
            {
                int result = this.m_Equipment.Contents.Count(item => item.HasTag(tag));
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

            //Check jobs
            foreach (string tag in tags)
            {
                try
                {
                    IJob job = Jobs.First(j => j.Name.Equals(tag, StringComparison.OrdinalIgnoreCase));
                    if (job is null == false)
                    {
                        data.Add(new Tuple<string, int>(job.Name, 1));
                    }
                }
                catch (Exception e)
                {
                    //suppress this
                }
            }

            //Fetch all job levels
            if (tags.Any(tag => tag.Equals("jobs", StringComparison.OrdinalIgnoreCase)))
            {
                data.AddRange(from job in Jobs select new Tuple<string, int>(job.Name, 1));
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

                IEnumerable<IRelationship> relationships = RelationshipHandler?.GetAllForObject(this);

                if (relationships.IsNullOrEmpty())
                {
                    return data.ToArray();
                }

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
                        this.Romance.WillRomance(this, other, relationships) == true ? 1 : 0));
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

        public void AddIdentifiedItem(string nameRef)
        {
            m_IdentifiedItems.Add(nameRef);
        }

        public new void Move(Vector2Int position)
        {
            base.Move(position);
            foreach (IJoyObject joyObject in Backpack)
            {
                joyObject.Move(position);
            }
        }

        public virtual bool RemoveContents(IItemInstance item)
        {
            if (m_Backpack.Contains(item))
            {
                m_Backpack.Remove(item);
                this.ItemRemoved?.Invoke(this, new ItemChangedEventArgs(){ Item = item });
                return true;
            }

            return false;
        }

        public virtual bool RemoveItemFromPerson(IItemInstance item)
        {
            //Check slots first
            bool result = this.Equipment.RemoveContents(item);

            //Then the backpack
            result |=  RemoveContents(item);
            return result;
        }

        public virtual bool RemoveEquipment(IItemInstance item)
        {
            return this.Equipment.RemoveContents(item);
        }

        public IItemInstance[] SearchBackpackForItemType(IEnumerable<string> tags)
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

                    if (matches > 0)
                    {
                        matchingItems.Add(item);
                    }
                }

                return matchingItems.ToArray();
            }
            catch (Exception ex)
            {
                GlobalConstants.ActionLog.AddText("ERROR WHEN SEARCHING BACKPACK OF " + this.ToString());
                GlobalConstants.ActionLog.AddText(ex.Message);
                GlobalConstants.ActionLog.AddText(ex.StackTrace);
                return new List<IItemInstance>().ToArray();
            }
        }

        public virtual bool EquipItem(IItemInstance itemRef)
        {
            if (!this.Equipment.CanAddContents(itemRef))
            {
                return false;
            }

            return this.Equipment.AddContents(itemRef);
        }

        public virtual bool UnequipItem(IItemInstance actor)
        {
            return this.Equipment.Contains(actor) && this.Equipment.RemoveContents(actor);
        }

        public void DecreaseMana(int value)
        {
            this.ModifyValue(DerivedValueName.MANA, -value);
        }

        public void IncreaseMana(int value)
        {
            this.ModifyValue(DerivedValueName.MANA, value);
        }

        public void DecreaseComposure(int value)
        {
            this.ModifyValue(DerivedValueName.COMPOSURE, -value);
        }

        public void IncreaseComposure(int value)
        {
            this.ModifyValue(DerivedValueName.COMPOSURE, value);
        }

        public void DecreaseConcentration(int value)
        {
            this.ModifyValue(DerivedValueName.CONCENTRATION, -value);
        }

        public void IncreaseConcentration(int value)
        {
            this.ModifyValue(DerivedValueName.CONCENTRATION, value);
        }

        public override int ModifyValue(string name, int value)
        {
            int result = base.ModifyValue(name, value);
            this.DerivedValueChange?.Invoke(this, new ValueChangedEventArgs()
            {
                Delta = value,
                Name = name,
                NewValue = result
            });
            return result;
        }

        public void AddExperience(int value)
        {
            int result = this.CurrentJob.AddExperience(value);
            this.ExperienceChange?.Invoke(this, new ValueChangedEventArgs()
            {
                Delta = value,
                Name = "experience",
                NewValue = result
            });
        }

        public void DamageMe(int value, Entity source)
        {
            int damage = value;

            int result = base.DamageValue(DerivedValueName.HITPOINTS, damage);
            this.DerivedValueChange?.Invoke(this, new ValueChangedEventArgs()
            {
                Delta = damage,
                Name = DerivedValueName.HITPOINTS,
                NewValue = result
            });
        }

        public IEnumerable<IItemInstance> GetEquipment(string slotRef)
        {
            return this.Equipment.GetSlotContents(slotRef);
        }

        public IEnumerable<IItemInstance> Contents
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

            actor.MyWorld = this.MyWorld;
            actor.Move(this.WorldPosition);

            if (actor is ItemInstance goItem)
            {
                goItem.MonoBehaviourHandler.gameObject.SetActive(false);
            }

            if (this.m_Backpack.Contains(actor) == false)
            {
                m_Backpack.Add(actor);
            }
            
            this.ItemAdded?.Invoke(this, new ItemChangedEventArgs(){ Item = actor });
            return true;
        }

        public virtual bool CanAddContents(IItemInstance actor)
        {
            return true;
        }

        public virtual bool Contains(IItemInstance actor)
        {
            bool result = false;
            result |= this.Backpack.Contains(actor);
            if (result)
            {
                return result;
            }

            foreach (IItemInstance item in this.Backpack)
            {
                result |= item.Contains(actor);
                if (result)
                {
                    return result;
                }
            }

            return result;
        }

        public virtual bool AddContents(IEnumerable<IItemInstance> actors)
        {
            foreach (IItemInstance actor in actors)
            {
                if (m_IdentifiedItems.Any(i => i.Equals(actor.JoyName, StringComparison.OrdinalIgnoreCase)))
                {
                    actor.IdentifyMe();
                }
            }

            this.m_Backpack.AddRange(
                actors.Where(actor => this.m_Backpack.Any(item => item.GUID == actor.GUID) == false));
            foreach (IItemInstance actor in actors)
            {
                this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
            }
            return true;
        }

        public virtual void Clear()
        {
            m_Backpack.Clear();
        }

        public string ContentString { get; }
        public event ItemRemovedEventHandler ItemRemoved;
        public event ItemAddedEventHandler ItemAdded;

        public string CreatureType { get; protected set; }

        public IBioSex Sex { get; protected set; }

        public IGender Gender { get; protected set; }

        public NeedAIData CurrentTarget
        {
            get { return m_CurrentTarget; }
            set { m_CurrentTarget = value; }
        }

        public IDriver Driver => m_Driver;

        public EquipmentStorage Equipment => this.m_Equipment;

        public IDictionary<string, IRollableValue<int>> Statistics
        {
            get { return m_Statistics; }
        }

        public IDictionary<string, IEntitySkill> Skills
        {
            get { return m_Skills; }
        }

        public IDictionary<string, INeed> Needs
        {
            get { return m_Needs; }
        }

        public List<IAbility> Abilities => m_Abilities;

        public string JobName
        {
            get { return m_CurrentJob.Name; }
        }

        public bool Sentient
        {
            get { return this.Tags.Any(tag => tag.Equals("sentient", StringComparison.OrdinalIgnoreCase)); }
        }

        public int Size
        {
            get { return m_Size; }
        }

        public HashSet<Vector2Int> Vision
        {
            get { return m_VisionProvider.Vision; }
        }

        public bool PlayerControlled { get; set; }

        public List<IItemInstance> Backpack => m_Backpack;

        public IItemInstance NaturalWeapons => m_NaturalWeapons;

        public List<string> IdentifiedItems => m_IdentifiedItems;
        public IJob CurrentJob => m_CurrentJob;

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
            get { return this.DerivedValues[DerivedValueName.MANA].Maximum; }
        }

        public int ManaRemaining
        {
            get { return this.DerivedValues[DerivedValueName.MANA].Value; }
        }

        public int ComposureRemaining
        {
            get { return this.DerivedValues[DerivedValueName.COMPOSURE].Value; }
        }

        public int Composure
        {
            get { return this.DerivedValues[DerivedValueName.COMPOSURE].Maximum; }
        }

        public int Concentration
        {
            get { return this.DerivedValues[DerivedValueName.CONCENTRATION].Maximum; }
        }

        public int ConcentrationRemaining
        {
            get { return this.DerivedValues[DerivedValueName.CONCENTRATION].Value; }
        }

        public Vector2Int TargetPoint { get; set; }

        protected int RegenTicker { get; set; }

        public Quest QuestOffered { get; set; }

        public List<ICulture> Cultures => m_Cultures;

        public IVision VisionProvider => m_VisionProvider;

        public List<string> Slots => m_Slots;

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

        public new IWorldInstance MyWorld
        {
            get => m_MyWorld;
            set
            {
                m_MyWorld = value;
                foreach (IItemInstance item in Backpack)
                {
                    item.MyWorld = value;
                }
            }
        }

        public List<IJob> Jobs { get; protected set; }

        public override IEnumerable<Tuple<string, string>> Tooltip => this.ConstructDescription();
    }
}