namespace JoyLib.Code.Entities.Gender
{
    public interface IGender
    {
        string Possessive { get; }
        string Personal { get; }
        string Reflexive { get; }
        string Name { get; }

        string PossessivePlural { get; }
        string ReflexivePlural { get; }
    }
}