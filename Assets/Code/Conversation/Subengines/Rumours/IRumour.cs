using System;
using System.Collections.Generic;
using JoyLib.Code.Conversation.Conversations;

namespace JoyLib.Code.Conversation.Subengines.Rumours
{
    public interface IRumour
    {
        JoyObject[] Participants
        {
            get;
        }

        string[] Tags
        {
            get;
        }

        int ViralPotential
        {
            get;
        }

        ITopicCondition[] Conditions
        {
            get;
        }

        string[] Parameters
        {
            get;
        }

        string Words
        {
            get;
        }

        bool FulfilsConditions(IEnumerable<Tuple<string, int>> values);
        bool FulfilsConditions(IEnumerable<JoyObject> participants);

        string ConstructString();
    }
}
