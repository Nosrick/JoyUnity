namespace JoyLib.Code.Conversation
{
    public class ConversationCondition
    {
        private string m_Condition;
        private int m_Value;

        public ConversationCondition(string conditionRef, int valueRef)
        {
            m_Condition = conditionRef;
            m_Value = valueRef;
        }

        public bool FulfillsCondition(int comparison)
        {
            if (m_Condition.Equals(">"))
            {
                if (comparison > m_Value)
                    return true;
            }
            else if (m_Condition.Equals("<"))
            {
                if (comparison < m_Value)
                    return true;
            }
            else
            {
                if (m_Value == comparison)
                {
                    return true;
                }
            }
            return false;
        }

        public int value
        {
            get
            {
                return m_Value;
            }
        }
    }
}
