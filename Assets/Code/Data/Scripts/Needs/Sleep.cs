using System.Collections.Generic;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class Sleep : AbstractNeed
    {
        public override string Name => "sleep";

        protected const int DECAY = 200;
        protected const int PRIORITY = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 5;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;

        public Sleep()
            : base(
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
        
        public Sleep(
            int decayRef, 
            int decayCounterRef, 
            bool doesDecayRef, 
            int priorityRef, 
            int happinessThresholdRef, 
            int valueRef, 
            int maxValueRef, 
            ISpriteState fulfillingSprite,
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
        }

        public override bool FindFulfilmentObject(IEntity actor)
        {
            Dictionary<Vector2Int, IJoyObject> objects = actor.MyWorld.GetObjectsOfType(new [] {"bed", "sleep"});
            
            foreach (KeyValuePair<Vector2Int, IJoyObject> pair in objects)
            {
                if (actor.MyWorld.GetEntity(pair.Key) is null)
                {
                    this.m_CachedActions["seekaction"].Execute(
                        new [] {actor, pair.Value},
                        new[] {"need", "sleep", "seek"},
                        new Dictionary<string, object>
                        {
                            {"need", "sleep"}
                        });
                    return true;
                }
            }

            this.m_CachedActions["wanderaction"].Execute(
                new IJoyObject[] {actor},
                new[] {"need", "sleep", "wander"});
            return false;

        }

        public override bool Interact(IEntity actor, IJoyObject obj)
        {
            if (!(obj is ItemInstance item))
            {
                return false;
            }
            
            item.Interact(actor);

            return true;
        }

        public override INeed Copy()
        {
            return new Sleep(
                this.m_Decay,
                this.m_DecayCounter,
                this.m_DoesDecay,
                this.m_Priority,
                this.m_HappinessThreshold,
                this.m_Value,
                this.m_MaximumValue,
                this.FulfillingSprite,
                this.AverageForDay,
                this.AverageForWeek);
        }

        public override INeed Randomise()
        {
            return new Sleep(
                DECAY,
                DECAY, 
                true, 
                PRIORITY, this.Roller.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX), 
                HAPPINESS_THRESHOLD_MAX, this.Roller.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX),
                this.FulfillingSprite);
        }
    }
}