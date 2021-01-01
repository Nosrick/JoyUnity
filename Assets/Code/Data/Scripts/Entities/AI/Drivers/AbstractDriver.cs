namespace JoyLib.Code.Entities.AI.Drivers
{
    public abstract class AbstractDriver : IDriver
    {
        public virtual bool PlayerControlled => false;

        public virtual void Interact()
        {
            throw new System.NotImplementedException("Someone forgot to implement Interact()");
        }

        public virtual void Locomotion(Entity vehicle)
        {
            throw new System.NotImplementedException("Someone forgot to implement Locomotion()");
        }
    }
}