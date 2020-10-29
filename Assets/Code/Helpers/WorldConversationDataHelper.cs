using JoyLib.Code.World;
using System.Linq;
using Castle.Core.Internal;

namespace JoyLib.Code.Helpers
{
    public static class WorldConversationDataHelper
    {
        public static int GetNumberOfFloors(int floorsSoFar, WorldInstance worldToCheck)
        {
            if (worldToCheck.Areas.Count > 0)
            {
                foreach (WorldInstance world in worldToCheck.Areas.Values)
                {
                    return GetNumberOfFloors(floorsSoFar + 1, world);
                }
            }
                
            return floorsSoFar;
        }

        public static int GetNumberOfCreatures(string entityType, WorldInstance worldToCheck)
        {
            return entityType.IsNullOrEmpty() ? 
                worldToCheck.Entities.Count 
                : worldToCheck.Entities.Count(x => x.CreatureType.Equals(entityType));
        }
    }
}
