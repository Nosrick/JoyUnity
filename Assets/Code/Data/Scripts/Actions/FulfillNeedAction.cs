using JoyLib.Code.Entities;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.Scripting.Actions
{
    public class FulfillNeedAction : IJoyAction
    {
        public string Name => "fulfillneedaction";

        public string ActionString => "fulfilling need";

        public bool Execute(JoyObject[] participants, string[] tags = null, params object[] args)
        {
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

            JoyObject[] fellowActors = participants.Where(p => p.GUID != actor.GUID).ToArray();
            
            actor.Needs[need].Fulfill(value);
            actor.FulfillmentData = new Entities.Needs.FulfillmentData(need, counter, fellowActors);

            if (doAll)
            {
                foreach (JoyObject jo in fellowActors)
                {
                    if (jo is Entity entity)
                    {
                        JoyObject[] others = participants.Where(p => p.GUID != entity.GUID).ToArray();
                        entity.Needs[need].Fulfill(value);
                        actor.FulfillmentData = new Entities.Needs.FulfillmentData(need, counter, others);
                    }
                }
            }

            ActionLog.instance.LogAction(actor, ActionString + need);

            return true;
        }
    }
}