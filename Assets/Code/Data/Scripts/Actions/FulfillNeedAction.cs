using System.Linq;
using JoyLib.Code.Entities;

namespace JoyLib.Code.Scripting.Actions
{
    public class FulfillNeedAction : AbstractAction
    {
        public override string Name => "fulfillneedaction";

        public override string ActionString => "fulfilling need";

        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.ClearLastParameters();
            
            if(!(participants[0] is Entity actor))
            {
                return false;
            }

            if(!(args[0] is string need))
            {
                return false;
            }

            if(!(args[1] is int value))
            {
                return false;
            }

            int counter = args[2] is null ? 0 : (int) args[2];

            bool doAll = args.Length < 4 ? false : (bool) args[3];

            IJoyObject[] fellowActors = participants.Where(p => p.Guid != actor.Guid).ToArray();
            
            actor.Needs[need].Fulfill(value);
            actor.FulfillmentData = new Entities.Needs.FulfillmentData(need, counter, fellowActors);

            if (doAll)
            {
                foreach (JoyObject jo in fellowActors)
                {
                    if (jo is Entity entity)
                    {
                        IJoyObject[] others = participants.Where(p => p.Guid != entity.Guid).ToArray();
                        entity.Needs[need].Fulfill(value);
                        actor.FulfillmentData = new Entities.Needs.FulfillmentData(need, counter, others);
                    }
                }
            }

            GlobalConstants.ActionLog.LogAction(actor, this.ActionString + " " + need);

            this.SetLastParameters(participants, tags, args);

            return true;
        }
    }
}