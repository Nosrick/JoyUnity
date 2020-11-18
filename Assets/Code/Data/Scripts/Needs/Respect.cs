using System;
using System.Linq;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class Respect : AbstractNeed
    {
        public override string Name => "respect";

        protected const int DECAY_MIN = 4;
        protected const int DECAY_MAX = 128;

        protected const int PRIORITY_MIN = 0;
        protected const int PRIORITY_MAX = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 0;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;
        
        protected static EntityRelationshipHandler RelationshipHandler { get; set; }
        
        public Respect()
            : base(
                1,
                1,
                true,
                1,
                1,
                1,
                1,
                new string[0])
        {
            Initialise();
        }
        
        public Respect(
            int decayRef, 
            int decayCounterRef, 
            bool doesDecayRef, 
            int priorityRef, 
            int happinessThresholdRef, 
            int valueRef,
            int maxValueRef,
            Sprite fulfillingSprite,
            int averageForDayRef = 0,
            int averageForWeekRef = 0) 
            : base(
                decayRef,
                decayCounterRef, 
                doesDecayRef,
                priorityRef,
                happinessThresholdRef,
                valueRef,
                maxValueRef,
                new string[0],
                fulfillingSprite,
                averageForDayRef, 
                averageForWeekRef)
        {
            Initialise();
        }

        protected void Initialise()
        {
            if(RelationshipHandler is null)
            {
                RelationshipHandler = GlobalConstants.GameManager.GetComponent<EntityRelationshipHandler>();
            }
        }

        //This is to do with others, so look for something to do
        public override bool FindFulfilmentObject(Entity actor)
        {
            INeed[] needs = actor.Needs.Where(need => 
                need.Name.Equals("family", StringComparison.OrdinalIgnoreCase)
                || need.Name.Equals("friendship", StringComparison.OrdinalIgnoreCase)
                || need.Name.Equals("purpose", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            INeed chosenNeed = null;
            int bestMatch = Int32.MaxValue;
            foreach (INeed need in needs)
            {
                if (need.ContributingHappiness == false && bestMatch > need.Value)
                {
                    chosenNeed = need;
                    bestMatch = need.Value;
                }
            }

            //If this is true, then there are no needs that are not contributing happiness
            if (chosenNeed == null)
            {
                return true;
            }

            return chosenNeed.FindFulfilmentObject(actor);
        }

        public override bool Interact(Entity actor, IJoyObject obj)
        {
            return false;
        }

        public override INeed Copy()
        {
            return new Respect(
                this.m_Decay,
                this.m_DecayCounter,
                this.m_DoesDecay,
                this.m_Priority,
                this.m_HappinessThreshold,
                this.m_Value,
                this.m_MaximumValue,
                this.FulfillingSprite,
                this.m_AverageForDay,
                this.m_AverageForWeek);
        }

        public override INeed Randomise()
        {
            int decay = RNG.instance.Roll(DECAY_MIN, DECAY_MAX);
            int decayCounter = RNG.instance.Roll(0, DECAY_MAX);
            int priority = RNG.instance.Roll(PRIORITY_MIN, PRIORITY_MAX);
            int happinessThreshold = RNG.instance.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX);
            int value = RNG.instance.Roll(0, HAPPINESS_THRESHOLD_MAX);
            int maxValue = RNG.instance.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX);
            
            return new Respect(
                decay,
                decayCounter,
                true,
                priority,
                happinessThreshold,
                value,
                maxValue,
                this.FulfillingSprite);
        }

        public override bool Tick(Entity actor)
        {
            if (this.m_DecayCounter == 0 && m_DoesDecay)
            {
                IRelationship[] relationships = RelationshipHandler.GetAllForObject(actor);

                int average = (int)Math.Ceiling(
                    relationships.Average(relationship => 
                        relationship.GetHighestRelationshipValue(actor.GUID)));

                this.Fulfill(average);
                base.Tick(actor);
                return true;
            }

            base.Tick(actor);
            return false;
        }
    }
}