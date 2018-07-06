using JoyLib.Code.Entities;
using JoyLib.Code.World;
using System.Linq;

namespace JoyLib.Code.Helpers
{
    public static class WorldConversationDataHelper
    {
        public static int GetNumberOfFloors(int floorsSoFar, WorldInstance worldToCheck)
        {
            if (worldToCheck.Areas.Count > 0)
                foreach (WorldInstance world in worldToCheck.Areas.Values)
                    return GetNumberOfFloors(floorsSoFar + 1, world);

            return floorsSoFar;
        }

        public static int GetNumberOfCreatures(Entity entityRef, WorldInstance worldToCheck)
        {
            return worldToCheck.Entities.Count(x => x.CreatureType.Equals(entityRef.CreatureType));
        }
    }
}
