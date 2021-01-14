namespace JoyLib.Code.Unity.GUI
{
    public class DerivedValueContainer : NamedItem
    {
        public override int Value
        {
            get => this.m_Value;
            set
            {
                base.Value = value;
                this.m_ValueText.text = this.m_Value + "/" + this.m_Maximum;
            }
        }

        public override int Maximum
        {
            get => this.m_Maximum;
            set
            {
                base.Maximum = value;
                this.m_ValueText.text = this.m_Value + "/" + this.m_Maximum;
            }
        }
    }
}