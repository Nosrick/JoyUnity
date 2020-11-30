using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Scripting.Actions
{
    public class PlaceItemInWorldAction : AbstractAction
    {
        public override string Name => "placeiteminworldaction";
        public override string ActionString => "placing item in world";

        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            ClearLastParameters();
            
            if (!(participants[0] is IEntity entity))
            {
                return false;
            }

            if (!(participants[1] is IItemInstance item))
            {
                return false;
            }
            
            entity.RemoveContents(item);
            item.Move(entity.WorldPosition);
            entity.MyWorld.AddObject(item);
            
            SetLastParameters(participants, tags, args);

            return true;
        }
    }
}