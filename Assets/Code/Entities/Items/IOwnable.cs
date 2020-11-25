namespace JoyLib.Code.Entities.Items
{
    public interface IOwnable
    {
        long OwnerGUID { get; }

        void SetOwner(long newOwner, bool recursive = false);
    }
}