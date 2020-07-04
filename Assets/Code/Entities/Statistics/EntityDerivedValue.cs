using JoyLib.Code.Entities.Statistics.Formulas;
using JoyLib.Code.Collections;
using System;

namespace JoyLib.Code.Entities.Statistics
{
    public class EntityDerivedValue : IDerivedValue
    {
        public const string HITPOINTS = "hitpoints";
        public const string CONCENTRATION = "concentration";
        public const string COMPOSURE = "composure";
        public const string MANA = "mana";

        public EntityDerivedValue(string name, int value, int maximum)
        {
            Name = name;
            Value = value;
            Maximum = maximum;
        }

        public static BasicValueContainer<IDerivedValue> GetDefault(IBasicValue endurance, IBasicValue focus, IBasicValue wit)
        {
            BasicValueContainer<IDerivedValue> basicDerivedValues = new BasicValueContainer<IDerivedValue>();

            IValueFormula derivedValueFormula = new BasicDerivedValueFormula();
            IValueFormula manaFormula = new ManaFormula();

            int hitPoints = derivedValueFormula.Calculate(new IBasicValue[] { endurance });
            basicDerivedValues.Add(new EntityDerivedValue(HITPOINTS, hitPoints, hitPoints));

            int concentration = derivedValueFormula.Calculate(new IBasicValue[] { focus });
            basicDerivedValues.Add(new EntityDerivedValue(CONCENTRATION, concentration, concentration));

            int composure = derivedValueFormula.Calculate(new IBasicValue[] { wit });
            basicDerivedValues.Add(new EntityDerivedValue(COMPOSURE, composure, composure));

            int mana = manaFormula.Calculate(new IBasicValue[] { endurance, focus, wit });
            basicDerivedValues.Add(new EntityDerivedValue(MANA, mana, mana));

            return basicDerivedValues;
        }

        public static BasicValueContainer<IDerivedValue> GetDefaultForItem(int materialBonus, float weight)
        {
            BasicValueContainer<IDerivedValue> basicDerivedValues = new BasicValueContainer<IDerivedValue>();

            IValueFormula hitpointFormula = new ItemHitPointsFormula();

            IBasicValue bonus = new ConcreteBasicValue("bonus", materialBonus);
            IBasicValue weightValue = new ConcreteBasicValue("weight", (int)weight);

            int hitpoints = hitpointFormula.Calculate(new IBasicValue[] { bonus, weightValue });

            basicDerivedValues.Add(new EntityDerivedValue("hitpoints", hitpoints, hitpoints));

            return basicDerivedValues;
        }

        public string Name
        {
            get;
            protected set;
        }

        public int Maximum
        {
            get;
            protected set;
        }

        public int Value
        {
            get;
            protected set;
        }

        public int ModifyValue(int value)
        {
            Value = Math.Max(-Maximum, Math.Min(Maximum, Value + value));
            return Value;
        }

        public int ModifyMaximum(int value)
        {
            Maximum = Math.Max(1, Maximum + value);
            return Maximum;
        }

        public int SetValue(int value)
        {
            Value = Math.Max(-Maximum, value);
            return Value;
        }

        public int SetMaximum(int value)
        {
            Maximum = Math.Max(1, value);
            return Maximum;
        }
    }
}
