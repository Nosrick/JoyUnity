using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Quests
{
    public class Quest
    {
        protected readonly List<QuestStep> m_Steps;
        protected readonly QuestMorality m_Morality;
        protected readonly List<ItemInstance> m_Rewards;

        public Quest(List<QuestStep> steps, QuestMorality morality, List<ItemInstance> rewards, int step = 0)
        {
            m_Steps = steps;
            m_Morality = morality;
            m_Rewards = rewards;
            this.step = step;
        }

        public bool BelongsToThis(object searchTerm)
        {
            if(searchTerm is ItemInstance)
            {
                ItemInstance item = (ItemInstance)searchTerm;
                return steps[step].objects.Contains(item);
            }
            else if(searchTerm is Entity)
            {
                Entity entity = (Entity)searchTerm;
                return steps[step].actors.Contains(entity);
            }
            else if(searchTerm is WorldInstance)
            {
                WorldInstance world = (WorldInstance)searchTerm;
                return steps[step].areas.Contains(world);
            }

            return false;
        }

        public override string ToString()
        {
            string fullString = "";
            string rewardString = m_Rewards.Count > 0 ? "I'll give you " : "";
            for (int i = 0; i < m_Rewards.Count; i++)
            {
                rewardString += m_Rewards[i].JoyName;
                if(m_Rewards[i].Contents.Count != 0)
                {
                    rewardString += ", " + m_Rewards[i].ContentString;
                }
                if (m_Rewards.Count > 1)
                {
                    if (i == m_Rewards.Count - 2)
                        rewardString += "and ";
                    else if (i < m_Rewards.Count - 2)
                        rewardString += ", ";
                }
            }

            for (int j = 0; j < m_Steps.Count; j++)
            {
                fullString += m_Steps[j].ToString();
            }
            fullString += rewardString + ". ";
            return fullString;
        }

        public int step
        {
            get;
            set;
        }

        public List<QuestStep> steps
        {
            get
            {
                return m_Steps.ToList();
            }
        }

        public QuestMorality morality
        {
            get
            {
                return m_Morality;
            }
        }

        public List<ItemInstance> rewards
        {
            get
            {
                return m_Rewards.ToList();
            }
        }
    }
}
