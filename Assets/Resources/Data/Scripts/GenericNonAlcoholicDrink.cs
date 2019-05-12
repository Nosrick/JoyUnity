using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Entities.Abilities
{
    public class GenericNonAlcoholicDrink : Ability
    {
        public GenericNonAlcoholicDrink(AbilityTrigger triggerRef, AbilityTarget targetRef, bool stackRef, string nameRef, string internalNameRef, string descriptionRef, int priorityRef, int counterRef, int magnitudeRef, int manaCost) : 
            base(triggerRef, targetRef, stackRef, nameRef, internalNameRef, descriptionRef, priorityRef, counterRef, magnitudeRef, manaCost)
        {
        }

        public override bool OnAttack(Entity attacker, Entity target)
        {
            return base.OnAttack(attacker, target);
        }

        public override int OnHeal(Entity receiver, int healing)
        {
            return base.OnHeal(receiver, healing);
        }

        public override bool OnKill(Entity attacker, Entity target)
        {
            return base.OnKill(attacker, target);
        }

        public override bool OnPickup(Entity entity, ItemInstance item)
        {
            return base.OnPickup(entity, item);
        }

        public override int OnTakeHit(Entity attacker, Entity defender, int damage)
        {
            return base.OnTakeHit(attacker, defender, damage);
        }

        public override void OnTick(Entity entity)
        {
            base.OnTick(entity);
        }

        public override bool Use(Entity user, JoyObject target)
        {
            if(target.GetType() == typeof(ItemInstance))
            {
                ItemInstance item = (ItemInstance)target;
                user.FulfillNeed("Thirst", item.ItemType.Value, 1);
                user.RemoveItemFromPerson(item);
                return true;
            }
            return false;
        }
    }
}
