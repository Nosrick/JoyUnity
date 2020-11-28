using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.World;

namespace JoyLib.Code.Quests
{
    public interface IQuestProvider
    {
        IQuest MakeRandomQuest(Entity questor, Entity provider, WorldInstance overworldRef);

        IQuest MakeQuestOfType(Entity questor, Entity provider, WorldInstance overworldRef, string[] tags);

        List<IQuestAction> Actions { get; }
    }
}