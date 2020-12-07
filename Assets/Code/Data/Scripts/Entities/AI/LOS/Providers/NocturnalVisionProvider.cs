using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public class NocturnalVisionProvider : AbstractVisionProvider
    {
        public new string Name => "nocturnalvision";

        public NocturnalVisionProvider() :
            base(new FOVShadowCasting())
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

        public override bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y)
        {
            return HasVisibility(viewer, world, new Vector2Int(x, y));
        }

        public override bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point)
        {
            return Vision.Contains(point);
        }
    }
}