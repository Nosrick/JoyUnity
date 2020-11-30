namespace JoyLib.Code.Entities
{
    public interface ILiveEntityHandler
    {
        bool AddEntity(IEntity created);

        bool Remove(long GUID);

        IEntity Get(long GUID);

        IEntity GetPlayer();

        void SetPlayer(IEntity entity);
        
        
    }
}