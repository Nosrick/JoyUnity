namespace JoyLib.Code.Entities.Statistics
{
    public class ConcreteDerivedIntValue : IDerivedValue<int>
    {
        public const string HITPOINTS = "hitpoints";
        public const string CONCENTRATION = "concentration";
        public const string COMPOSURE = "composure";
        public const string MANA = "mana";

        public const string ITEM_HITPOINTS = "item hitpoints";

        public string Name
        {
            get;
            set;
        }

        public int Maximum
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }
        
        public ConcreteDerivedIntValue()
        {}

        public ConcreteDerivedIntValue(string name, int value, int maximum)
        {
            this.Name = name;
            this.Value = value;
            this.Maximum = maximum;
        }

        public int SetValue(string data)
        {
            this.Value = int.Parse(data);
            return this.Value;
        }

        public int SetMaximum(string data)
        {
            this.Maximum = int.Parse(data);
            return this.Maximum;
        }
    }
}
