namespace JoyLib.Code.Conversation
{
    public struct TopicData
    {
        public string value;
        public int ID;
        public int nextTopic;
        public bool closesWindow;

        public TopicData(string valueRef, int IDRef, int nextTopicRef, bool closesWindowRef = false)
        {
            value = valueRef;
            ID = IDRef;
            nextTopic = nextTopicRef;
            closesWindow = closesWindowRef;
        }
    }
}
