using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Scripting.Actions
{
    public class TradeAction : AbstractAction
    {
        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            if (participants.Length != 2)
            {
                return false;
            }

            if (args.Length == 0)
            {
                return false;
            }

            IEntity left, right;
            if (!(participants[0] is IEntity)
                && !(participants[1] is IEntity))
            {
                return false;
            }
            
            left = participants[0] as IEntity;
            right = participants[1] as IEntity;

            if (!(args[0] is IEnumerable<IItemInstance> leftOffering))
            {
                return false;
            }

            IEnumerable<IItemInstance> rightOffering =
                args.Length == 2 
                    ? args[1] as IEnumerable<IItemInstance> 
                    : new List<IItemInstance>();

            left.AddContents(rightOffering);
            left.RemoveContents(leftOffering);
            right.AddContents(leftOffering);
            right.RemoveContents(rightOffering);

            HashSet<string> myTags = tags is null ? new HashSet<string>() : new HashSet<string>(tags);
            if (myTags.Any(tag => tag.Equals("trade", StringComparison.OrdinalIgnoreCase)) == false)
            {
                myTags.Add("trade");
            }

            if (myTags.Any(tag => tag.Equals("give", StringComparison.OrdinalIgnoreCase)) == false)
            {
                myTags.Add("give");
            }
            
            if (myTags.Any(tag => tag.Equals("item", StringComparison.OrdinalIgnoreCase)) == false)
            {
                myTags.Add("item");
            }
            
            this.SetLastParameters(participants, tags, args);
            return true;
        }
    }
}