using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using System;
using System.Linq;
using System.Collections.Generic;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Abilities
{
    public abstract class AbstractAbility : IAbility
    {
        protected readonly Dictionary<string, IJoyAction> m_CachedActions;

        public AbstractAbility()
        {
        }

        public AbstractAbility(
            string name, 
            string internalName, 
            string description, 
            bool stacking, 
            int counter, 
            int magnitude,
            int priority, 
            bool permanent, 
            string[] actions,
            Tuple<IBasicValue, int>[] costs, 
            AbilityTrigger trigger, 
            AbilityTarget target, 
            params string[] tags)
        {
            this.Name = name;
            this.InternalName = internalName;
            this.Description = description;
            this.Stacking = stacking;
            this.Counter = counter;
            this.Magnitude = magnitude;
            this.Priority = priority;
            this.Permanent = permanent;
            this.Costs = costs;
            this.AbilityTrigger = trigger;
            this.TargetType = target;
            this.Tags = tags;

            this.m_CachedActions = new Dictionary<string, IJoyAction>(actions.Length);

            foreach(string action in actions)
            {
                this.m_CachedActions.Add(action, ScriptingEngine.instance.FetchAction(action));
            }
        }

        //When the entity attacks, before any resolution occurs
        //Returns false to denote no effect took place
        public virtual bool OnAttack(IEntity attacker, IEntity target)
        {
            return false;
        }

        //When the entity is hit, after resolution
        //Triggers even when no damage happens
        //Returns the damage by default
        public virtual int OnTakeHit(IEntity attacker, IEntity defender, int damage)
        {
            return damage;
        }

        //When the entity is healed
        //Returns the amount healed
        public virtual int OnHeal(IEntity receiver, IEntity healer, int value)
        {
            return value;
        }

        //When the entity picks up an item
        //Return true to denote an effect took place
        //Thus returns false by default
        public virtual bool OnPickup(IEntity entity, IItemInstance item)
        {
            return false;
        }

        //Triggered when the entity ticks
        //Returns true when an effect triggers
        //Thus returns false by default
        public virtual bool OnTick(IEntity entity)
        {
            return false;
        }

        //Triggered when an entity reduces another entity to zero of a Derived Value
        //Returns true when the effect takes place
        public virtual bool OnReduceToZero(IEntity attacker, IEntity target, IDerivedValue value)
        {
            return false;
        }

        //Triggered when an entity reduces another entity to the "disabled" of a Derived Value
        //Returns true when the effect takes place
        public virtual bool OnDisable(IEntity attacker, IEntity target, IDerivedValue value)
        {
            return false;
        }

        //Triggered when an entity uses the JoyObject this ability is attached to
        //This is typically an item
        //Returns true when the ability triggers
        public virtual bool OnUse(IEntity user, IJoyObject target)
        {
            return false;
        }

        //Triggered when an entity interacts with the JoyObject (could be an item or an entity or whatever)
        //Returns true when the effect triggers
        public virtual bool OnInteract(IEntity actor, IJoyObject observer)
        {
            return false;
        }

        //Triggered when the ability is added to the entity
        //Returns true if an effect was added (other than this one)
        public virtual bool OnAdd(IEntity entity)
        {
            return false;
        }

        //Triggered when the ability is removed from the entity
        //Return true if an effect was removed (other than this one)
        public virtual bool OnRemove(IEntity entity)
        {
            return false;
        }

        //When the entity uses a skill
        //This returns the success threshold modification for the roll
        //The second parameter is for checking against other possible stat/skill values
        public virtual int OnCheckRollModifyThreshold(int successThreshold, params IBasicValue[] values)
        {
            return successThreshold;
        }

        //This returns bonus/penalty dice for the roll
        //The second parameter is for checking against other possible stat/skill values
        public virtual int OnCheckRollModifyDice(int dicePool, params IBasicValue[] values)
        {
            return dicePool;
        }

        //This is used for directly modifying the successes of the check
        //And should return the new successes
        //The second parameter is for checking against other possible stat/skill values
        public virtual int OnCheckSuccess(int successes, params IBasicValue[] values)
        {
            return successes;
        }

        public bool HasTag(string tag)
        {
            return Tags.Contains(tag);
        }

        public bool DecrementCounter(int value)
        {
            Counter = Math.Max(0, Counter - value);
            return ReadyForRemoval;
        }

        public bool DecrementMagnitude(int value)
        {
            Magnitude = Math.Max(0, Magnitude - value);
            return ReadyForRemoval;
        }

        public void IncrementMagnitude(int value)
        {
            if(Stacking == true)
            {
                Magnitude += value;
            }
        }

        public void IncrementCounter(int value)
        {
            if(Stacking == true)
            {
                Counter += value;
            }
        }

        public bool EnactToll(IEntity caster)
        {
            bool[] canCast = new bool[Costs.Length];
            for(int i = 0; i < Costs.Length; i++)
            {
                Tuple<string, int>[] returnData = caster.GetData(new string[] { Costs[i].Item1.Name });
                canCast[i] = returnData.All(x => x.Item2 >= Costs[i].Item2);
            }
            bool takeToll = canCast.All(x => x == true);

            if(takeToll)
            {

            }

            return takeToll;
        }

        public string Name
        {
            get;
            protected set;
        }

        public string InternalName
        {
            get;
            protected set;
        }

        public string Description
        {
            get;
            protected set;
        }

        public bool Stacking
        {
            get;
            protected set;
        }

        public int Counter
        {
            get;
            protected set;
        }

        public int Magnitude
        {
            get;
            protected set;
        }

        public int Priority
        {
            get;
            protected set;
        }

        public bool ReadyForRemoval
        {
            get
            {
                if (Permanent == true)
                {
                    return false;
                }
                if (Counter <= 0 || Magnitude <= 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool Permanent
        {
            get;
            protected set;
        }

        public Tuple<IBasicValue, int>[] Costs
        {
            get;
            protected set;
        }

        public AbilityTarget TargetType
        {
            get;
            protected set;
        }

        public AbilityTrigger AbilityTrigger
        {
            get;
            protected set;
        }

        public string[] Tags
        {
            get;
            protected set;
        }
    }
}
