namespace JoyLib.Code.Entities.Romance
{
    public interface IEntityRomanceHandler
    {
        IRomance Get(string romance);
        
        IRomance[] Romances { get; }
    }
}