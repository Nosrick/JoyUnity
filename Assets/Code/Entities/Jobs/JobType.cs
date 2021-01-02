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
            if(this.m_SkillDiscounts.ContainsKey(skillName))
            {
                return this.m_SkillDiscounts[skillName];
            }
            return 0;
        }

        public int GetStatisticDiscount(string statisticName)
        {
            if(this.m_StatisticDiscounts.ContainsKey(statisticName))
            {
                return this.m_StatisticDiscounts[statisticName];
            }
            return 0;
        }

        public int AddExperience(int value)
        {
            this.Experience += value;
            return this.Experience;
        }

        public bool SpendExperience(int value)
        {
            if (this.Experience < value)
            {
                return false;
            }

            this.Experience -= value;
            return true;
        }

        public IJob Copy(IJob original)
        {
            IJob job = new JobType(
                original.Name,
                original.Description,
                original.StatisticDiscounts,
                original.SkillDiscounts,
                original.Abilities);

            return job;
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
                return this.m_StatisticDiscounts;
            }
        }

        public Dictionary<string, int> SkillDiscounts
        {
            get
            {
                return this.m_SkillDiscounts.ToDictionary(k => k.Key, v => v.Value);
            }
        }

        public Dictionary<IAbility, int> Abilities
        {
            get => this.m_Abilities;
            protected set => this.m_Abilities = value;
        }
    }
}
