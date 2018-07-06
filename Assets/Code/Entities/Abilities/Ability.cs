using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Entities.Abilities
{
    public abstract class Ability
    {
        public readonly AbilityTrigger m_AbilityTrigger;
        public readonly bool m_Stacking;
        public readonly string m_Name;
        public readonly string m_InternalName;
        public readonly string m_Description;
        protected AbilityTarget m_Target;
        protected bool m_ReadyForRemoval;
        protected int m_Counter;
        protected int m_Magnitude;
        protected int m_Priority;
        protected int m_ManaCost;

        public const int PERMANENT_ABILITY = int.MinValue;

        public Ability(AbilityTrigger triggerRef, AbilityTarget targetRef, bool stackRef, string nameRef, string internalNameRef, string descriptionRef,
            int priorityRef, int counterRef, int magnitudeRef, int manaCost)
        {
            m_AbilityTrigger = triggerRef;
            m_Target = targetRef;
            m_Stacking = stackRef;
            m_Name = nameRef;
            m_InternalName = internalNameRef;
            m_Description = descriptionRef;
            m_ReadyForRemoval = false;
            m_Counter = counterRef;
            m_Magnitude = magnitudeRef;
            m_Priority = priorityRef;

            m_ManaCost = manaCost;
        }

        //When the entity attacks
        public virtual void OnAttack(Entity attacker, Entity target)
        {
        }

        //When the entity is hit
        public virtual int OnTakeHit(Entity attacker, Entity defender, int damage)
        {
            return damage;
        }

        //When the entity heals
        public virtual int OnHeal(Entity receiver, int healing)
        {
            return healing;
        }

        //When the entity picks up an item
        public virtual void OnPickup(Entity entity, ItemInstance item)
        {
        }

        public virtual void OnTick(Entity entity)
        {
        }

        public virtual void OnKill(Entity attacker, Entity target)
        {
        }

        public virtual void Use(Entity user, JoyObject target)
        {
            if (user.ManaRemaining < m_ManaCost)
                return;
        }

        protected void CheckForRemoval()
        {
            if (permanent)
                return;

            if (m_Counter <= 0 || m_Magnitude <= 0)
                m_ReadyForRemoval = true;
        }

        public void DecrementCounter(int value)
        {
            m_Counter -= value;
            CheckForRemoval();
        }

        public void DecrementMagnitude(int value)
        {
            m_Magnitude -= value;
            CheckForRemoval();
        }

        public int counter
        {
            get
            {
                return m_Counter;
            }
            protected set
            {
                m_Counter = value;
            }
        }

        public int magnitude
        {
            get
            {
                return m_Magnitude;
            }
            protected set
            {
                m_Magnitude = value;
            }
        }


        public int priority
        {
            get
            {
                return m_Priority;
            }
        }

        public bool readyForRemoval
        {
            get
            {
                return m_ReadyForRemoval;
            }
        }

        public bool permanent
        {
            get
            {
                return m_Counter == PERMANENT_ABILITY;
            }
        }

        public int manaCost
        {
            get
            {
                return m_ManaCost;
            }
        }

        public AbilityTarget targetType
        {
            get
            {
                return m_Target;
            }
        }
    }
}
