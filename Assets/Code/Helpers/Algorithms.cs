namespace JoyLib.Code.Helpers
{
    public static class AlgorithmsElf
    {
        public static void Swap<T>(ref T left, ref T right)
        {
            T temp;
            temp = left;
            left = right;
            right = temp;
        }

        public static int Difference(int left, int right)
        {
            return left - right;
        }
    }
}
