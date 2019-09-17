using JoyLib.Code.Entities.Items;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Needs
{
    public class Hunger : AbstractNeed
    {
        public Hunger() : base("Hunger", 0, 1, true, 1, 1, 1, 1)
        {

        }

        public Hunger(string nameRef, int decayRef, int decayCounterRef, bool doesDecayRef, int priorityRef, 
            int happinessThresholdRef, int valueRef, int maxValueRef, int averageForDayRef = 0, int averageForWeekRef = 0) : 
            base(nameRef, decayRef, decayCounterRef, doesDecayRef, priorityRef, 
                happinessThresholdRef, valueRef, maxValueRef, averageForDayRef, averageForWeekRef)
        {
        }

        public override INeed Copy()
        {
            return new Hunger(this.m_Name, this.m_Decay, this.m_DecayCounter, this.m_DoesDecay, this.m_Priority, this.m_HappinessThreshold,
                this.m_Value, this.m_MaximumValue, this.m_AverageForDay, this.m_AverageForWeek);
        }

        public override bool FindFulfilmentObject(Entity actor)
        {
            ItemInstance[] targets = actor.SearchBackpackForItemType(new string[] { "food" });
            int bestFood = 0;
            ItemInstance chosenFood = null;

            //Look for food in the target list
            foreach(ItemInstance target in targets)
            {
                if(target.ItemType.Value > bestFood)
                {
                    bestFood = target.ItemType.Value;
                    chosenFood = target;
                }
            }

            //If we've found food, eat it
            if(chosenFood != null)
            {
                this.Interact(actor, chosenFood);
                actor.RemoveItemFromBackpack(chosenFood);
                return true;
            }

            //Search the floor
            List<AI.NeedAIData> intents = actor.MyWorld.SearchForObjects(actor, new string[] { "food" }, AI.Intent.Interact);
            foreach(AI.NeedAIData intent in intents)
            {
                if(intent.target.GetType() == typeof(ItemInstance))
                {
                    ItemInstance tempItem = (ItemInstance)intent.target;
                    if(tempItem.ItemType.Value > bestFood)
                    {
                        bestFood = tempItem.ItemType.Value;
                        chosenFood = tempItem;
                    }
                }
            }

            if(chosenFood != null)
            {
                if(chosenFood.WorldPosition.Equals(actor.WorldPosition))
                {
                    this.Interact(actor, chosenFood);
                    actor.MyWorld.RemoveObject(chosenFood.WorldPosition, chosenFood);
                    return true;
                }
                else
                {
                    actor.Seek(chosenFood, "Hunger");
                }
            }

            actor.Wander();

            return false;
        }

        public override bool Interact(Entity user, JoyObject obj)
        {
            ItemInstance item = obj as ItemInstance;

            if(item == null)
            {
                return false;
            }

            item.Interact(user);

            return true;
        }

        public override INeed Randomise()
        {
            return new Hunger("Hunger", 60, 200, true, RNG.Roll(5, 24), 24, 12, 0, 0);
        }
    }
}
