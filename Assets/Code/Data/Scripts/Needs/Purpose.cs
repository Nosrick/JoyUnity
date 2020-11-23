using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class Purpose : AbstractNeed
    {
        public override string Name => "purpose";

        protected const int DECAY_MIN = 4;
        protected const int DECAY_MAX = 128;

        protected const int PRIORITY_MIN = 0;
        protected const int PRIORITY_MAX = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 0;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;

        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;

        protected static EntityRelationshipHandler RelationshipHandler
        {
            get;
            set;
        }

        protected static QuestProvider QuestProvider
        {
            get;
            set;
        }

        public Purpose()
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
        }

        public Purpose(
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
        }

        protected void Initialise()
        {
            if (RelationshipHandler is null)
            {
                RelationshipHandler = GameObject.Find("GameManager").GetComponent<EntityRelationshipHandler>();
                QuestProvider = GameObject.Find("GameManager").GetComponent<QuestProvider>();
            }
        }

        //Currently, the questing and employment systems are not (fully) in.
        //This will just seek out a random person and ask for a quest stub.
        public override bool FindFulfilmentObject(Entity actor)
        {
            IEnumerable<string> tags = actor.Tags.Where(x => x.Contains("sentient"));

            List<Entity> possibleListeners = actor.MyWorld.SearchForEntities(actor, tags).ToList();

            Entity bestMatch = null;
            int bestRelationship = int.MinValue;
            foreach (Entity possible in possibleListeners)
            {
                List<JoyObject> participants = new List<JoyObject>();
                participants.Add(actor);
                participants.Add(possible);

                string[] relationshipTags = new[] {"friendship"};
                IRelationship[] relationships = RelationshipHandler.Get(participants.ToArray(), relationshipTags);

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
                m_CachedActions["wanderaction"].Execute(
                    new JoyObject[] {actor},
                    new[] {"wander", "need", "purpose"});
                return false;
            }

            m_CachedActions["seekaction"].Execute(
                new JoyObject[] {actor, bestMatch},
                new[] {"need", "seek", "purpose"},
                new object[] {"purpose"});
            return true;
        }

        public override bool Interact(Entity actor, IJoyObject obj)
        {
            if (!(obj is Entity listener))
            {
                return false;
            }
            
            //Asking to do something for your friend increases your relationship
            m_CachedActions["fulfillneedaction"].Execute(
                new JoyObject[] {actor, listener},
                new[] {"need", "friendship", "fulfill"},
                new object[] {"friendship", actor.Statistics[EntityStatistic.PERSONALITY].Value, 0, true});

            if (RelationshipHandler.IsFamily(actor, listener))
            {
                m_CachedActions["fulfillneedaction"].Execute(
                    new JoyObject[] {actor, listener},
                    new[] {"need", "family", "fulfill"},
                    new object[] {"family", actor.Statistics[EntityStatistic.PERSONALITY].Value, 0, true});
            }
            
            actor.AddQuest(
                QuestProvider.MakeRandomQuest(
                    actor, 
                    listener, 
                    actor.MyWorld.GetOverworld()));

            m_CachedActions["fulfillneedaction"].Execute(
                new JoyObject[] {actor},
                new[] {"need", "purpose", "fulfill"},
                new object[] {"purpose", listener.Statistics[EntityStatistic.PERSONALITY].Value, 0, false});

            return true;
        }

        public override INeed Copy()
        {
            return new Purpose(
                this.m_Decay,
                this.m_DecayCounter,
                this.m_DoesDecay,
                this.m_Priority,
                this.m_HappinessThreshold,
                this.m_Value,
                this.m_MaximumValue,
                this.FulfillingSprite,
                this.AverageForDay);
        }

        public override INeed Randomise()
        {
            int decay = RNG.instance.Roll(DECAY_MIN, DECAY_MAX);
            int decayCounter = RNG.instance.Roll(0, DECAY_MAX);
            int priority = RNG.instance.Roll(PRIORITY_MIN, PRIORITY_MAX);
            int happinessThreshold = RNG.instance.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX);
            int value = RNG.instance.Roll(0, HAPPINESS_THRESHOLD_MAX);
            int maxValue = RNG.instance.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX);
            
            return new Purpose(
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