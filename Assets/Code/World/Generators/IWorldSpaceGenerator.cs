using System.Collections.Generic;

namespace JoyLib.Code.World.Generators
{
    public interface IWorldSpaceGenerator
    {
        WorldTile[,] GenerateWorldSpace(int sizeRef);
        void GenerateTileObjects(WorldTile[,] worldTiles);
        List<JoyObject> GenerateWalls(WorldTile[,] worldTiles);
    }
}
