using JoyLib.Code.Entities;

namespace JoyLib.Code.Conversation.Conversations
{
    public abstract class BaseConversation
    {
        public abstract string Interact(Entity instigator, Entity listener);
    }
}
