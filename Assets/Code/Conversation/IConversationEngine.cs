using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Conversation
{
    public interface IConversationEngine
    {
        void SetActors(IEntity instigator, IEntity listener);
        ITopic[] Converse(string topic, int index = 0);
        ITopic[] Converse(int index = 0);
        
        ITopic[] CurrentTopics { get; }
        ITopic[] AllTopics { get; }
        IEntity Instigator { get; }
        IEntity Listener { get; }
        
        GameObject Window { get; set; }
    }
}