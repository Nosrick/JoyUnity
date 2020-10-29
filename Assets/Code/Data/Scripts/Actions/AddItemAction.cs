using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Scripting.Actions
{
    public class AddItemAction : IJoyAction
    {
        public string Name => "additemaction";

        public string ActionString => "adding item";
        public IJoyObject[] LastParticipants { get; protected set; }
        public string[] LastTags { get; protected set; }
        public object[] LastArgs { get; protected set; }

        public bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            if(!(participants[0] is IItemContainer container))
            {
                return false;
            }

            if(!(participants[1] is ItemInstance item))
            {
                return false;
            }
            
            SetLastParameters(participants, tags, args);

            return container.AddContents(item);
        }
        
        public void SetLastParameters(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.LastParticipants = participants;
            this.LastTags = tags;
            this.LastArgs = args;
        }
    }
}