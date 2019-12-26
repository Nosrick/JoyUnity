using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Abilities
{
    public class GenericNonAlcoholicDrink : AbstractAbility
    {
        public GenericNonAlcoholicDrink()
        {
            this.Name = "Drink";
            this.InternalName = "GenericNonAlcoholicDrink";
            this.Description = "Quench your thirst with a cool refreshment.";
            this.Stacking = false;
            this.Counter = 1;
            this.Magnitude = 1;
            this.Priority = 1;
            this.Permanent = false;
            this.Costs = new Tuple<IBasicValue, int>[0];
            this.AbilityTrigger = AbilityTrigger.OnUse;
            this.TargetType = AbilityTarget.Self;
        }

        public GenericNonAlcoholicDrink(string name, string internalName, string description, bool stacking, int counter, int magnitude,
            int priority, bool permanent, Tuple<IBasicValue, int>[] costs, AbilityTrigger trigger, AbilityTarget target) : 
            base(name, internalName, description, stacking, counter, magnitude, priority, permanent, costs, trigger, target)
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
                user.FulfillNeed("thirst", item.ItemType.Value, new JoyObject[] { item }, 1);
                user.RemoveItemFromPerson(item);
                return true;
            }
            return false;
        }
    }
}
