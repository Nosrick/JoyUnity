using System.Linq;
using System.Collections.Generic;
using JoyLib.Code.Rollers;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Relationships;
using System;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class Sex : AbstractNeed
    {
        private static EntityRelationshipHandler s_EntityRelationshipHandler;

        public Sex() : 
            base(
                0, 
                1, 
                true, 
                1, 
                1, 
                1, 
                1, 
                new string[] { 
                    "seekaction",
                    "wanderaction",
                    "fulfillneedaction"
                })
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
                new string[] { 
                    "seekaction",
                    "wanderaction",
                    "fulfillneedaction"
                 },
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
                this.m_AverageForDay,
                this.m_AverageForWeek);
        }

        public override bool FindFulfilmentObject(Entity actor)
        {
            IEnumerable<string> tags = actor.Tags.Where(x => x.Contains("sentient"));

            List<Entity> possibleMates = actor.MyWorld.SearchForEntities(actor, tags).ToList();

            if (possibleMates.Count == 0)
            {
                return false;
            }

            Entity bestMate = null;
            int bestRelationship = actor.Sexuality.MatingThreshold;
            foreach (Entity mate in possibleMates)
            {
                List<long> participants = new List<long>();
                participants.Add(actor.GUID);
                participants.Add(mate.GUID);
                string[] relationshipTags = new string[] { "sexual" };
                IRelationship[] relationships = s_EntityRelationshipHandler.Get(participants.ToArray(), relationshipTags);

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

            if (bestMate != null)
            {
                m_CachedActions["seekaction"].Execute(
                    new JoyObject[] { actor },
                    new string[] { "need", "seek", "sex" },
                    new object[] { bestMate });
                return true;
            }
            else
            {
                m_CachedActions["wanderaction"].Execute(
                    new JoyObject[] { actor },
                    new string[] { "need", "wander", "sex" },
                    new object[] {});
                return true;
            }
        }

        public override bool Interact(Entity user, JoyObject obj)
        {
            if (!(obj is Entity partner))
            {
                return false;
            }

            if (user.Sexuality.WillMateWith(user, partner, 
                s_EntityRelationshipHandler.Get(
                    new long[] { user.GUID, partner.GUID },
                    new string[] { "sexual" })))
            {
                int satisfaction = CalculateSatisfaction(
                    new Entity[] { user, partner },
                    new string[] {
                        EntityStatistic.ENDURANCE,
                        EntityStatistic.CUNNING,
                        EntityStatistic.PERSONALITY });

                int time = RNG.instance.Roll(5, 30);

                if (user.FulfillmentData.Name.Equals(this.Name) && 
                partner.FulfillmentData.Name.Equals(this.Name))
                {
                    HashSet<JoyObject> userParticipants = new HashSet<JoyObject>(user.FulfillmentData.Targets);
                    userParticipants.Add(user);
                    userParticipants.Add(partner);
                    m_CachedActions["fulfillneedaction"].Execute(
                        userParticipants.ToArray(),
                        new string[] { "sex", "need", "fulfill" },
                        new object[] { this.Name, satisfaction, time });

                    HashSet<JoyObject> partnerParticipants = new HashSet<JoyObject>(partner.FulfillmentData.Targets);
                    partnerParticipants.Add(partner);
                    partnerParticipants.Add(user);
                    m_CachedActions["fulfillneedaction"].Execute(
                        partnerParticipants.ToArray(),
                        new string[] { "sex", "need", "fulfill" },
                        new object[] { this.Name, satisfaction, time });
                }
            }

            return true;
        }

        protected int CalculateSatisfaction(IEnumerable<Entity> participants, IEnumerable<string> tags)
        {
            int satisfaction = 0;
            int total = 0;
            foreach (Entity participant in participants)
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
            int decay = RNG.instance.Roll(200, 600);
            return new Sex(decay, decay, true, 12, RNG.instance.Roll(5, 24), 24, 0, 0);
        }

        public override string Name
        {
            get
            {
                return "sex";
            }
        }
    }
}
