using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using UnityEngine;

namespace JoyLib.Code.Scripting.Actions
{
    public class WanderAction : AbstractAction
    {
        public override string Name => "wanderaction";

        public override string ActionString => "wandering";

        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.ClearLastParameters();
            
            if(!(participants[0] is Entity actor))
            {
                return false;
            }

            NeedAIData needAIData = new NeedAIData
            {
                idle = false,
                intent = Intent.Interact,
                searching = true,
                targetPoint = GlobalConstants.NO_TARGET
            };
            Debug.Log(actor.JoyName + " is wandering.");
            foreach (string tag in tags)
            {
                Debug.Log(tag);
            }

            actor.CurrentTarget = needAIData;

            this.SetLastParameters(participants, tags, args);

            return true;
        }
    }
}