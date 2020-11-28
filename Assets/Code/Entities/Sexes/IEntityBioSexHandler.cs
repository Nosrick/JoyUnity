namespace JoyLib.Code.Entities.Sexes
{
    public interface IEntityBioSexHandler
    {
        IBioSex Get(string name);
        IBioSex[] Sexes { get; }
    }
}