using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;

namespace JoyLib.Code.Quests
{
    public interface IQuestAction
    {
        string[] Tags { get; }
        string Description { get; }
        List<IItemInstance> Items { get; }
        List<IJoyObject> Actors { get; }
        List<WorldInstance> Areas { get; }
        
        RNG Roller { get; }

        IQuestStep Make(Entity questor, Entity provider, WorldInstance overworld, IEnumerable<string> tags);
        bool ExecutedSuccessfully(IJoyAction action);

        string AssembleDescription();

        void ExecutePrerequisites(Entity questor);

        IQuestAction Create(
            IEnumerable<string> tags,
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<WorldInstance> areas);
    }
}