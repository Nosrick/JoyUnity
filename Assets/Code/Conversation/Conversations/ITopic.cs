using JoyLib.Code.Entities;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Conversation.Conversations
{
    public interface ITopic
    {
        ITopicCondition[] Conditions
        {
            get;
        }

        string ID
        {
            get;
        }

        string[] NextTopics
        {
            get;
        }

        string Words
        {
            get;
        }

        int Priority
        {
            get;
        }

        string[] GetConditionTags();

        bool PassesConditions(Tuple<string, int>[] values);

        ITopic[] Interact(Entity instigator, Entity listener);
    }

    public class TopicComparer : IComparer<ITopic>
    {
        public int Compare(ITopic x, ITopic y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
}
