using JoyLib.Code.Entities.AI.LOS.Providers;

namespace JoyLib.Code.Entities.AI.LOS
{
    public interface IFOVBoard
    {
        bool[,] GetVision();

        bool Contains(int x, int y);

        void ClearBoard();

        bool Visited(int x, int y);

        void Visit(int x, int y);

        void Visible(int x, int y);

        bool IsBlocked(int x, int y);

        bool IsObstacle(int x, int y);

        double Radius(int deltaX, int deltaY);
    }
}
