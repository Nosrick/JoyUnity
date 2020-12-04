using System.Linq;
using System.Collections.Generic;
using JoyLib.Code.Rollers;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Relationships;
using System;
using JoyLib.Code.Entities.AI;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class Sex : AbstractNeed
    {
        public override string Name => "sex";
        
        protected static IEntityRelationshipHandler EntityRelationshipHandler { get; set; }

        protected const int DECAY_MIN = 200;
        protected const int DECAY_MAX = 600;

        protected const int PRIORITY = 12;

        protected const int HAPPINESS_THRESHOLD_MIN = 5;
        protected const int HAPPINESS_THRESHOLD_MAX = 24;
        
        protected const int MAX_VALUE_MIN = HAPPINESS_THRESHOLD_MAX;
        protected const int MAX_VALUE_MAX = MAX_VALUE_MIN * 4;
        

        public Sex() : 
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
            Initialise();
        }

        public Sex(
            int decayRef,
            int decayCounterRef,
            bool doesDecayRef,
            int priorityRef,
            int happinessThresholdRef,
            int valueRef,
            int maxValueRef,
            Sprite fulfillingSprite,
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
                fulfillingSprite,
                averageForDayRef,
                averageForWeekRef)
        {
            Initialise();
        }

        protected void Initialise()
        {
            if(GlobalConstants.GameManager is null == false && EntityRelationshipHandler is null)
            {
                EntityRelationshipHandler = GlobalConstants.GameManager.RelationshipHandler;
            }
        }

        public override INeed Copy()
        {
            return new Sex(
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

        public override bool FindFulfilmentObject(IEntity actor)
        {
            IEnumerable<string> tags = actor.Tags.Where(x => x.Contains("sentient"));

            List<IEntity> possibleMates = actor.MyWorld.SearchForEntities(actor, tags).ToList();

            IEntity bestMate = null;
            int bestRelationship = actor.Sexuality.MatingThreshold;
            foreach (IEntity mate in possibleMates)
            {
                List<IJoyObject> participants = new List<IJoyObject>();
                participants.Add(actor);
                participants.Add(mate);
                string[] relationshipTags = new string[] { "sexual" };
                IEnumerable<IRelationship> relationships = EntityRelationshipHandler.Get(participants, relationshipTags);

                foreach (IRelationship relationship in relationships)
                {
                    int thisRelationship = relationship.GetRelationshipValue(actor.GUID, mate.GUID);
                    if (thisRelationship >= bestRelationship)
                    {
                        bestRelationship = thisRelationship;
                        bestMate = mate;
                    }
                }
            }

            if (bestMate is null)
            {
                m_CachedActions["wanderaction"].Execute(
                    new IJoyObject[] { actor },
                    new [] { "need", "wander", "sex" },
                    new object[] {});
                return false;
            }
            else
            {
                m_CachedActions["seekaction"].Execute(
                    new IJoyObject[] { actor, bestMate },
                    new [] { "need", "seek", "sex" },
                    new object[] { "sex" });
                return true;
            }
        }

        public override bool Interact(IEntity actor, IJoyObject obj)
        {
            if (!(obj is IEntity partner))
            {
                return false;
            }

            if (actor.Sexuality.WillMateWith(actor, partner, 
                EntityRelationshipHandler.Get(
                    new IJoyObject[] { actor, partner },
                    new string[] { "sexual" })))
            {
                int satisfaction = CalculateSatisfaction(
                    new IEntity[] { actor, partner },
                    new string[] {
                        EntityStatistic.ENDURANCE,
                        EntityStatistic.CUNNING,
                        EntityStatistic.PERSONALITY });

                int time = Roller.Roll(5, 30);

                if (actor.FulfillmentData.Name.Equals(this.Name) && 
                    partner.FulfillmentData.Name.Equals(this.Name))
                {
                    HashSet<IJoyObject> userParticipants = new HashSet<IJoyObject>(actor.FulfillmentData.Targets);
                    userParticipants.Add(actor);
                    userParticipants.Add(partner);
                    m_CachedActions["fulfillneedaction"].Execute(
                        userParticipants.ToArray(),
                        new string[] { "sex", "need", "fulfill" },
                        new object[] { this.Name, satisfaction, time });

                    HashSet<IJoyObject> partnerParticipants = new HashSet<IJoyObject>(partner.FulfillmentData.Targets);
                    partnerParticipants.Add(partner);
                    partnerParticipants.Add(actor);
                    m_CachedActions["fulfillneedaction"].Execute(
                        partnerParticipants.ToArray(),
                        new string[] { "sex", "need", "fulfill" },
                        new object[] { this.Name, satisfaction, time });
                }
            }

            return true;
        }

        protected int CalculateSatisfaction(IEnumerable<IEntity> participants, IEnumerable<string> tags)
        {
            int satisfaction = 0;
            int total = 0;
            foreach (IEntity participant in participants)
            {
                Tuple<string, int>[] data = participant.GetData(tags.ToArray());
                int subTotal = 0;
                foreach (Tuple<string, int> tuple in data)
                {
                    subTotal += tuple.Item2;
                }
                subTotal /= data.Length;
                total += subTotal;
            }

            satisfaction = total / participants.Count();

            return satisfaction;
        }

        public override INeed Randomise()
        {
            int decay = Roller.Roll(DECAY_MIN, DECAY_MAX);
            int decayCounter = Roller.Roll(0, DECAY_MAX);
            int maxValue = Roller.Roll(MAX_VALUE_MIN, MAX_VALUE_MAX);
            return new Sex(
                decay, 
                decayCounter, 
                true, 
                PRIORITY, 
                Roller.Roll(HAPPINESS_THRESHOLD_MIN, HAPPINESS_THRESHOLD_MAX), 
                HAPPINESS_THRESHOLD_MAX, 
                maxValue,
                this.FulfillingSprite);
        }
    }
}
