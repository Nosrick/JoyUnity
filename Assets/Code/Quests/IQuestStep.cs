using System.Collections.Generic;
using JoyLib.Code.Entities;

namespace JoyLib.Code.Quests
{
    public interface IQuestStep : ITagged
    {
        IQuestAction Action { get; }
        List<long> Items { get; }
        List<long> Actors { get; }
        List<long> Areas { get; }

        void StartQuest(IEntity questor);
    }
}