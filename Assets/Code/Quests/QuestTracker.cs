using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Quests
{
    public static class QuestTracker
    {
        private static Dictionary<long, List<Quest>> s_EntityQuests;

        public static void Initialise()
        {
            s_EntityQuests = new Dictionary<long, List<Quest>>();
        }

        public static List<Quest> GetQuestsForEntity(long GUID)
        {
            if (s_EntityQuests.ContainsKey(GUID))
                return s_EntityQuests[GUID];
            else
                return new List<Quest>();
        }

        public static Quest GetPrimaryQuestForEntity(long GUID)
        {
            if (s_EntityQuests.ContainsKey(GUID) && s_EntityQuests[GUID].Count > 0)
                return s_EntityQuests[GUID][0];
            else
                return null;
        }

        public static void AddQuest(long GUID, Quest quest)
        {
            if (s_EntityQuests.ContainsKey(GUID))
                s_EntityQuests[GUID].Add(quest);
            else
            {
                s_EntityQuests.Add(GUID, new List<Quest>() { quest });
            }
        }

        private static void CompleteQuest(Entity questor, Quest quest)
        {
            for (int i = 0; i < quest.rewards.Count; i++)
            {
                questor.AddContents(quest.rewards[i]);
            }
            questor.AddExperience(quest.step * 10);

            lock (s_EntityQuests)
            {
                s_EntityQuests[questor.GUID].Remove(quest);
            }
        }

        public static void PerformDelivery(Entity questor, ItemInstance item, Entity recipient)
        {
            try
            {
                bool matchingEntity, matchingItem;
                matchingEntity = s_EntityQuests[questor.GUID].Any(x => x.BelongsToThis(recipient));
                matchingItem = s_EntityQuests[questor.GUID].Any(x => x.BelongsToThis(item));

                if (matchingItem && matchingEntity)
                {
                    Quest quest = s_EntityQuests[questor.GUID].First(x => x.BelongsToThis(recipient));
                    quest.step += 1;
                    if (quest.step == quest.steps.Count)
                    {
                        //Then the quest is complete!
                        CompleteQuest(questor, quest);
                    }
                }
            }
            catch
            {

            }
        }

        public static void PerformEntityDestruction(Entity questor, Entity target)
        {
            try
            {
                if (s_EntityQuests[questor.GUID].Any(x => x.BelongsToThis(target)))
                {
                    Quest quest = s_EntityQuests[questor.GUID].First(x => x.BelongsToThis(target));
                    quest.step += 1;
                    if (quest.step == quest.steps.Count)
                    {
                        CompleteQuest(questor, quest);
                    }
                }
            }
            catch
            {

            }
        }

        public static void PerformItemDestruction(Entity questor, ItemInstance target)
        {
            try
            {
                if (s_EntityQuests[questor.GUID].Any(x => x.BelongsToThis(target)))
                {
                    Quest quest = s_EntityQuests[questor.GUID].First(x => x.BelongsToThis(target));
                    quest.step += 1;
                    if (quest.step == quest.steps.Count)
                    {
                        CompleteQuest(questor, quest);
                    }
                }
            }
            catch
            {

            }
        }

        public static void PerformEntityDestruction(Entity questor, ItemInstance target)
        {
            try
            {
                if (s_EntityQuests[questor.GUID].Any(x => x.BelongsToThis(target)))
                {
                    Quest quest = s_EntityQuests[questor.GUID].First(x => x.BelongsToThis(target));
                    quest.step += 1;
                    if (quest.step == quest.steps.Count)
                    {
                        CompleteQuest(questor, quest);
                    }
                }
            }
            catch
            {

            }
        }

        public static void PerformExploration(Entity questor, WorldInstance target)
        {
            try
            {
                if (s_EntityQuests[questor.GUID].Any(x => x.BelongsToThis(target)))
                {
                    Quest quest = s_EntityQuests[questor.GUID].First(x => x.BelongsToThis(target));
                    quest.step += 1;
                    if (quest.step == quest.steps.Count)
                    {
                        CompleteQuest(questor, quest);
                    }
                }
            }
            catch
            {

            }
        }

        public static void PerformRetrieval(Entity questor, ItemInstance target)
        {
            try
            {
                if (s_EntityQuests[questor.GUID].Any(x => x.BelongsToThis(target)))
                {
                    Quest quest = s_EntityQuests[questor.GUID].First(x => x.BelongsToThis(target));
                    quest.step += 1;
                    if (quest.step == quest.steps.Count)
                    {
                        CompleteQuest(questor, quest);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
