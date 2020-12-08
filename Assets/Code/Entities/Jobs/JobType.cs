using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Abilities;

namespace JoyLib.Code.Entities.Jobs
{
    public class JobType : IJob
    {
        protected Dictionary<IAbility, int> m_Abilities;
        protected Dictionary<string, int> m_StatisticDiscounts;
        protected Dictionary<string, int> m_SkillDiscounts;

        public JobType()
        {
        }

        public JobType(
            string name, 
            string description, 
            Dictionary<string, int> statDiscounts, 
            Dictionary<string, int> skillDiscounts,
            Dictionary<IAbility, int> abilities)
        {
            this.Name = name;
            this.Description = description;
            this.Abilities = abilities;
            this.m_StatisticDiscounts = statDiscounts;
            this.m_SkillDiscounts = skillDiscounts;
        }

        public int GetSkillDiscount(string skillName)
        {
            if(m_SkillDiscounts.ContainsKey(skillName))
            {
                return m_SkillDiscounts[skillName];
            }
            return 0;
        }

        public float GetStatisticDiscount(string statisticName)
        {
            if(m_StatisticDiscounts.ContainsKey(statisticName))
            {
                return m_StatisticDiscounts[statisticName];
            }
            return 0.0f;
        }

        public int AddExperience(int value)
        {
            Experience += value;
            return Experience;
        }

        public bool SpendExperience(int value)
        {
            if (Experience < value)
            {
                return false;
            }

            Experience -= value;
            return true;
        }

        public string Name
        {
            get;
            protected set;
        }

        public string Description
        {
            get;
            protected set;
        }

        public int Experience { get; protected set; }

        public Dictionary<string, int> StatisticDiscounts
        {
            get
            {
                return m_StatisticDiscounts;
            }
        }

        public Dictionary<string, int> SkillDiscounts
        {
            get
            {
                return m_SkillDiscounts.ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public Dictionary<IAbility, int> Abilities
        {
            get => m_Abilities;
            protected set => m_Abilities = value;
        }
    }
}
