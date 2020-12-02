using System;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Rollers;

namespace JoyLib.Code.Cultures
{
    public interface ICulture
    {
        string GetRandomName(string genderRef);

        string GetNameForChain(int chain, string gender, int group = Int32.MinValue);

        IBioSex ChooseSex(IBioSex[] sexes);

        ISexuality ChooseSexuality(ISexuality[] sexualities);

        IRomance ChooseRomance(IRomance[] romances);

        IGender ChooseGender(IBioSex sex, IGender[] genders);

        IJob ChooseJob(IJob[] jobs);

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