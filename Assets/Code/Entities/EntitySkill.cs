using JoyLib.Code.Entities.Needs;
using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public class EntitySkill
    {
        protected int m_Value;
        protected float m_Experience;

        protected Dictionary<NeedIndex, float> m_Coefficients;
        protected Dictionary<NeedIndex, EntityNeed> m_GoverningNeeds;

        protected const int MAXIMUM_XP = 100;

        public EntitySkill(int value, int successThreshold, float experience, Dictionary<NeedIndex, float> coefficients, 
            Dictionary<NeedIndex, EntityNeed> governingNeeds)
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
            foreach(NeedIndex index in m_Coefficients.Keys)
            {
                if (m_GoverningNeeds.ContainsKey(index) && m_GoverningNeeds[index].contributingHappiness)
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
            set;
        }

        public Dictionary<NeedIndex, float> coefficients
        {
            get
            {
                return m_Coefficients;
            }
        }

        public Dictionary<NeedIndex, EntityNeed> governingNeeds
        {
            get
            {
                return m_GoverningNeeds;
            }
        }
    }
}