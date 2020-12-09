﻿using System.Collections.Generic;
using JoyLib.Code.Entities.Abilities;

namespace JoyLib.Code.Entities.Jobs
{
    public interface IJob
    {
        int GetSkillDiscount(string skillName);

        int GetStatisticDiscount(string statisticName);

        int AddExperience(int value);
        bool SpendExperience(int value);

        IJob Copy(IJob original);

        Dictionary<IAbility, int> Abilities { get; }
        
        string Name { get; }
        
        string Description { get; }
        
        int Experience { get; }
        
        Dictionary<string, int> StatisticDiscounts { get; }
        
        Dictionary<string, int> SkillDiscounts { get; }
    }
}