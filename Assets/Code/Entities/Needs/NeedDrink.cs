using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Needs
{
    public class NeedDrink : NeedAbstract
    {
        public override NeedAIData FindFulfilmentObject(Entity entity)
        {
            List<NeedAIData> targets = entity.MyWorld.SearchForObjects(entity, "Drinks", Intent.Interact);
            if(targets.Count > 0)
            {
                int index = RNG.Roll(0, targets.Count - 1);
                return targets[index];
            }
            else
            {
                NeedAIData data = new NeedAIData();
                data.searching = true;
                return data;
            }
        }

        public override void Interact(EntityNeed need, Entity actor, JoyObject obj)
        {
            if(obj.GetType() != typeof(ItemInstance))
            {
                return;
            }

            ItemInstance item = (ItemInstance)obj;
            
        }

        public override void OnTick(EntityNeed need, Entity actor)
        {
        }
    }
}
