using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Needs
{
    public class Hunger : AbstractNeed
    {
        protected const int DECAY = 200;
        protected const int PRIORITY = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 5;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;
        
        public Hunger() : 
            base(
                0, 
                1, 
                true, 
                1, 
                1, 
                1, 
                1,
                new string[0])
        {
        }

        public Hunger(
            int decayRef, 
            int decayCounterRef, 
            bool doesDecayRef, 
            int priorityRef, 
            int happinessThresholdRef, 
            int valueRef, 
            int maxValueRef, 
            int averageForDayRef = 0, 
            int averageForWeekRef = 0) : 

            base(
                decayRef, 
                decayCounterRef, 
                doesDecayRef, 
                priorityRef, 
                happinessThresholdRef, 
                valueRef, 
                maxValueRef, 
                new string[0],
                averageForDayRef, 
                averageForWeekRef)
        {
        }

        public override INeed Copy()
        {
            return new Hunger(
                this.m_Decay, 
                this.m_DecayCounter, 
                this.m_DoesDecay, 
                this.m_Priority, 
                this.m_HappinessThreshold,
                this.m_Value, 
                this.m_MaximumValue, 
                this.m_AverageForDay, 
                this.m_AverageForWeek);
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
            IEnumerable<JoyObject> objects = actor.MyWorld.SearchForObjects(actor, new string[] { "food" });
            foreach(JoyObject obj in objects)
            {
                if(!(obj is ItemInstance item))
                {
                    continue;
                }

                if(item.ItemType.Value > bestFood)
                {
                    bestFood = item.ItemType.Value;
                    chosenFood = item;
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
                    m_CachedActions["seekaction"].Execute(
                        new JoyObject[]{ actor, chosenFood },
                        new string[] { "seek", "need", "hunger" },
                        new object[] { "hunger" });
                    return true;
                }
            }

            m_CachedActions["wanderaction"].Execute(
                new JoyObject[]{ actor },
                new string[] { "wander", "need", "hunger" });

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
            return new Hunger(
                DECAY, 
                DECAY, 
                true, 
                PRIORITY, 
                RNG.instance.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX), 
                HAPPINESS_THRESHOLD_MAX, 
                RNG.instance.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX));
        }

        public override string Name => "hunger";
    }
}
