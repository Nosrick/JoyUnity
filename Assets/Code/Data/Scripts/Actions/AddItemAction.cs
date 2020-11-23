using System;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Scripting.Actions
{
    public class AddItemAction : AbstractAction
    {
        public override string Name => "additemaction";

        public override string ActionString => "adding item";

        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            ClearLastParameters();
            
            if(!(participants[0] is IItemContainer container))
            {
                return false;
            }

            if(!(participants[1] is ItemInstance item))
            {
                return false;
            }

            bool newOwner = args.Length > 0 && (bool)args[0];

            if (newOwner && container is Entity owner)
            {
                if (tags is null == false 
                    && (tags.Any(tag => tag.Equals("theft", StringComparison.OrdinalIgnoreCase)) 
                        && owner.GUID != item.Owner)
                    || item.Owner == 0)
                {
                    item.SetOwner(owner.GUID);
                }
            }
            
            SetLastParameters(participants, tags, args);

            return container.AddContents(item);
        }
    }
}