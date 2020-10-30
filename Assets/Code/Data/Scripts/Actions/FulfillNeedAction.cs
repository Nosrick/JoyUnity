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
        public IJoyObject[] LastParticipants { get; protected set; }
        public string[] LastTags { get; protected set; }
        public object[] LastArgs { get; protected set; }
        public bool Successful { get; protected set; }

        public bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            Successful = false;
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

            IJoyObject[] fellowActors = participants.Where(p => p.GUID != actor.GUID).ToArray();
            
            actor.Needs[need].Fulfill(value);
            actor.FulfillmentData = new Entities.Needs.FulfillmentData(need, counter, fellowActors);

            if (doAll)
            {
                foreach (JoyObject jo in fellowActors)
                {
                    if (jo is Entity entity)
                    {
                        IJoyObject[] others = participants.Where(p => p.GUID != entity.GUID).ToArray();
                        entity.Needs[need].Fulfill(value);
                        actor.FulfillmentData = new Entities.Needs.FulfillmentData(need, counter, others);
                    }
                }
            }

            ActionLog.instance.LogAction(actor, ActionString + need);
            
            SetLastParameters(participants, tags, args);

            return true;
        }
        
        public void SetLastParameters(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.LastParticipants = participants;
            this.LastTags = tags;
            this.LastArgs = args;
            Successful = true;
        }
    }
}