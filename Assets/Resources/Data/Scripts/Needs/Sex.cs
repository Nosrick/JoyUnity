using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }

        public override bool Interact(Entity user, JoyObject obj)
        {
            if (!(obj is Entity partner))
            {
                return false;
            }

            //partner

            return true;
        }

        public override INeed Randomise()
        {
            int decay = RNG.Roll(200, 600);
            return new Sex(this.m_Name, decay, decay, true, 12, RNG.Roll(5, 24), 24, 0, 0);
        }
    }
}
