using System.Linq;
using System.Collections.Generic;
using JoyLib.Code.Rollers;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Relationships;
using System;

namespace JoyLib.Code.Entities.Needs
{
    public class Sex : AbstractNeed
    {
        private readonly static string s_Name = "sex";

        public Sex() : 
            base(
                s_Name, 
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

        }

        public Sex(string nameRef,
            int decayRef,
            int decayCounterRef,
            bool doesDecayRef,
            int priorityRef,
            int happinessThresholdRef,
            int valueRef,
            int maxValueRef,
            int averageForDayRef = 0,
            int averageForWeekRef = 0) :

            base(nameRef,
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
        }

        public override INeed Copy()
        {
            return new Sex(
                this.m_Name,
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
                List<IRelationship> relationships = EntityRelationshipHandler.instance.Get(participants.ToArray(), relationshipTags);

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

            if (user.Sexuality.WillMateWith(user, partner))
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
            return new Sex(this.m_Name, decay, decay, true, 12, RNG.instance.Roll(5, 24), 24, 0, 0);
        }
    }
}
