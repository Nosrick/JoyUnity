using JoyLib.Code.Entities;
using System;
using System.Collections.Generic;
using JoyLib.Code.Scripting;

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

        IJoyAction[] CachedActions
        {
            get;
        }

        Speaker Speaker
        {
            get;
        }

        string Link
        {
            get;
        }

        string[] GetConditionTags();

        bool FulfilsConditions(IEnumerable<Tuple<string, int>> values);
        bool FulfilsConditions(IEnumerable<JoyObject> participants);

        ITopic[] Interact(Entity instigator, Entity listener);

        void Initialise(
            ITopicCondition[] conditions,
            string ID,
            string[] nextTopics,
            string words,
            int priority,
            string[] cachedActions,
            Speaker speaker,
            string link = "");
        
        void Initialise(
            ITopicCondition[] conditions,
            string ID,
            string[] nextTopics,
            string words,
            int priority,
            IJoyAction[] actions,
            Speaker speaker,
            string link);
    }

    public class TopicComparer : IComparer<ITopic>
    {
        public int Compare(ITopic x, ITopic y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }

    public enum Speaker
    {
        LISTENER,
        INSTIGATOR
    }
}
