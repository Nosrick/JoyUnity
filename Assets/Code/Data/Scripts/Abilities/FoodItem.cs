using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities.Abilities
{
    public class FoodItem : AbstractAbility
    {
        public FoodItem() : base(
            "eat",
            "fooditem",
            "Sate your hunger with a tasty meal.",
            false,
            1,
            1,
            1,
            false,
            new string[] { "fulfillneedaction" },
            new Tuple<IBasicValue, int>[0],
            new Dictionary<IBasicValue, int>(),
            AbilityTarget.Self)
        { }
        
        public FoodItem(
            string name, 
            string internalName, 
            string description, 
            bool stacking, 
            int counter, 
            int magnitude,
            int priority, 
            bool permanent, 
            Tuple<IBasicValue, int>[] costs, 
            Dictionary<IBasicValue, int> prerequisites,
            AbilityTarget target) : 
            base(
                name, 
                internalName, 
                description, 
                stacking, 
                counter, 
                magnitude, 
                priority, 
                permanent, 
                new string[] { "fulfillneedaction" },
                costs, 
                prerequisites,
                target)
        {}
        
        public override bool OnUse(IEntity user, IJoyObject target)
        {
            if(target is IItemInstance item)
            {
                m_CachedActions["fulfillneedaction"].Execute(
                    new IJoyObject[] { user },
                    new string[] { "hunger", "need", "fulfill" },
                    new object[] { "hunger", item.ItemType.Value, 10 }
                );
                user.RemoveItemFromPerson(item);
                return true;
            }
            return false;
        }
    }
}