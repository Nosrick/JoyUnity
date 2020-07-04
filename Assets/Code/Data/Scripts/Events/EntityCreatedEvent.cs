using JoyLib.Code.Entities;

namespace JoyLib.Code.Events
{
    public class EntityCreatedEvent : JoyEvent
    {
        public void AttachEntity(Entity created)
        {
            Created = created;
        }

        public Entity Created
        {
            get;
            set;
        }
    }
}