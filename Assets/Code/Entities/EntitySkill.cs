using JoyLib.Code.Entities.Needs;
using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public class EntitySkill
    {
        protected int m_Value;
        protected float m_Experience;

        protected Dictionary<string, float> m_Coefficients;
        protected Dictionary<string, INeed> m_GoverningNeeds;

        protected const int MAXIMUM_XP = 100;

        public EntitySkill(int value, int successThreshold, float experience, Dictionary<string, float> coefficients, 
            Dictionary<string, INeed> governingNeeds)
        {
            m_Value = value;
            SuccessThreshold = successThreshold;
            m_Experience = experience;
            m_Coefficients = coefficients;
            m_GoverningNeeds = governingNeeds;
        }

        public void AddExperience(float value)
        {
            float tempValue = value;
            m_Experience += value;
            foreach(string index in m_Coefficients.Keys)
            {
                if (m_GoverningNeeds.ContainsKey(index) && m_GoverningNeeds[index].ContributingHappiness)
                {
                    m_Experience += m_Coefficients[index] * value;
                }
            }
            while (tempValue / MAXIMUM_XP >= 1)
            {
                if (m_Experience >= MAXIMUM_XP)
                {
                    m_Value += 1;
                    m_Experience -= MAXIMUM_XP;
                    tempValue -= MAXIMUM_XP;
                }
            }
        }

        public int value
        {
            get
            {
                return m_Value;
            }
        }

        public float experience
        {
            get
            {
                return m_Experience;
            }
        }

        public int SuccessThreshold
        {
            get;
            protected set;
        }

        public Dictionary<string, float> coefficients
        {
            get
            {
                return m_Coefficients;
            }
        }

        public Dictionary<string, INeed> governingNeeds
        {
            get
            {
                return m_GoverningNeeds;
            }
        }
    }
}