using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Managers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Quests
{
    [Serializable]
    public class Quest : IQuest
    {
        protected List<string> m_Tags;
        
        public Quest(
            List<IQuestStep> steps,
            QuestMorality morality,
            List<IItemInstance> rewards,
            long instigator,
            long questor,
            IEnumerable<string> tags)
        {
            this.Steps = steps;
            this.Morality = morality;
            this.RewardGUIDs = rewards.Select(instance => instance.GUID).ToList();
            this.Instigator = instigator;
            this.Questor = questor;
            this.CurrentStep = 0;
            this.ID = GUIDManager.Instance.AssignGUID();
            this.Tags = new List<string>(tags);
        }

        ~Quest()
        {
            GUIDManager.Instance.ReleaseGUID(this.ID);
            GlobalConstants.GameManager.ItemHandler.CleanUpRewards(this.RewardGUIDs);
        }

        public bool AdvanceStep()
        {
            this.CurrentStep++;

            return this.IsComplete;
        }

        public bool FulfilsRequirements(IEntity questor, IJoyAction action)
        {
            return this.Steps[this.CurrentStep].Action.ExecutedSuccessfully(action);
        }

        public void StartQuest(IEntity questor)
        {
            foreach (IQuestStep step in this.Steps)
            {
                step.StartQuest(questor);
            }
        }

        public bool BelongsToThis(object searchTerm)
        {
            switch (searchTerm)
            {
                case IItemInstance itemInstance:
                {
                    return this.Steps[this.CurrentStep].Items.Contains(itemInstance.GUID);
                }
                case IEntity entity:
                {
                    return this.Steps[this.CurrentStep].Actors.Contains(entity.GUID);
                }
                case IWorldInstance worldInstance:
                {
                    return this.Steps[this.CurrentStep].Areas.Contains(worldInstance.GUID);
                }
                default:
                    return false;
            }
        }

        public bool CompleteQuest(IEntity questor)
        {
            if (this.IsComplete == false)
            {
                return false;
            }

            foreach (IItemInstance reward in this.Rewards)
            {
                if (reward is ItemInstance item)
                {
                    item.Instantiate();
                }
                questor.AddContents(reward);
            }

            return true;
        }
        
        public bool AddTag(string tag)
        {
            if (this.m_Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)) != false)
            {
                return false;
            }
            
            this.m_Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (!this.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            
            this.m_Tags.Remove(tag);
            return true;
        }

        public bool HasTag(string tag)
        {
            return this.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        public override string ToString()
        {
            string fullString = "";
            string rewardString = this.Rewards.Count > 0 ? "I'll give you " : "";
            for (int i = 0; i < this.Rewards.Count; i++)
            {
                rewardString += this.Rewards[i].JoyName;
                if(this.Rewards[i].Contents.Any())
                {
                    rewardString += ", " + this.Rewards[i].ContentString;
                }
                if (this.Rewards.Count > 1)
                {
                    if (i == this.Rewards.Count - 2)
                        rewardString += "and ";
                    else if (i < this.Rewards.Count - 2)
                        rewardString += ", ";
                }
            }

            for (int j = 0; j < this.Steps.Count; j++)
            {
                fullString += this.Steps[j].ToString();
            }
            fullString += " " + rewardString + ".";
            return fullString;
        }

        [OdinSerialize]
        public List<IQuestStep> Steps { get; protected set; }
        
        [OdinSerialize]
        public QuestMorality Morality { get; protected set; }
        
        [OdinSerialize]
        public List<long> RewardGUIDs { get; protected set; }

        public List<IItemInstance> Rewards =>
            GlobalConstants.GameManager.ItemHandler.GetQuestRewards(this.ID).ToList();
        
        [OdinSerialize]
        public int CurrentStep { get; protected set;  }

        [OdinSerialize]
        public long Instigator { get; protected set; }

        [OdinSerialize] 
        public long Questor { get; protected set; }

        [OdinSerialize]
        public long ID { get; protected set; }

        public bool IsComplete => this.CurrentStep == this.Steps.Count;

        public IEnumerable<string> Tags
        {
            get => this.m_Tags;
            protected set => this.m_Tags = new List<string>(value);
        }
    }
}
