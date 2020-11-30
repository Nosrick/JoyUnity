using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using System;

namespace JoyLib.Code.Entities.Abilities
{
    public interface IAbility
    {
        //When the ability is added to the user
        bool OnAdd(IEntity entity);

        //When the ability is removed from the user
        bool OnRemove(IEntity entity);

        //When the entity attacks
        bool OnAttack(IEntity attacker, IEntity target);

        //When the entity is hit
        int OnTakeHit(IEntity attacker, IEntity defender, int damage);

        //When the entity heals
        int OnHeal(IEntity receiver, IEntity healer, int healing);

        //When the entity picks up an item
        bool OnPickup(IEntity entity, IItemInstance item);

        //When the entity ticks
        bool OnTick(IEntity entity);

        //When the entity reduces another entity to zero of a Derived Value
        bool OnReduceToZero(IEntity attacker, IEntity target, IDerivedValue value);

        //When the entity reduces another entity to the "disabled" status of a Derived Value
        bool OnDisable(IEntity attacker, IEntity target, IDerivedValue value);

        //When the entity uses an item
        bool OnUse(IEntity user, IJoyObject target);

        //When the entity interacts with something
        bool OnInteract(IEntity actor, IJoyObject observer);

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

        bool EnactToll(IEntity caster);

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
