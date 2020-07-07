using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS.Providers
{
    public class DiurnalVisionProvider : AbstractVisionProvider
    {
        public new string Name => "diurnalvision";

        public int MinimumLightLevel
        {
            get
            {
                return 5;
            }
        }

        public DiurnalVisionProvider() :
            base(new FOVShadowCasting())
        { }

        public override void Update(Entity viewer, WorldInstance world)
        {
            Board = (FOVBasicBoard)Algorithm.Do(
                                    viewer,
                                    world,
                                    viewer.WorldPosition,
                                    viewer.VisionProvider.GetVisionRect(viewer),
                                    viewer.VisionProvider.GetVisibleWalls(viewer, world));

            Vision = Board.Vision;
        }

        public override bool HasVisibility(Entity viewer, WorldInstance world, int x, int y)
        {
            return Vision[x, y] && world.LightLevels[x, y] > MinimumLightLevel;
        }

        public override bool HasVisibility(Entity viewer, WorldInstance world, Vector2Int point)
        {
            return HasVisibility(viewer, world, point.x, point.y);
        }
    }
}