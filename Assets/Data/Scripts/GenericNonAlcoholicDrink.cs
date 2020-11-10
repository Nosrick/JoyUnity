using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Abilities
{
    public class GenericNonAlcoholicDrink : AbstractAbility
    {
        public GenericNonAlcoholicDrink() : base(
            "drink",
            "genericnonalcoholicdrink",
            "Quench your thirst with a cool refreshment.",
            false,
            1,
            1,
            1,
            false,
            new string[] { "fulfillneedaction" },
            new Tuple<IBasicValue, int>[0],
            AbilityTrigger.OnUse,
            AbilityTarget.Self)
        {

        }

        public GenericNonAlcoholicDrink(
            string name, 
            string internalName, 
            string description, 
            bool stacking, 
            int counter, 
            int magnitude,
            int priority, 
            bool permanent, 
            Tuple<IBasicValue, int>[] costs, 
            AbilityTrigger trigger, 
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
                trigger, 
                target)
        {
        }

        public override bool OnAdd(Entity entity)
        {
            return false;
        }

        public override bool OnAttack(Entity attacker, Entity target)
        {
            return false;
        }

        public override int OnHeal(Entity receiver, Entity healer, int healing)
        {
            return healing;
        }

        public override bool OnInteract(Entity actor, JoyObject observer)
        {
            return false;
        }

        public override bool OnReduceToZero(Entity attacker, Entity target, IDerivedValue value)
        {
            return false;
        }

        public override bool OnDisable(Entity attacker, Entity target, IDerivedValue value)
        {
            return false;
        }

        public override bool OnPickup(Entity entity, ItemInstance item)
        {
            return false;
        }

        public override bool OnRemove(Entity entity)
        {
            return false;
        }

        public override int OnCheckRollModifyDice(int dicePool, params IBasicValue[] values)
        {
            return dicePool;
        }

        public override int OnCheckRollModifyThreshold(int successThreshold, params IBasicValue[] values)
        {
            return successThreshold;
        }

        public override int OnCheckSuccess(int successes, params IBasicValue[] values)
        {
            return successes;
        }

        public override int OnTakeHit(Entity attacker, Entity defender, int damage)
        {
            return damage;
        }

        public override bool OnTick(Entity entity)
        {
            return false;
        }

        public override bool OnUse(Entity user, JoyObject target)
        {
            ItemInstance item = target as ItemInstance;
            if(item != null)
            {
                m_CachedActions["fulfillneedaction"].Execute(
                    new JoyObject[] { user },
                    new string[] { "thirst", "alcohol", "need", "fulfill" },
                    new object[] { "thirst", item.ItemType.Value, 10 }
                );
                user.RemoveItemFromPerson(item);
                return true;
            }
            return false;
        }
    }
}
