using System.Linq;

namespace JoyLib.Code.Entities.Statistics.Formulas
{
    public class BasicDerivedValueFormula : IValueFormula
    {
        public int Calculate(IBasicValue[] components)
        {
            return (components.Sum(x => x.Value) * 3);
        }
    }
}
