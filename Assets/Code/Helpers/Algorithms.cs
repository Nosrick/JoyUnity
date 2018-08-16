using System.Collections.Generic;

namespace JoyLib.Code.Helpers
{
    public static class Algorithms
    {
        public static void Swap<T>(ref T left, ref T right)
        {
            T temp;
            temp = left;
            left = right;
            right = temp;
        }
    }
}
