using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities;
using UnityEngine;

namespace JoyLib.Code.Conversation
{
    public interface IConversationEngine
    {
        void SetActors(Entity instigator, Entity listener);
        ITopic[] Converse(string topic, int index = 0);
        ITopic[] Converse(int index = 0);
        
        ITopic[] CurrentTopics { get; }
        ITopic[] AllTopics { get; }
        Entity Instigator { get; }
        Entity Listener { get; }
        
        GameObject Window { get; set; }
    }
}