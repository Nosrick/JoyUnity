namespace JoyLib.Code.Conversation.Conversations
{
    public class LessThanCondition : ITopicCondition
    {
        public LessThanCondition(string criteria, int condition)
        {
            this.Criteria = criteria;
            this.Value = condition;
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
            if(value < Value)
            {
                return true;
            }
            return false;
        }
    }
}
