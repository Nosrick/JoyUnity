namespace JoyLib.Code.Entities.Statistics
{
    public class ConcreteDerivedFloatValue : IDerivedValue<float>
    {
        public string Name { get; set; }
        public float Value { get; set; }
        public float Maximum { get; set; }

        public ConcreteDerivedFloatValue()
        {
            
        }

        public ConcreteDerivedFloatValue(string name, float value, float maximum)
        {
            this.Name = name;
            this.Value = value;
            this.Maximum = maximum;
        }
        
        public float SetValue(string data)
        {
            this.Value = float.Parse(data);
            return this.Value;
        }

        public float SetMaximum(string data)
        {
            this.Maximum = float.Parse(data);
            return this.Maximum;
        }
    }
}