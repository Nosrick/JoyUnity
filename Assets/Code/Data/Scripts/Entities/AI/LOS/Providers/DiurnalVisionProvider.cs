using JoyLib.Code.World;
using UnityEngine;
using System.Linq;

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
        {

        }

        public override void Update(Entity viewer, WorldInstance world)
        {
            Board = (FOVBasicBoard)Algorithm.Do(
                                    viewer,
                                    world,
                                    viewer.WorldPosition,
                                    GetVisionRect(viewer),
                                    GetVisibleWalls(viewer, world));

            Vision = Board.GetVision();
        }

        public override bool HasVisibility(Entity viewer, WorldInstance world, int x, int y, bool[,] vision)
        {
            return vision[x, y] && world.LightLevels[x, y] > MinimumLightLevel;
        }

        public override bool HasVisibility(Entity viewer, WorldInstance world, Vector2Int point, bool[,] vision)
        {
            return HasVisibility(viewer, world, point.x, point.y, vision);
        }
    }
}