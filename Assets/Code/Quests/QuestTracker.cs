using System;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Quests
{
    public class QuestTracker : IQuestTracker
    {
        protected Dictionary<long, List<IQuest>> EntityQuests { get; set; }

        public QuestTracker()
        {
            Initialise();
        }
        
        public void Initialise()
        {
            if (EntityQuests is null)
            {
                EntityQuests = new Dictionary<long, List<IQuest>>();
            }
        }

        public List<IQuest> GetQuestsForEntity(long GUID)
        {
            if (EntityQuests.ContainsKey(GUID))
            {
                return EntityQuests[GUID];
            }
                
            return new List<IQuest>();
        }

        public IQuest GetPrimaryQuestForEntity(long GUID)
        {
            if (EntityQuests.ContainsKey(GUID) && EntityQuests[GUID].Count > 0)
            {
                return EntityQuests[GUID][0];
            }
            
            throw new InvalidOperationException("No quests found for " + GUID + ".");
        }

        public void AddQuest(long GUID, IQuest quest)
        {
            if (EntityQuests.ContainsKey(GUID))
            {
                EntityQuests[GUID].Add(quest);
            }
            else
            {
                EntityQuests.Add(GUID, new List<IQuest> { quest });
            }
        }

        public void CompleteQuest(Entity questor, IQuest quest)
        {
            quest.CompleteQuest(questor);
            EntityQuests[questor.GUID].Remove(quest);
        }

        public void AbandonQuest(Entity questor, IQuest quest)
        {
            EntityQuests[questor.GUID].Remove(quest);
        }

        public void FailQuest(Entity questor, IQuest quest)
        {
            EntityQuests[questor.GUID].Remove(quest);
        }

        public void PerformQuestAction(Entity questor, IQuest quest, IJoyAction completedAction)
        {
            if (quest.FulfilsRequirements(questor, completedAction) && quest.AdvanceStep())
            {
                CompleteQuest(questor, quest);
            }
        }

        public void PerformQuestAction(Entity questor, IJoyAction completedAction)
        {
            List<IQuest> copy = new List<IQuest>(GetQuestsForEntity(questor.GUID));
            foreach (IQuest quest in copy)
            {
                PerformQuestAction(questor, quest, completedAction);
            }
        }
    }
}
