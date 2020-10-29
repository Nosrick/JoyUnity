using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;

namespace JoyLib.Code.Quests
{
    public interface IQuestAction
    {
        string[] Tags { get; }
        string Description { get; }
        List<ItemInstance> Items { get; }
        List<IJoyObject> Actors { get; }
        List<WorldInstance> Areas { get; }

        IQuestStep Make(Entity provider, WorldInstance overworld);
        bool ExecutedSuccessfully(IJoyAction action);

        string AssembleDescription();

        void ExecutePrerequisites(Entity questor);

        IQuestAction Create(
            string[] tags,
            List<ItemInstance> items,
            List<IJoyObject> actors,
            List<WorldInstance> areas);
    }
}