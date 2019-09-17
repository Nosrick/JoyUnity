namespace JoyLib.Code.Conversation.Conversations
{
    public class GreaterThanCondition : ITopicCondition
    {
        public GreaterThanCondition(string criteria, int value)
        {
            Criteria = criteria;
            Value = value;
        }

        public string Criteria
        {
            get;
            protected set;
        }

        public int Value
        {
            get;
            protected set;
        }

        public bool FulfillsCondition(int value)
        {
            if(value > Value)
            {
                return true;
            }
            return false;
        }
    }
}
