namespace JoyLib.Code.Entities
{
    public interface ILiveEntityHandler
    {
        bool AddEntity(Entity created);

        bool Remove(long GUID);

        Entity Get(long GUID);

        Entity GetPlayer();

        void SetPlayer(Entity entity);
        
        
    }
}