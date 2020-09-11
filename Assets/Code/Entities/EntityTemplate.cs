using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Collections;
using System.Linq;

namespace JoyLib.Code.Entities
{
    public class EntityTemplate
    {
        protected readonly string m_CreatureType;
        protected readonly string m_Type;

        protected readonly BasicValueContainer<IRollableValue> m_Statistics;
        protected readonly BasicValueContainer<IGrowingValue> m_Skills;
        protected readonly string[] m_Needs;
        protected readonly IAbility[] m_Abilities;
        protected readonly string[] m_Slots;
        protected readonly string[] m_Tags;
        
        protected readonly int m_Size;

        protected readonly bool m_Sentient;

        protected readonly string m_Tileset;

        public EntityTemplate(
            BasicValueContainer<IRollableValue> statistics, 
            BasicValueContainer<IGrowingValue> skills, 
            string[] needs,
            IAbility[] abilities,
            string[] slots, 
            int size, 
            IVision visionType, 
            string creatureType, 
            string type, 
            string tileset, 
            string[] tags)
        {
            m_Statistics = statistics;
            m_Skills = skills;
            m_Abilities = abilities;
            m_Slots = slots;
            m_Needs = needs;

            m_Size = size;

            m_Sentient = tags.Contains("sentient");

            VisionType = visionType;

            m_CreatureType = creatureType;
            m_Type = type;

            m_Tileset = tileset;

            m_Tags = new string[tags.Length];
            for(int i = 0; i < tags.Length; i++)
            {
                m_Tags[i] = tags[i];
            }
        }

        public string[] Tags
        {
            get
            {
                return m_Tags;
            }
        }

        public string[] Slots
        {
            get
            {
                return m_Slots;
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

        public string[] Needs
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

        public string Tileset
        {
            get
            {
                return m_Tileset;
            }
        }
    }
}
