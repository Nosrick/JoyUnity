using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class Friendship : AbstractNeed
    {
        public override string Name => "friendship";

        protected static IEntityRelationshipHandler EntityRelationshipHandler
        {
            get;
            set;
        }

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
                new[] { "modifyrelationshippointsaction"})
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
                new[] { "modifyrelationshippointsaction"},
                fulfillingSprite,
                averageForDayRef, 
                averageForWeekRef)
        {
            Initialise();
        }

        protected void Initialise()
        {
            if(EntityRelationshipHandler is null)
            {
                EntityRelationshipHandler = GlobalConstants.GameManager.RelationshipHandler;
            }
        }

        public override bool FindFulfilmentObject(Entity actor)
        {
            IEnumerable<string> tags = actor.Tags.Where(x => x.Contains("sentient"));

            List<Entity> possibleListeners = actor.MyWorld.SearchForEntities(actor, tags).ToList();

            Entity bestMatch = null;
            int bestRelationship = int.MinValue;
            foreach (Entity possible in possibleListeners)
            {
                List<IJoyObject> participants = new List<IJoyObject>();
                participants.Add(actor);
                participants.Add(possible);

                string[] relationshipTags = new[] {"friendship"};
                IRelationship[] relationships =
                    EntityRelationshipHandler.Get(participants.ToArray(), relationshipTags);
                
                foreach (IRelationship relationship in relationships)
                {
                    int thisRelationship = relationship.GetRelationshipValue(actor.GUID, possible.GUID);
                    if (bestRelationship < thisRelationship)
                    {
                        bestRelationship = thisRelationship;
                        bestMatch = possible;
                    }
                }
            }

            if (bestMatch is null)
            {
                bestMatch = possibleListeners[RNG.instance.Roll(0, possibleListeners.Count)];
            }

            m_CachedActions["seekaction"].Execute(
                new IJoyObject[] {actor, bestMatch},
                new[] {"need", "seek", "friendship"},
                new object[] {"friendship"});
            return true;
        }

        public override bool Interact(Entity actor, IJoyObject obj)
        {
            m_CachedActions["fulfillneedaction"].Execute(
                new[] {actor, obj},
                new[] {"need", "friendship", "fulfill"},
                new object[] {"friendship", actor.Statistics[EntityStatistic.PERSONALITY].Value, 5, true});

            m_CachedActions["modifyrelationshippointsaction"].Execute(
            new []{actor, obj},
            new[] { "friendship"},
            new object[] { actor.Statistics[EntityStatistic.PERSONALITY].Value, false });

            if (obj is Entity listener)
            {
                m_CachedActions["modifyrelationshippointsaction"].Execute(
                                new IJoyObject[]{listener, actor},
                                new[] { "friendship"},
                                new object[] { listener.Statistics[EntityStatistic.PERSONALITY].Value, false });
            }

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
                this.FulfillingSprite,
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
                maxValue,
                this.FulfillingSprite);
        }
    }
}