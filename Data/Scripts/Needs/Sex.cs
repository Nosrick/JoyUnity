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

        public Sex() : base(s_Name, 0, 1, true, 1, 1, 1, 1)
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
                List<IRelationship> relationships = EntityRelationshipHandler.Get(participants.ToArray(), relationshipTags);

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
                actor.Seek(bestMate, "sex");
                return true;
            }

            return false;
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
                        EntityStatistic.INTELLECT,
                        EntityStatistic.PERSONALITY });

                int time = RNG.Roll(5, 30);


                if (user.FulfillmentData.Name.Equals(this.Name))
                {
                    if (user.FulfillmentData.Name.Equals(this.Name))
                    {
                        HashSet<JoyObject> newTargets = new HashSet<JoyObject>(user.FulfillmentData.Targets);
                        newTargets.Add(partner);
                        user.FulfillNeed(this.Name, satisfaction, newTargets.ToArray(), time);
                    }
                }
                else
                {
                    user.FulfillNeed(this.Name, satisfaction, new JoyObject[] { partner }, time);
                }

                if (partner.FulfillmentData.Name.Equals(this.Name))
                {
                    HashSet<JoyObject> newTargets = new HashSet<JoyObject>(partner.FulfillmentData.Targets);
                    newTargets.Add(user);
                    partner.FulfillNeed(this.Name, satisfaction, newTargets.ToArray(), time);
                }
                else
                {
                    partner.FulfillNeed(this.Name, satisfaction, new JoyObject[] { user }, time);
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
            int decay = RNG.Roll(200, 600);
            return new Sex(this.m_Name, decay, decay, true, 12, RNG.Roll(5, 24), 24, 0, 0);
        }
    }
}
