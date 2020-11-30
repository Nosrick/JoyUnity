﻿using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Quests
{
    public interface IQuestTracker
    {
        List<IQuest> GetQuestsForEntity(long GUID);

        IQuest GetPrimaryQuestForEntity(long GUID);

        void AddQuest(long GUID, IQuest quest);

        void CompleteQuest(IEntity questor, IQuest quest);

        void FailQuest(IEntity questor, IQuest quest);

        void AbandonQuest(IEntity questor, IQuest quest);

        void PerformQuestAction(IEntity questor, IQuest quest, IJoyAction completedAction);

        void PerformQuestAction(IEntity questor, IJoyAction completedAction);
    }
}