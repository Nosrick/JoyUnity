using JoyLib.Code.Entities;
using JoyLib.Code.World;

namespace JoyLib.Code.Scripting.Actions
{
    public class EnterWorldAction : AbstractAction
    {
        public override string Name => "enterworldaction";

        public override string ActionString => "enters";

        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.ClearLastParameters();
            
            if (participants.Length != 1 || args.Length != 1)
            {
                return false;
            }

            if (!(participants[0] is Entity actor))
            {
                return false;
            }

            if (!(args[0] is WorldInstance worldInstance))
            {
                return false;
            }
            
            actor.MyWorld.RemoveEntity(actor.WorldPosition);
            worldInstance.AddEntity(actor);

            if (!actor.HasDataKey(worldInstance.Name))
            {
                actor.AddData(worldInstance.Name, "explored");
            }

            this.SetLastParameters(participants, tags, args);

            return true;
        }
    }
}