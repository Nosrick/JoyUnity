namespace JoyLib.Code.Entities.Items
{
    public interface IOwnable
    {
        long Owner { get; }

        void SetOwner(long newOwner);
    }
}