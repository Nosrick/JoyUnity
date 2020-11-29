using System;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.Rollers
{
    public class RNG
    {
        protected int m_Seed;
        protected Random m_Roller;

        public RNG()
        {
            m_Seed = AlgorithmsElf.ChopLong(DateTime.UtcNow.ToFileTimeUtc());
            m_Roller = new Random();
        }

        public RNG(int seed)
        {
            m_Seed = seed;
            m_Roller = new Random(m_Seed);
        }

        /// <summary>
        /// Returns a random number between the two specified numbers.
        /// </summary>
        /// <param name="lower">Inclusive.</param>
        /// <param name="upper">Exclusive.</param>
        /// <returns></returns>
        public int Roll(int lower, int upper)
        {
            return m_Roller.Next(lower, upper);
        }

        /// <summary>
        /// Roll the successes of a dice pool.
        /// </summary>
        /// <param name="number">The number of d10s to roll.</param>
        /// <param name="threshold">The threshold at which success happens. Inclusive.</param>
        /// <returns>The number of successes.</returns>
        public int RollSuccesses(int number, int threshold)
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

        public void SetSeed(int seed)
        {
            m_Seed = seed;
            m_Roller = new Random(m_Seed);
        }
    }
}
