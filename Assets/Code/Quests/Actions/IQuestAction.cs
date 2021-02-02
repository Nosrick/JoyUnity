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
        List<long> Items { get; }
        List<long> Actors { get; }
        List<long> Areas { get; }
        
        RNG Roller { get; }

        IQuestStep Make(IEntity questor, IEntity provider, IWorldInstance overworld, IEnumerable<string> tags);
        bool ExecutedSuccessfully(IJoyAction action);

        string AssembleDescription();

        void ExecutePrerequisites(IEntity questor);

        IQuestAction Create(
            IEnumerable<string> tags,
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<IWorldInstance> areas,
            IItemFactory itemFactory = null);
    }
}