using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Scripting.Actions
{
    public class GiveItemAction : AbstractAction
    {
        public override string Name => "giveitemaction";
        public override string ActionString => "gives";
        
        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.ClearLastParameters();
            
            if (participants.Length != 2)
            {
                return false;
            }

            if (!(participants[0] is Entity left))
            {
                return false;
            }
            if (!(participants[1] is Entity right))
            {
                return false;
            }

            if (!(args[0] is ItemInstance item))
            {
                return false;
            }

            if (!left.RemoveContents(item))
            {
                return false;
            }

            if (!right.AddContents(item))
            {
                return false;
            }

            item.SetOwner(right.GUID);

            this.SetLastParameters(participants, tags, args);

            return true;
        }
    }
}