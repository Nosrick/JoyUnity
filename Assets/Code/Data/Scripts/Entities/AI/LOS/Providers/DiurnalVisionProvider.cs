using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public class DiurnalVisionProvider : AbstractVisionProvider
    {
        public new string Name => "diurnalvision";

        private static int MinimumLightLevel => 5;

        public DiurnalVisionProvider() :
            base(new FOVRecursiveShadowCasting())
        {

        }

        public override void Update(IEntity viewer, IWorldInstance world)
        {
            if (viewer is null)
            {
                Debug.Log("VIEWER IS NULL");
            }

            if (world is null)
            {
                Debug.Log("WORLD IS NULL");
            }
            
            Vision = new HashSet<Vector2Int>();
            
            Board = (FOVBasicBoard)Algorithm.Do(
                                    viewer,
                                    world,
                                    world.Dimensions,
                                    world.Walls.Keys);

            Vision = Board.GetVision();
        }

        public override bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y,
            IEnumerable<Vector2Int> vision)
        {
            //TODO: Fix this once lighting calculations are back in
            return true;/* && world.LightLevels[x, y] > MinimumLightLevel;*/
        }

        public override bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point,
            IEnumerable<Vector2Int> vision)
        {
            return HasVisibility(viewer, world, point.x, point.y, vision);
        }
    }
}