using JoyLib.Code.Entities.AI;

namespace JoyLib.Code.Entities.Needs
{
    public class NeedConfidence : NeedAbstract
    {
        //We don't have this implemented yet, so just go wandering
        public override NeedAIData FindFulfilmentObject(Entity entity)
        {
            NeedAIData needAIData = new NeedAIData();
            needAIData.intent = Intent.Interact;
            needAIData.searching = true;
            needAIData.target = null;
            needAIData.targetPoint = new UnityEngine.Vector2Int(-1, -1);

            return needAIData;
        }

        public override void Interact(EntityNeed need, Entity actor, JoyObject obj)
        {
        }

        public override void OnTick(EntityNeed need, Entity actor)
        {
        }
    }
}
