using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Needs
{
    public class Thirst : AbstractNeed
    {
        public Thirst() : base("thirst", 0, 1, true, 1, 1, 1, 1)
        {

        }

        public Thirst(string nameRef, 
            int decayRef, 
            int decayCounterRef, 
            bool doesDecayRef, 
            int priorityRef, 
            int happinessThresholdRef,
            int valueRef, 
            int maxValueRef, 
            int averageForDayRef = 0, 
            int averageForWeekRef = 0) : 

            base(nameRef,
                decayRef,
                decayCounterRef,
                doesDecayRef,
                priorityRef,
                happinessThresholdRef,
                valueRef,
                maxValueRef,
                averageForDayRef,
                averageForWeekRef)
        {

        }

        public override INeed Copy()
        {
            return new Thirst(
                this.m_Name,
                this.m_Decay,
                this.m_DecayCounter,
                this.m_DoesDecay,
                this.m_Priority,
                this.m_HappinessThreshold,
                this.m_Value,
                this.m_MaximumValue,
                this.m_AverageForDay,
                this.m_AverageForWeek
                );
        }

        public override bool FindFulfilmentObject(Entity actor)
        {
            string type = "drink";
            ItemInstance[] targets = actor.SearchBackpackForItemType(new string[] { type });
            int bestDrink = 0;
            ItemInstance chosenDrink = null;

            //Look for food in the target list
            foreach (ItemInstance target in targets)
            {
                if (target.ItemType.Value > bestDrink)
                {
                    bestDrink = target.ItemType.Value;
                    chosenDrink = target;
                }
            }

            //If we've found food, eat it
            if (chosenDrink != null)
            {
                this.Interact(actor, chosenDrink);
                actor.RemoveItemFromBackpack(chosenDrink);
                return true;
            }

            //Search the floor
            IEnumerable<JoyObject> objects = actor.MyWorld.SearchForObjects(actor, new string[] { type });
            foreach (JoyObject obj in objects)
            {
                if (!(obj is ItemInstance item))
                {
                    continue;
                }

                if (item.ItemType.Value > bestDrink)
                {
                    bestDrink = item.ItemType.Value;
                    chosenDrink = item;
                }
            }

            if (chosenDrink != null)
            {
                if (chosenDrink.WorldPosition.Equals(actor.WorldPosition))
                {
                    this.Interact(actor, chosenDrink);
                    actor.MyWorld.RemoveObject(chosenDrink.WorldPosition, chosenDrink);
                    return true;
                }
                else
                {
                    actor.Seek(chosenDrink, this.m_Name);
                }
            }

            actor.Wander();

            return false;
        }

        public override bool Interact(Entity user, JoyObject obj)
        {
            if (!(obj is ItemInstance item))
            {
                return false;
            }

            item.Interact(user);

            return true;
        }

        public override INeed Randomise()
        {
            return new Thirst(this.m_Name, 200, 200, true, 12, RNG.Roll(5, 24), 24, 0, 0);
        }
    }
}
