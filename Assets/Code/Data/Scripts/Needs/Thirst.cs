﻿using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using System.Collections.Generic;

namespace JoyLib.Code.Entities.Needs
{
    public class Thirst : AbstractNeed
    {
        protected const int DECAY = 200;
        protected const int PRIORITY = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 5;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;
        
        public Thirst() :
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

        public Thirst(
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
            return new Thirst(
                this.m_Decay,
                this.m_DecayCounter,
                this.m_DoesDecay,
                this.m_Priority,
                this.m_HappinessThreshold,
                this.m_Value,
                this.m_MaximumValue,
                this.AverageForDay,
                this.m_AverageForWeek);
        }

        public override bool FindFulfilmentObject(Entity actor)
        {
            string type = "drink";
            ItemInstance[] targets = actor.SearchBackpackForItemType(new string[] { type });
            int bestDrink = 0;
            ItemInstance chosenDrink = null;

            if (targets.Length == 0)
            {
                m_CachedActions["wanderaction"].Execute(
                    new JoyObject[] { actor },
                    new string[] { "need", "thirst", "wander" });

                return false;
            }

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
                    m_CachedActions["seekaction"].Execute(
                        new JoyObject[] { actor, chosenDrink },
                        new string[] { "need", "thirst", "seek" },
                        new object[] { "thirst" });
                    return true;
                }
            }

            m_CachedActions["wanderaction"].Execute(
                new JoyObject[] { actor },
                new string[] { "need", "thirst", "wander" });

            return false;
        }

        public override bool Interact(Entity actor, JoyObject obj)
        {
            if (!(obj is ItemInstance item))
            {
                return false;
            }

            item.Interact(actor);

            return true;
        }

        public override INeed Randomise()
        {
            return new Thirst(
                DECAY,
                DECAY, 
                true, 
                PRIORITY, 
                RNG.instance.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX), 
                HAPPINESS_THRESHOLD_MAX, 
                RNG.instance.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX));
        }

        public override string Name => "thirst";
    }
}
