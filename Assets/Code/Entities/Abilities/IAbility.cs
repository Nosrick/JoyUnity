using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using System;

namespace JoyLib.Code.Entities.Abilities
{
    public interface IAbility
    {
        //When the ability is added to the user
        bool OnAdd(Entity entity);

        //When the ability is removed from the user
        bool OnRemove(Entity entity);

        //When the entity attacks
        bool OnAttack(Entity attacker, Entity target);

        //When the entity is hit
        int OnTakeHit(Entity attacker, Entity defender, int damage);

        //When the entity heals
        int OnHeal(Entity receiver, Entity healer, int healing);

        //When the entity picks up an item
        bool OnPickup(Entity entity, ItemInstance item);

        //When the entity ticks
        bool OnTick(Entity entity);

        //When the entity reduces another entity to zero of a Derived Value
        bool OnReduceToZero(Entity attacker, Entity target, IDerivedValue value);

        //When the entity reduces another entity to the "disabled" status of a Derived Value
        bool OnDisable(Entity attacker, Entity target, IDerivedValue value);

        //When the entity uses an item
        bool OnUse(Entity user, JoyObject target);

        //When the entity interacts with something
        bool OnInteract(Entity actor, JoyObject observer);

        //When the entity uses a skill
        //This returns the success threshold modification for the roll
        //The second parameter is for checking against other possible stat/skill values
        int OnCheckRollModifyThreshold(int successThreshold, params IBasicValue[] values);

        //This returns bonus/penalty dice for the roll
        //The second parameter is for checking against other possible stat/skill values
        int OnCheckRollModifyDice(int dicePool, params IBasicValue[] values);

        //This is used for directly modifying the successes of the check
        //And should return the new successes
        //The second parameter is for checking against other possible stat/skill values
        int OnCheckSuccess(int successes, params IBasicValue[] values);

        bool DecrementCounter(int value);

        bool DecrementMagnitude(int value);

        void IncrementMagnitude(int value);

        void IncrementCounter(int value);

        bool HasTag(string tag);

        bool EnactToll(Entity caster);

        string Name
        {
            get;
        }

        string InternalName
        {
            get;
        }

        string Description
        {
            get;
        }

        bool Stacking
        {
            get;
        }

        int Counter
        {
            get;
        }

        int Magnitude
        {
            get;
        }

        int Priority
        {
            get;
        }

        bool ReadyForRemoval
        {
            get;
        }

        bool Permanent
        {
            get;
        }

        Tuple<IBasicValue, int>[] Costs
        {
            get;
        }

        string[] Tags
        {
            get;
        }

        AbilityTarget TargetType
        {
            get;
        }

        AbilityTrigger AbilityTrigger
        {
            get;
        }
    }
}
