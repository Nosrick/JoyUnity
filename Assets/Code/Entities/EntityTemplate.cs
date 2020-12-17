using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.Entities
{
    public class EntityTemplate : IEntityTemplate
    {
        protected readonly string m_CreatureType;
        protected readonly string m_Type;

        protected readonly Dictionary<string, IRollableValue<int>> m_Statistics;
        protected readonly Dictionary<string, IEntitySkill> m_Skills;
        protected readonly string[] m_Needs;
        protected readonly IAbility[] m_Abilities;
        protected readonly string[] m_Slots;
        protected readonly HashSet<string> m_Tags;
        
        protected readonly int m_Size;

        protected readonly bool m_Sentient;

        public EntityTemplate(
            Dictionary<string, IRollableValue<int>> statistics, 
            Dictionary<string, IEntitySkill> skills, 
            string[] needs,
            IAbility[] abilities,
            string[] slots, 
            int size, 
            IVision visionType, 
            string creatureType, 
            string type, 
            string[] tags)
        {
            m_Statistics = statistics;
            m_Skills = skills;
            m_Abilities = abilities;
            m_Slots = slots;
            m_Needs = needs;

            m_Size = size;

            m_Sentient = tags.Any(tag => tag.Equals("sentient", StringComparison.OrdinalIgnoreCase));

            VisionType = visionType;

            m_CreatureType = creatureType;
            m_Type = type;

            m_Tags = new HashSet<string>();
            for(int i = 0; i < tags.Length; i++)
            {
                m_Tags.Add(tags[i]);
            }
        }

        public IEnumerable<string> Tags
        {
            get
            {
                return new List<string>(m_Tags);
            }
        }

        public bool HasTag(string tag)
        {
            return m_Tags.Contains(tag);
        }

        public bool AddTag(string tag)
        {
            return m_Tags.Add(tag);
        }

        public bool RemoveTag(string tag)
        {
            return m_Tags.Remove(tag);
        }

        public IEnumerable<string> Slots
        {
            get
            {
                return m_Slots;
            }
        }

        public IDictionary<string, IRollableValue<int>> Statistics
        {
            get
            {
                IDictionary<string, IRollableValue<int>> stats = new Dictionary<string, IRollableValue<int>>();
                foreach (KeyValuePair<string, IRollableValue<int>> stat in this.m_Statistics)
                {
                    stats.Add(new KeyValuePair<string, IRollableValue<int>>(
                        ObjectExtensions.Copy(stat.Key), 
                        ObjectExtensions.Copy(stat.Value)));
                }

                return stats;
            }
        }

        public IDictionary<string, IEntitySkill> Skills
        {
            get
            {
                IDictionary<string, IEntitySkill> skills = new Dictionary<string, IEntitySkill>();
                foreach (KeyValuePair<string, IEntitySkill> skill in this.m_Skills)
                {
                    skills.Add(new KeyValuePair<string, IEntitySkill>(
                        ObjectExtensions.Copy(skill.Key),
                        ObjectExtensions.Copy(skill.Value)));
                }
                return skills;
            }
        }

        public IEnumerable<string> Needs
        {
            get
            {
                return m_Needs;
            }
        }

        public IEnumerable<IAbility> Abilities
        {
            get
            {
                return m_Abilities;
            }
        }

        public int Size
        {
            get
            {
                return m_Size;
            }
        }

        public bool Sentient
        {
            get
            {
                return m_Sentient;
            }
        }

        public IVision VisionType
        {
            get;
            protected set;
        }

        public string CreatureType
        {
            get
            {
                return m_CreatureType;
            }
        }

        public string JoyType
        {
            get
            {
                return m_Type;
            }
        }
    }
}
