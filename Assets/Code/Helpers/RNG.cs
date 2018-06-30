using System;

namespace JoyLib.Code.Helpers
{
    public static class RNG
    {
        private static int s_Seed = 0;
        private static Random s_Roller;

        /// <summary>
        /// Returns a random number between the two specified numbers. Inclusive.
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static int Roll(int lower, int upper)
        {
            return s_Roller.Next(lower, upper + 1);
        }

        public static void SetSeed(int seed)
        {
            s_Seed = seed;
            s_Roller = new Random(s_Seed);
        }
    }
}
