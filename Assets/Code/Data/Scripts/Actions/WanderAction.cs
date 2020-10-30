using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI;
using JoyLib;

namespace JoyLib.Code.Scripting.Actions
{
    public class WanderAction : IJoyAction
    {
        public string Name => "wanderaction";

        public string ActionString => "wandering";
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

            NeedAIData needAIData = new NeedAIData
            {
                idle = false,
                intent = Intent.Interact,
                searching = true,
                targetPoint = GlobalConstants.NO_TARGET
            };

            actor.CurrentTarget = needAIData;

            SetLastParameters(participants, tags, args);

            return true;
        }
        
        public void SetLastParameters(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            LastParticipants = participants;
            LastTags = tags;
            LastArgs = args;
            Successful = true;
        }
    }
}