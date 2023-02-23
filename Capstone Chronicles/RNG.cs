using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    public static class RNG
    {
        public static Random Rand { get; private set; } = new Random(DateTime.Now.Millisecond);

        // Presets
        public static bool OneInTwo { get => Chance(0.5f); }
        public static bool OneInFour { get => Chance(0.25f); }
        public static bool ThreeInFour { get => Chance(0.75f); }
        public static bool OneInThree { get => Chance(1f / 3f); }
        public static bool TwoInThree { get => Chance(2f / 3f); }

        /// <summary>
        /// Runs a check based on percentage
        /// </summary>
        /// <param name="percentage">Between 0 and 1 (inclusive)</param>
        public static bool Chance(float percentage)
        {
            //Ensures 0 will always fail and 100 will always succeed
            var rVal = Rand.NextSingle() + float.Epsilon;

            if (rVal <= percentage)//Success condition
                return true;

            return false;
        }

        /// <summary>
        /// Returns a random integer between two inclusive values
        /// </summary>
        public static int RandomInt(int inclusiveMin, int inclusiveMax)
        {
            if (inclusiveMin > inclusiveMax)
                throw new ArgumentOutOfRangeException("Arguments must create a valid range!");
            else if (inclusiveMin == inclusiveMax)
                return inclusiveMax;

            return Rand.Next(inclusiveMin, inclusiveMax + 1);
        }
    }
}
