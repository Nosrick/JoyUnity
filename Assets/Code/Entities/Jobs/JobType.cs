using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Abilities;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Jobs
{
    public class JobType
    {
        protected NonUniqueDictionary<int, IAbility> m_Abilities;
        protected Dictionary<string, float> m_StatisticGrowths;
        protected Dictionary<string, int> m_SkillGrowths;

        public JobType(string name, string description, Dictionary<string, float> statGrowths, Dictionary<string, int> skillGrowths,
            NonUniqueDictionary<int, IAbility> abilities)
        {
            this.Name = name;
            this.Description = description;
            this.m_Abilities = abilities;
            this.m_StatisticGrowths = statGrowths;
            this.m_SkillGrowths = skillGrowths;
        }

        public int GetSkillGrowth(string skillName)
        {
            if(m_SkillGrowths.ContainsKey(skillName))
            {
                return m_SkillGrowths[skillName];
            }
            return 0;
        }

        public float GetStatisticGrowth(string statisticName)
        {
            if(m_StatisticGrowths.ContainsKey(statisticName))
            {
                return m_StatisticGrowths[statisticName];
            }
            return 0.0f;
        }

        public IAbility[] GetAbilitiesForLevel(int level)
        {
            List<IAbility> abilities = new List<IAbility>();

            foreach(Tuple<int, IAbility> tuple in m_Abilities)
            {
                if(tuple.Item1 == level)
                {
                    abilities.Add(tuple.Item2);
                }
            }

            return abilities.ToArray();
        }

        public string Name
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }
    }
}
