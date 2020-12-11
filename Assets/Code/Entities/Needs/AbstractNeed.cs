using System;
using System.Collections.Generic;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Entities.Needs
{
    public abstract class AbstractNeed : INeed
    {
        public virtual string Name
        {
            get => "abstractneed";
            set {}
        }

        public Sprite FulfillingSprite { get; protected set; }

        public RNG Roller { get; protected set; }

        protected Dictionary<string, IJoyAction> m_CachedActions;

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
        protected int m_AverageForMonth;

        public AbstractNeed(
            int decayRef, 
            int decayCounterRef, 
            bool doesDecayRef, 
            int priorityRef, 
            int happinessThresholdRef,
            int valueRef, 
            int maxValueRef, 
            string[] actions,
            Sprite fulfillingSprite = null,
            int averageForDayRef = 0, 
            int averageForWeekRef = 0,
            RNG roller = null)
        {
            Roller = roller is null ? new RNG() : roller; 
            m_CachedActions = new Dictionary<string, IJoyAction>();

            IJoyAction[] standardActions = FetchStandardActions();

            foreach (IJoyAction action in standardActions)
            {
                m_CachedActions.Add(action.Name, action);
            }
            
            m_Decay = decayRef;
            m_DecayCounter = decayCounterRef;
            m_DoesDecay = doesDecayRef;

            m_Priority = priorityRef;

            m_HappinessThreshold = happinessThresholdRef;

            m_Value = valueRef;
            m_MaximumValue = maxValueRef;

            m_AverageForDay = averageForDayRef;
            m_AverageForWeek = averageForWeekRef;

            FulfillingSprite = fulfillingSprite;

            foreach(string action in actions)
            {
                m_CachedActions.Add(action, ScriptingEngine.instance.FetchAction(action));
            }

            if (GlobalConstants.GameManager is null == false)
            {
                this.FulfillingSprite = GlobalConstants.GameManager.ObjectIconHandler.GetSprite("needs", this.Name);
            }
        }

        public abstract bool FindFulfilmentObject(IEntity actor);

        public abstract INeed Copy();

        public abstract INeed Randomise();

        //This will be called once per in-game minute
        public virtual bool Tick(Entity actor)
        {
            m_DecayCounter -= 1;
            if (m_DecayCounter == 0 && m_DoesDecay)
            {
                m_DecayCounter = m_Decay;
                Decay(1);
                return true;
            }
            return false;
        }

        public virtual int Fulfill(int value)
        {
            return ModifyValue(value);
        }

        public virtual int Decay(int value)
        {
            return ModifyValue(-value);
        }

        public int ModifyValue(int value)
        {
            m_Value = Math.Max(0, Math.Min(m_MaximumValue, m_Value + value));
            return m_Value;
        }

        public abstract bool Interact(IEntity actor, IJoyObject obj);

        public int SetValue(int value)
        {
            m_Value = Math.Max(0, Math.Min(m_MaximumValue, value));
            return m_Value;
        }

        protected IJoyAction[] FetchStandardActions()
        {
            List<IJoyAction> actions = new List<IJoyAction>();
            actions.Add(ScriptingEngine.instance.FetchAction("seekaction"));
            actions.Add(ScriptingEngine.instance.FetchAction("wanderaction"));
            actions.Add(ScriptingEngine.instance.FetchAction("fulfillneedaction"));

            return actions.ToArray();
        }

        public int Priority
        {
            get
            {
                return m_Priority;
            }
            protected set
            {
                m_Priority = value;
            }
        }

        public bool ContributingHappiness
        {
            get
            {
                return m_Value >= m_HappinessThreshold;
            }
        }

        public int Value
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

        public int AverageForDay
        {
            get
            {
                return m_AverageForDay;
            }
            protected set
            {
                m_AverageForDay = value;
            }
        }

        public int AverageForWeek
        {
            get
            {
                return m_AverageForWeek;
            }
            protected set
            {
                m_AverageForWeek = value;
            }
        }

        public int AverageForMonth
        {
            get
            {
                return m_AverageForMonth;
            }
            protected set
            {
                m_AverageForMonth = value;
            }
        }

        public int HappinessThreshold => m_HappinessThreshold;
    }
}
