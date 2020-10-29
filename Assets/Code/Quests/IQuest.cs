using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Quests
{
    public interface IQuest
    {
        List<IQuestStep> Steps { get; }
        QuestMorality Morality { get; }
        List<ItemInstance> Rewards { get; }
        int CurrentStep { get; }
        
        JoyObject Instigator { get; }
        
        long ID { get; }

        bool BelongsToThis(object searchTerm);
        bool AdvanceStep();

        bool FulfilsRequirements(Entity questor, IJoyAction action);

        void StartQuest(Entity questor);

        bool CompleteQuest(Entity questor);
    }
}