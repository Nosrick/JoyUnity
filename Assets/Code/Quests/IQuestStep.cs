using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;

namespace JoyLib.Code.Quests
{
    public interface IQuestStep : ITagged
    {
        IQuestAction Action { get; }
        List<IItemInstance> Items { get; }
        List<IJoyObject> Actors { get; }
        List<WorldInstance> Areas { get; }

        void StartQuest(Entity questor);
    }
}