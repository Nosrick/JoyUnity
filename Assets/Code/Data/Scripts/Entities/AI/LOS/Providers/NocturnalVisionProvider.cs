using System.Collections.Generic;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public class NocturnalVisionProvider : AbstractVisionProvider
    {
        public new string Name => "nocturnalvision";

        public override int MaximumLightLevel => 24;

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

            this.Vision = new HashSet<Vector2Int>();

            this.Board = (FOVBasicBoard) this.Algorithm.Do(
                                    viewer,
                                    world,
                                    world.Dimensions,
                                    world.Walls.Keys);

            this.Vision = this.Board.GetVision();
        }

        public override bool HasVisibility(IEntity viewer, IWorldInstance world, int x, int y)
        {
            return this.HasVisibility(viewer, world, new Vector2Int(x, y));
        }

        public override bool HasVisibility(IEntity viewer, IWorldInstance world, Vector2Int point)
        {
            return this.Vision.Contains(point) && world.LightCalculator.Light.GetLight(point) < this.MaximumLightLevel;
        }
    }
}