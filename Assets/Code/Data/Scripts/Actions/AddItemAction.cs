using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Scripting.Actions
{
    public class AddItemAction : IJoyAction
    {
        public string Name => "additemaction";

        public string ActionString => "adding item";

        public bool Execute(JoyObject[] participants, string[] tags = null, params object[] args)
        {
            if(!(participants[0] is IItemContainer container))
            {
                return false;
            }

            if(!(participants[1] is ItemInstance item))
            {
                return false;
            }

            return container.AddContents(item);
        }
    }
}