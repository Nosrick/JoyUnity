using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib;

namespace JoyLib.Code.Scripting.Actions
{
    public class SeekAction : IJoyAction
    {
        public string Name => "seekaction";

        public string ActionString => "seeking";

        public bool Execute(JoyObject[] participants, string[] tags = null, params object[] args)
        {
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
                intent = Intent.Interact,
                searching = false,
                target = participants[1],
                targetPoint = GlobalConstants.NO_TARGET,
                need = needName
            };

            actor.CurrentTarget = needAIData;

            return true;
        }
    }
}