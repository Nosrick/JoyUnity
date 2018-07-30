using JoyLib.Code.Entities.AI;

namespace JoyLib.Code.Entities.Needs
{
    public abstract class NeedAbstract
    {
        public abstract NeedAIData FindFulfilmentObject(Entity entity);
        public abstract void Interact(EntityNeed need, Entity actor, JoyObject obj);
        public abstract void OnTick(EntityNeed need, Entity actor);

        public string Name
        {
            get;
            protected set;
        }
    }
}
