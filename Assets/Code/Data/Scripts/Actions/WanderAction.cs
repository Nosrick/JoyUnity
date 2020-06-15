using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib;

namespace JoyLib.Code.Scripting.Actions
{
    public class WanderAction : IJoyAction
    {
        public string Name => "wanderaction";

        public string ActionString => "wandering";

        public bool Execute(JoyObject[] participants, string[] tags = null, params object[] args)
        {
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

            actor.CurrentTarget = needAIData;

            return true;
        }
    }
}