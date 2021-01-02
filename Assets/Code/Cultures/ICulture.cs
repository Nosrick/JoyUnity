﻿using System.Collections.Generic;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Cultures
{
    public interface ICulture
    {
        string GetRandomName(string genderRef);

        string GetNameForChain(int chain, string gender, int group);

        IBioSex ChooseSex(IEnumerable<IBioSex> sexes);

        ISexuality ChooseSexuality(IEnumerable<ISexuality> sexualities);

        IRomance ChooseRomance(IEnumerable<IRomance> romances);

        IGender ChooseGender(IBioSex sex, IEnumerable<IGender> genders);

        IJob ChooseJob(IEnumerable<IJob> jobs);

        IDictionary<string, IRollableValue<int>> GetStats(IDictionary<string, IRollableValue<int>> baseStats);

        void ClearLastGroup();

        int GetStatVariance(string statistic);
        
        string Tileset { get; }
        
        int LastGroup { get; }
        
        string[] Inhabitants { get; }
        
        string CultureName { get; }
        
        string[] RulerTypes { get; }
        
        string[] Crimes { get; }
        
        string[] RelationshipTypes { get; }
        
        string[] RomanceTypes { get; }
        
        string[] Sexes { get; }
        
        string[] Sexualities { get; }
        
        string[] Genders { get; }
        
        string[] Jobs { get; }
        
        int NonConformingGenderChance { get; }
        
        NameData[] NameData { get; }
        
        RNG Roller { get; }
    }
}