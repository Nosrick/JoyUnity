using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;

namespace JoyLib.Code.Quests
{
    public interface IQuestStep
    {
        IQuestAction Action { get; }
        List<ItemInstance> Items { get; }
        List<IJoyObject> Actors { get; }
        List<WorldInstance> Areas { get; }

        void StartQuest(Entity questor);
    }
}