using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.World;

namespace JoyLib.Code.Quests
{
    public interface IQuestProvider
    {
        IQuest MakeRandomQuest(IEntity questor, IEntity provider, WorldInstance overworldRef);

        IQuest MakeQuestOfType(IEntity questor, IEntity provider, WorldInstance overworldRef, string[] tags);

        IEnumerable<IQuest> MakeOneOfEachType(IEntity questor, IEntity provider, WorldInstance overworldRef);

        List<IQuestAction> Actions { get; }
    }
}