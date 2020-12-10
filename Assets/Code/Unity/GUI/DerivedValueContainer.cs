namespace JoyLib.Code.Unity.GUI
{
    public class DerivedValueContainer : NamedItem
    {
        public override int Value
        {
            get => m_Value;
            set
            {
                base.Value = value;
                m_ValueText.text = m_Value + "/" + m_Maximum;
            }
        }

        public override int Maximum
        {
            get => m_Maximum;
            set
            {
                base.Maximum = value;
                m_ValueText.text = m_Value + "/" + m_Maximum;
            }
        }
    }
}