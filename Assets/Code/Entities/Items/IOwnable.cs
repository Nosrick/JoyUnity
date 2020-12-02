namespace JoyLib.Code.Entities.Items
{
    public interface IOwnable
    {
        string OwnerString { get; }
        
        long OwnerGUID { get; }

        void SetOwner(long newOwner, bool recursive = false);
    }
}