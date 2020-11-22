using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;

namespace JoyLib.Code.Cultures
{
    public interface ICulture
    {
        string GetRandomName(IGender genderRef);

        string GetNameForChain(int chain, string gender);

        IBioSex ChooseSex(IBioSex[] sexes);

        ISexuality ChooseSexuality(ISexuality[] sexualities);

        IRomance ChooseRomance(IRomance[] romances);

        IGender ChooseGender(IBioSex sex, IGender[] genders);

        IJob ChooseJob(IJob[] jobs);

        int GetStatVariance(string statistic);
        
        string[] Inhabitants { get; }
        
        string CultureName { get; }
        
        string[] RulerTypes { get; }
        
        string[] Crimes { get; }
        
        string[] RelationshipTypes { get; }
        
        string[] RomanceTypes { get; }
        
        string[] Sexes { get; }
        
        string[] Sexualities { get; }
        
        string[] Genders { get; }
        
        int NonConformingGenderChance { get; }
        
        NameData[] NameData { get; }
    }
}