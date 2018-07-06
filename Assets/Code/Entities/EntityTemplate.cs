using JoyLib.Code.Entities.Abilities;
using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public class EntityTemplate
    {
        protected readonly string m_CreatureType;
        protected readonly string m_Type;

        protected readonly Dictionary<StatisticIndex, int> m_Statistics;
        protected readonly Dictionary<string, EntitySkill> m_Skills;
        protected readonly List<Ability> m_Abilities;
        
        protected readonly int m_Size;

        protected readonly bool m_Sentient;
        protected readonly VisionType m_VisionType;

        protected readonly string m_Tileset;

        public EntityTemplate(Dictionary<StatisticIndex, int> statistics, Dictionary<string, EntitySkill> skills, List<Ability> abilities, int size, bool sentient, VisionType visionType,
            string creatureType, string type, string tileset)
        {
            m_Statistics = statistics;
            m_Skills = skills;
            m_Abilities = abilities;

            m_Size = size;

            m_Sentient = sentient;

            m_VisionType = visionType;

            m_CreatureType = creatureType;
            m_Type = type;

            m_Tileset = tileset;
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

        public List<Ability> Abilities
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

        public VisionType VisionType
        {
            get
            {
                return m_VisionType;
            }
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

        public string Tileset
        {
            get
            {
                return m_Tileset;
            }
        }
    }
}
