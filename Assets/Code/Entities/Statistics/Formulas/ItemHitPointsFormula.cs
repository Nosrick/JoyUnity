using System;

namespace JoyLib.Code.Entities.Statistics.Formulas
{
    public class ItemHitPointsFormula : IValueFormula
    {
        public int Calculate(IBasicValue[] components)
        {
            IBasicValue bonus = components[0];
            IBasicValue weight = components[1];

            int hitpoints = ((int)Math.Sqrt(weight.Value)) + bonus.Value;
            return hitpoints;
        }
    }
}
