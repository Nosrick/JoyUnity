using System.Collections.Generic;
using UnityEngine;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVPreciseShadowCasting : AbstractFOVHandler
    {
        private FOVBasicBoard m_Board;

        public override IFOVBoard Do(Entity viewer, WorldInstance world, Vector2Int origin, RectInt dimensions, Vector2Int[] walls)
        {
            m_Board = new FOVBasicBoard(dimensions.width, dimensions.height, walls);
            m_Board.Visible(origin.x, origin.y);

            return m_Board;
        }

        public override LinkedList<Vector2Int> HasLOS(Vector2Int origin, Vector2Int target)
        {
            throw new System.NotImplementedException();
        }

        private bool CheckVisibility(Vector2Int start, Vector2Int end, bool blocking)
        {
            if(start.x > end.x)
            {
                bool arc1 = CheckVisibility(start, new Vector2Int(start.y, start.y), blocking);
                bool arc2 = CheckVisibility(new Vector2Int(0, 1), end, blocking);
                return arc1 && arc2;
            }

            return false;
        }
    }
}
