using JoyLib.Code.Entities;
using System.Collections.Generic;
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

            if(!(args[2] is int counter))
            {
                return false;
            }

            List<JoyObject> fellowActors = new List<JoyObject>(participants.Length - 1);

            for(int i = 1; i < participants.Length; i++)
            {
                fellowActors.Add(participants[i]);
            }

            actor.Needs[need].Fulfill(value);
            actor.FulfillmentData = new Entities.Needs.FulfillmentData(need, counter, fellowActors.ToArray());

            ActionLog.instance.LogAction(actor, ActionString + need);

            return true;
        }
    }
}