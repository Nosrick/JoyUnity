using System;
using System.Security.Cryptography;
using JoyLib.Code.Helpers;
using UnityEngine;

namespace JoyLib.Code.Rollers
{
    public class RNG
    {
        protected int m_Seed;
        protected RNGCryptoServiceProvider m_Roller;

        public RNG()
        {
            m_Roller = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Returns a random number between the two specified numbers.
        /// </summary>
        /// <param name="lower">Inclusive.</param>
        /// <param name="upper">Exclusive.</param>
        /// <returns></returns>
        public int Roll(int lower, int upper)
        {
            byte[] bytes = new byte[4];
            m_Roller.GetBytes(bytes);
            int result = BitConverter.ToInt32(bytes, 0) % upper;
            result = Math.Abs(result);
            return result;
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
    }
}
