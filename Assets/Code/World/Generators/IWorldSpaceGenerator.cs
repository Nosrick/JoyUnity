using JoyLib.Code.World.Generators.Interiors;
namespace JoyLib.Code.World.Generators
{
    public interface IWorldSpaceGenerator
    {
        WorldTile[,] GenerateWorldSpace(int sizeRef);
    }
}
