using System.Collections.Generic;
using JoyLib.Code.Entities.Abilities;

namespace JoyLib.Code.Entities.Jobs
{
    public interface IJob
    {
        int GetSkillGrowth(string skillName);

        float GetStatisticGrowth(string statisticName);

        IAbility[] GetAbilitiesForLevel(int level);
        
        string Name { get; }
        
        string Description { get; }
        
        Dictionary<string, float> StatisticGrowths { get; }
        
        Dictionary<string, int> SkillGrowths { get; }
    }
}