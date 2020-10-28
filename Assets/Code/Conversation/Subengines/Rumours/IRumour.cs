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

        bool Baseless
        {
            get;
        }

        bool FulfilsConditions(IEnumerable<Tuple<string, int>> values);
        bool FulfilsConditions(IEnumerable<JoyObject> participants);

        string ConstructString();

        IRumour Create(
            JoyObject[] participants,
            string[] tags,
            int viralPotential,
            ITopicCondition[] conditions,
            string[] parameters,
            string words,
            bool baseless = false);
    }
}
