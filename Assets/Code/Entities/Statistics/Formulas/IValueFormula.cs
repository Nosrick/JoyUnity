namespace JoyLib.Code.Entities.Statistics.Formulas
{
    public interface IValueFormula
    {
        int Calculate(IBasicValue[] components);
    }
}
