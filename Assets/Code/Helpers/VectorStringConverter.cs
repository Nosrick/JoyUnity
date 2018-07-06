using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class VectorStringConverter
    {
        public static Vector2Int StringToPoint(string pointString)
        {
            int indexOfX = pointString.IndexOf(':') + 1;
            int indexOfY = pointString.LastIndexOf(':') + 1;
            int xLength = 0;
            int yLength = 0;

            for(int i = indexOfX; i < pointString.Length; i++)
            {
                if (char.IsDigit(pointString[i]))
                    xLength += 1;
                else
                    break;
            }

            for (int i = indexOfY; i < pointString.Length; i++)
            {
                if (char.IsDigit(pointString[i]))
                    yLength += 1;
                else
                    break;
            }

            int x = int.Parse(pointString.Substring(indexOfX, xLength));
            int y = int.Parse(pointString.Substring(indexOfY, yLength));

            return new Vector2Int(x, y);
        }
    }
}
