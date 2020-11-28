using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Quests
{
    public interface IQuestTracker
    {
        List<IQuest> GetQuestsForEntity(long GUID);

        IQuest GetPrimaryQuestForEntity(long GUID);

        void AddQuest(long GUID, IQuest quest);

        void CompleteQuest(Entity questor, IQuest quest);

        void FailQuest(Entity questor, IQuest quest);

        void AbandonQuest(Entity questor, IQuest quest);

        void PerformQuestAction(Entity questor, IQuest quest, IJoyAction completedAction);

        void PerformQuestAction(Entity questor, IJoyAction completedAction);
    }
}