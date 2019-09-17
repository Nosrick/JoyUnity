using System;

namespace JoyLib.Code.Rollers
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

        /// <summary>
        /// Roll the successes of a dice pool.
        /// </summary>
        /// <param name="number">The number of d10s to roll.</param>
        /// <param name="threshold">The threshold at which success happens. Inclusive.</param>
        /// <returns>The number of successes.</returns>
        public static int RollSuccesses(int number, int threshold)
        {
            int successes = 0;
            for(int i = 0; i < number; i++)
            {
                if(Roll(1, 10) >= threshold)
                {
                    successes += 1;
                }
            }
            return successes;
        }

        public static void SetSeed(int seed)
        {
            s_Seed = seed;
            s_Roller = new Random(s_Seed);
        }
    }
}
