namespace JoyLib.Code.Conversation.Conversations
{
    public class EqualToCondition : ITopicCondition
    {
        public EqualToCondition(string criteria, int value)
        {
            this.Criteria = criteria;
            this.Value = value;
        }

        public int Value
        {
            get;
            protected set;
        }

        public string Criteria
        {
            get;
            protected set;
        }

        public bool FulfillsCondition(int value)
        {
            if(value == Value)
            {
                return true;
            }
            return false;
        }
    }
}
