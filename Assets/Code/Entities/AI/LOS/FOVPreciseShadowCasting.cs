using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities.AI.LOS
{
    public class FOVPreciseShadowCasting : IFOVHandler
    {
        private FOVBasicBoard m_Board;

        public FOVBasicBoard Do(Vector2Int origin, Vector2Int dimensions, int visionMod, List<Vector2Int> walls)
        {
            m_Board = new FOVBasicBoard(dimensions.x, dimensions.y, walls);
            m_Board.Visible(origin.x, origin.y);

            return m_Board;
        }

        public IFOVBoard Do(Vector2Int origin, int visionMod)
        {
            m_Board.Visible(origin.x, origin.y);

            return m_Board;
        }

        private bool CheckVisibility(Vector2Int start, Vector2Int end, bool blocking)
        {
            if(start.x > end.x)
            {
                bool arc1 = CheckVisibility(start, new Vector2Int(start.y, start.y), blocking);
                bool arc2 = CheckVisibility(new Vector2Int(0, 1), end, blocking);
                return arc1 && arc2;
            }

            int firstIndex = 0;
            bool firstShadow = false;
            while(firstIndex < m_Board.Vision.Length)
            {
                //int difference = 
            }

            return false;
        }

        IFOVBoard IFOVHandler.Do(Vector2Int origin, Vector2Int dimensions, int visionMod, List<Vector2Int> walls)
        {
            throw new System.NotImplementedException();
        }
    }
}
