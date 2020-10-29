using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using System.Collections.Generic;
using JoyLib.Code.Managers;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Quests
{
    public class Quest : IQuest
    {
        public Quest(
            List<IQuestStep> steps,
            QuestMorality morality,
            List<ItemInstance> rewards,
            JoyObject instigator)
        {
            this.Steps = steps;
            this.Morality = morality;
            this.Rewards = rewards;
            this.Instigator = instigator;
            this.CurrentStep = 0;
            this.ID = GUIDManager.Instance.AssignGUID();
        }

        ~Quest()
        {
            GUIDManager.Instance.ReleaseGUID(this.ID);
        }

        public bool AdvanceStep()
        {
            this.CurrentStep++;

            return this.IsComplete;
        }

        public bool FulfilsRequirements(Entity questor, IJoyAction action)
        {
            return Steps[CurrentStep].Action.ExecutedSuccessfully(action);
        }

        public void StartQuest(Entity questor)
        {
            foreach (IQuestStep step in Steps)
            {
                step.StartQuest(questor);
            }
        }

        public bool BelongsToThis(object searchTerm)
        {
            switch (searchTerm)
            {
                case ItemInstance itemInstance:
                {
                    return Steps[CurrentStep].Items.Contains(itemInstance);
                }
                case Entity entity:
                {
                    return Steps[CurrentStep].Actors.Contains(entity);
                }
                case WorldInstance worldInstance:
                {
                    return Steps[CurrentStep].Areas.Contains(worldInstance);
                }
                default:
                    return false;
            }
        }

        public bool CompleteQuest(Entity questor)
        {
            if (this.IsComplete == false)
            {
                return false;
            }

            foreach (ItemInstance reward in Rewards)
            {
                questor.AddContents(reward);
            }

            return true;
        }

        public override string ToString()
        {
            string fullString = "";
            string rewardString = Rewards.Count > 0 ? "I'll give you " : "";
            for (int i = 0; i < Rewards.Count; i++)
            {
                rewardString += Rewards[i].JoyName;
                if(Rewards[i].Contents.Count != 0)
                {
                    rewardString += ", " + Rewards[i].ContentString;
                }
                if (Rewards.Count > 1)
                {
                    if (i == Rewards.Count - 2)
                        rewardString += "and ";
                    else if (i < Rewards.Count - 2)
                        rewardString += ", ";
                }
            }

            for (int j = 0; j < Steps.Count; j++)
            {
                fullString += Steps[j].ToString();
            }
            fullString += rewardString + ". ";
            return fullString;
        }

        public List<IQuestStep> Steps { get; protected set; }
        public QuestMorality Morality { get; protected set; }
        public List<ItemInstance> Rewards { get; protected set; }
        public int CurrentStep { get; protected set;  }

        public JoyObject Instigator { get; protected set; }
        
        public long ID { get; protected set; }

        public bool IsComplete => this.CurrentStep == this.Steps.Count;
    }
}
