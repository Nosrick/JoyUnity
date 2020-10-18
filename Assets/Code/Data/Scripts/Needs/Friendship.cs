using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class Friendship : AbstractNeed
    {
        protected static EntityRelationshipHandler s_EntityRelationshipHandler;

        protected const int DECAY_MIN = 4;
        protected const int DECAY_MAX = 128;

        protected const int PRIORITY_MIN = 0;
        protected const int PRIORITY_MAX = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 0;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;
        
        public Friendship() : 
            base(
                0, 
                1, 
                true, 
                1, 
                1, 
                1, 
                1,
                new [] { 
                    "seekaction",
                    "wanderaction",
                    "fulfillneedaction" })
        {
            Initialise();
        }
        
        public Friendship(
            int decayRef, 
            int decayCounterRef, 
            bool doesDecayRef, 
            int priorityRef, 
            int happinessThresholdRef, 
            int valueRef, 
            int maxValueRef,  
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
                new [] { 
                    "seekaction",
                    "wanderaction",
                    "fulfillneedaction" }, 
                averageForDayRef, 
                averageForWeekRef)
        {
            Initialise();
        }

        protected void Initialise()
        {
            if(s_EntityRelationshipHandler is null)
            {
                s_EntityRelationshipHandler = GameObject.Find("GameManager").GetComponent<EntityRelationshipHandler>();
            }
        }

        public override bool FindFulfilmentObject(Entity actor)
        {
            IEnumerable<string> tags = actor.Tags.Where(x => x.Contains("sentient"));

            List<Entity> possibleListeners = actor.MyWorld.SearchForEntities(actor, tags).ToList();

            if (possibleListeners.Count == 0)
            {
                return false;
            }

            Entity bestMatch = null;
            int bestRelationship = int.MinValue;
            foreach (Entity possible in possibleListeners)
            {
                List<long> participants = new List<long>();
                participants.Add(actor.GUID);
                participants.Add(possible.GUID);

                string[] relationshipTags = new[] {"friendship"};
                IRelationship[] relationships =
                    s_EntityRelationshipHandler.Get(participants.ToArray(), relationshipTags);

                foreach (IRelationship relationship in relationships)
                {
                    int thisRelationship = relationship.GetRelationshipValue(actor.GUID, possible.GUID);
                    if (bestRelationship < thisRelationship)
                    {
                        thisRelationship = bestRelationship;
                        bestMatch = possible;
                    }
                }
            }

            if (bestMatch is null)
            {
                m_CachedActions["wanderaction"].Execute(
                    new JoyObject[] {actor},
                    new[] {"wander", "need", "friendship"},
                    new object[] {});
                return false;
            }

            m_CachedActions["seekaction"].Execute(
                new JoyObject[] {actor},
                new[] {"need", "seek", "friendship"},
                new object[] {bestMatch});
            return true;
        }

        public override bool Interact(Entity user, JoyObject obj)
        {
            m_CachedActions["fulfillneedaction"].Execute(
                new[] {user, obj},
                new[] {"need", "friendship", "fulfill"},
                new object[] {"friendship", user.Statistics[EntityStatistic.PERSONALITY].Value, 5});
            return true;
        }

        public override INeed Copy()
        {
            return new Friendship(
                this.m_Decay,
                this.m_DecayCounter,
                this.m_DoesDecay,
                this.m_Priority,
                this.m_HappinessThreshold,
                this.m_Value,
                this.m_MaximumValue,
                this.AverageForDay,
                this.AverageForWeek);
        }

        public override INeed Randomise()
        {
            int decay = RNG.instance.Roll(DECAY_MIN, DECAY_MAX);
            int decayCounter = RNG.instance.Roll(0, DECAY_MAX);
            int priority = RNG.instance.Roll(PRIORITY_MIN, PRIORITY_MAX);
            int happinessThreshold = RNG.instance.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX);
            int value = RNG.instance.Roll(0, HAPPINESS_THRESHOLD_MAX);
            int maxValue = RNG.instance.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX);
            
            return new Friendship(
                decay,
                decayCounter,
                true,
                priority,
                happinessThreshold,
                value,
                maxValue);
        }

        public override string Name => "friendship";
    }
}