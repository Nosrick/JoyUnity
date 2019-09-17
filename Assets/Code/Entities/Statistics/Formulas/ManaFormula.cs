using System.Linq;

namespace JoyLib.Code.Entities.Statistics.Formulas
{
    public class ManaFormula : IValueFormula
    {
        public int Calculate(IBasicValue[] components)
        {
            return components.Sum(x => x.Value);
        }
    }
}
