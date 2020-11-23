using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib;
using UnityEngine;

namespace JoyLib.Code.Scripting.Actions
{
    public class SeekAction : AbstractAction
    {
        public override string Name => "seekaction";

        public override string ActionString => "seeking";

        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            ClearLastParameters();
            
            if(!(participants[0] is Entity actor))
            {
                return false;
            }

            if(!(args[0] is string needName))
            {
                return false;
            }

            NeedAIData needAIData = new NeedAIData
            {
                idle = false,
                intent = Intent.Interact,
                searching = false,
                target = participants[1],
                targetPoint = GlobalConstants.NO_TARGET,
                need = needName
            };

            actor.CurrentTarget = needAIData;
            Debug.Log(actor.JoyName + " is seeking " + participants[1].JoyName + " for " + needName);
            
            SetLastParameters(participants, tags, args);

            return true;
        }
    }
}