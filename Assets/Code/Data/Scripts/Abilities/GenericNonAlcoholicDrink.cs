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
        { }

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

        public override bool OnAdd(IEntity entity)
        {
            return false;
        }

        public override bool OnAttack(IEntity attacker, IEntity target)
        {
            return false;
        }

        public override int OnHeal(IEntity receiver, IEntity healer, int healing)
        {
            return healing;
        }

        public override bool OnInteract(IEntity actor, IJoyObject observer)
        {
            return false;
        }

        public override bool OnReduceToZero(IEntity attacker, IEntity target, IDerivedValue value)
        {
            return false;
        }

        public override bool OnDisable(IEntity attacker, IEntity target, IDerivedValue value)
        {
            return false;
        }

        public override bool OnPickup(IEntity entity, IItemInstance item)
        {
            return false;
        }

        public override bool OnRemove(IEntity entity)
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

        public override int OnTakeHit(IEntity attacker, IEntity defender, int damage)
        {
            return damage;
        }

        public override bool OnTick(IEntity entity)
        {
            return false;
        }

        public override bool OnUse(IEntity user, IJoyObject target)
        {
            if(target is IItemInstance item)
            {
                m_CachedActions["fulfillneedaction"].Execute(
                    new IJoyObject[] { user },
                    new string[] { "thirst", "need", "fulfill" },
                    new object[] { "thirst", item.ItemType.Value, 10 }
                );
                user.RemoveItemFromPerson(item);
                return true;
            }
            return false;
        }
    }
}
