namespace JoyLib.Code.Entities.AI.Drivers
{
    public interface IDriver
    {
        bool PlayerControlled { get; }
        
        void Locomotion(Entity vehicle);

        void Interact();
    }
}