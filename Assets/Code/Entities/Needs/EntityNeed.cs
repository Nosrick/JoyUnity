using JoyLib.Code.Entities.AI;
using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public class EntityNeed
    {
        protected string m_Name;

        //How quickly the need decays
        //The higher the number, the slower it decays
        protected int m_Decay;
        protected int m_DecayCounter;
        protected bool m_DoesDecay;

        //How much of an impacy this need has on overall happiness
        protected int m_Priority;

        //How high the value has to be before it contributes to happiness
        protected int m_HappinessThreshold;

        //Current value
        protected int m_Value;
        protected int m_MaximumValue;

        //Average for the day
        //Will be calculated by adding the value every hour, then dividing by 24 when the day is up
        protected int m_AverageForDay;

        //Average for the week
        //Calculated by adding value for the day every day, then dividing by 7 when the week is up
        protected int m_AverageForWeek;

        //Bonus to health for having no diseases
        protected const int CLEAN_BONUS = 50;

        protected Entity m_Parent;

        protected NeedAIData m_NeedData;

        protected NeedAbstract m_NeedProvider;

        public EntityNeed(int decay, int priority, bool doesDecay, int value, int maximumValue,
            int happinessThreshold, int averageForDay, int averageForWeek, string name)
        {
            this.name = name;
            m_Decay = decay;
            m_DecayCounter = m_Decay;
            m_DoesDecay = doesDecay;
            m_Priority = priority;
            m_HappinessThreshold = happinessThreshold;
            m_Value = value;
            m_MaximumValue = maximumValue;
            m_AverageForDay = averageForDay;
            m_AverageForWeek = averageForWeek;

            //m_NeedProvider = NeedHandler.Get(name);

            m_NeedData.target = null;
        }

        public void SetParent(Entity parentRef)
        {
            parent = parentRef;
        }

        //This will be called once per in-game minute
        public void Tick()
        {
            m_DecayCounter -= 1;
            if (m_DecayCounter == 0 && m_DoesDecay)
            {
                m_DecayCounter = m_Decay;
                Decay(1);
            }

            if(!contributingHappiness && !parent.PlayerControlled)
            {
                SeekFulfilment();
            }

            //PythonEngine.ExecuteClassFunction(m_FilePath, m_PythonObject, "OnTick", new dynamic[] { this, parent });
            //m_NeedProvider.OnTick(this, parent);
        }

        public void ClearTarget()
        {
            m_NeedData = new NeedAIData();
        }

        private void SeekFulfilment()
        {
            if (m_NeedData.target == null)
            {
                //TODO: FIX THIS
                //m_NeedData = PythonEngine.ExecuteClassFunction(m_FilePath, m_PythonObject, "FindFulfilmentObject", new[] { parent });
                //m_NeedData = m_NeedProvider.FindFulfilmentObject(parent);
            }
            if(m_NeedData.target == null)
            {
                m_NeedData.searching = true;
                LookForPoint();
            }
        }

        private void LookForPoint()
        {
            //Choose a visible point to go towards
            List<Vector2Int> visiblePoints = new List<Vector2Int>();
            int perceptionMod = (int)parent.Statistics[StatisticIndex.Perception] / 10 + 1;
            for (int i = parent.WorldPosition.x - perceptionMod; i <= parent.WorldPosition.x + perceptionMod; i++)
            {
                if (i < 0)
                    continue;

                if (i >= parent.Vision.GetLength(0))
                    continue;

                for (int j = parent.WorldPosition.y - perceptionMod; j <= parent.WorldPosition.y + perceptionMod; j++)
                {
                    if (j < 0)
                        continue;

                    if (j >= parent.Vision.GetLength(1))
                        continue;

                    if (parent.Vision[i, j])
                    {
                        Vector2 point = new Vector2(i, j);
                        if (Vector2.Distance(point, parent.WorldPosition) <= perceptionMod)
                            visiblePoints.Add(new Vector2Int(i, j));
                    }
                }
            }
            if (visiblePoints.Count > 0)
            {
                m_NeedData.targetPoint = visiblePoints[RNG.Roll(0, visiblePoints.Count - 1)];
            }
        }

        public void Fulfill(int value)
        {
            m_Value = Math.Min(m_MaximumValue, m_Value + value);
            m_Value = Math.Max(0, m_Value);
        }

        public void Decay(int value)
        {
            m_Value = Math.Max(0, m_Value - value);
        }

        public static Dictionary<NeedIndex, EntityNeed> GetBasicRandomisedNeeds()
        {
            Dictionary<NeedIndex, EntityNeed> needs = new Dictionary<NeedIndex, EntityNeed>();

            //Add tier 1 needs
            needs.Add(NeedIndex.Hunger, new EntityNeed(60, 200, true, RNG.Roll(5, 24),
                24, 12, 0, 0, "Hunger"));
            needs.Add(NeedIndex.Drink, new EntityNeed(50, 200, true, RNG.Roll(5, 20),
                20, 10, 0, 0, "Drink"));
            needs.Add(NeedIndex.Sleep, new EntityNeed(60, 200, true, 12, 24, 8, 0, 0, "Sleep"));
            needs.Add(NeedIndex.Sex, new EntityNeed(240, RNG.Roll(10, 240), true, RNG.Roll(50, 200), 
                240, RNG.Roll(0, 240), 0, 0, "Sex"));

            return needs;
        }

        public static Dictionary<NeedIndex, EntityNeed> GetFullRandomisedNeeds()
        {
            Dictionary<NeedIndex, EntityNeed> needs = GetBasicRandomisedNeeds();

            //Add tier 2 needs
            //Health is an aggregate of food, drink and sleep, and gets a flat bonus of 50 for having no diseases
            int health = ((needs[NeedIndex.Hunger].value + needs[NeedIndex.Drink].value + needs[NeedIndex.Sleep].value) / 3) + CLEAN_BONUS;

            needs.Add(NeedIndex.Health, new EntityNeed(0, RNG.Roll(50, 150), false, health, 300, RNG.Roll(100, 200), 0, 0, "Health"));
            needs.Add(NeedIndex.Employment, new EntityNeed(RNG.Roll(200, 400), RNG.Roll(10, 150), true, 0, 300, RNG.Roll(50, 200), 0, 0, "Employment"));
            needs.Add(NeedIndex.Property, new EntityNeed(0, RNG.Roll(10, 150), false, 0, 300, RNG.Roll(50, 250), 0, 0, "Property"));

            //Add tier 3 needs
            needs.Add(NeedIndex.Friendship, new EntityNeed(RNG.Roll(24, 72), RNG.Roll(10, 150), true, 0, 300, RNG.Roll(10, 200), 0, 0, "Friendship"));
            needs.Add(NeedIndex.Family, new EntityNeed(0, RNG.Roll(10, 50), false, 0, 300, 100, 0, 0, "Family"));
            needs.Add(NeedIndex.Morality, new EntityNeed(0, RNG.Roll(0, 150), false, 150, 300, 150, 0, 0, "Morality"));

            //Add tier 4 needs
            needs.Add(NeedIndex.Respect, new EntityNeed(0, RNG.Roll(10, 150), false, 0, int.MaxValue, RNG.Roll(10, 200), 0, 0, "Respect"));
            int confidence = needs.Sum(x => x.Value.value) / needs.Count;
            needs.Add(NeedIndex.Confidence, new EntityNeed(RNG.Roll(24, 148), RNG.Roll(50, 150), true, confidence, 300, RNG.Roll(50, 200), 0, 0, "Confidence"));

            return needs;
        }

        public string name
        {
            get
            {
                return m_Name;
            }
            protected set
            {
                m_Name = value;
            }
        }

        public int value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        public int decay
        {
            get
            {
                return m_Decay;
            }
        }

        public int happinessThreshold
        {
            get
            {
                return m_HappinessThreshold;
            }
        }

        public bool contributingHappiness
        {
            get
            {
                return m_Value >= m_HappinessThreshold;
            }
        }

        public int priority
        {
            get
            {
                return m_Priority;
            }
        }

        public int averageForDay
        {
            get
            {
                return m_AverageForDay;
            }
        }

        public int averageForWeek
        {
            get
            {
                return m_AverageForWeek;
            }
        }
        
        public NeedAIData needData
        {
            get
            {
                return m_NeedData;
            }
        }

        protected Entity parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                m_Parent = value;
                if(m_Parent.Sexuality == Sexuality.Asexual && name.Equals("Sex"))
                {
                    m_Priority = 0;
                    m_HappinessThreshold = 0;
                    m_Value = 1;
                    m_DoesDecay = false;
                }
            }
        }
    }
}
