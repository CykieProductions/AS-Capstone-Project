/* Project Information
----------------------------------------------------------------------------------------------------------------------------
Working Title: 
----
Capstone Chronicles
--------------------------------------------------------------

--------------------------------------------------------------
Description:
----
This is my 2023 Capstone Project for A.S. Computer Programming and Analysis
It is a feature complete, turn-based, role-playing game (RPG)

School:     Hillsborough Community College
Course:     COP 2939-73927 Computer Programming Capstone
Instructor: Dr. Banisakher
Start Date: 01/18/2023
--------------------------------------------------------------

--------------------------------------------------------------
Links:
----
Github:

Course Page: 
https://hcc.instructure.com/courses/121338

Bitmaps: 
https://social.msdn.microsoft.com/Forums/en-US/6b7a7beb-0e18-484f-8e9c-5fad3482485d/uwp-bitmap-class-access-raw-pixel-data?forum=wpdevelop#:~:text=In%20C%23%2C%20use%20AsStream%20extension%20method%20to%20access,await%20Stream.CopyToAsync%28memoryStream%29%3B%20byte%5B%5D%20pixels%20%3D%20memoryStream.ToArray%28%29%3B%20%7D%20%7D
Audio:
https://stackoverflow.com/questions/34116886/how-to-play-background-music-in-a-c-sharp-console-application#:~:text=How%20to%20play%20background%20music%20in%20a%20C%23,Action%20to%20Content.%20...%203%20Use%20this%20code
Menus in a Console App:
https://www.youtube.com/watch?v=qAWhGEPMlS8
----------------------------------------------------------------------------------------------------------------------------
 */

using System.Numerics;
using static System.Console;

namespace Capstone_Chronicles
{
    internal static class Program
    {
        public static bool GameIsRunning { get; private set; } = true;

        static void Main(string[] args)
        {
            GameManager.ChangeScene(GameManager.SceneFactory.TitleScreen);

            //This means the game stopped without properly exiting
            if (GameIsRunning)
            {
                WriteLine("The program has ended unexpectedly...");
                ReadKey();
            }
        }

        public static void Exit()
        {
            GameIsRunning = false;
        }

        [Obsolete("This will only work in debug mode. Use a Label for actual text!", false)]
        public static void print<T>(T message, bool singleLine = false)
        {
#if DEBUG
            Write("DEBUG: " + message + (singleLine ? null : '\n'));
#endif
        }
    }

    public static class RNG
    {
        public static Random Rand { get; private set; } = new Random(DateTime.Now.Millisecond);

        // Presets
        public static bool OneInTwo { get => Chance(0.5f); }
        public static bool OneInFour { get => Chance(0.25f); }
        public static bool ThreeInFour { get => Chance(0.75f); }
        public static bool OneInThree { get => Chance(1 / 3); }
        public static bool TwoInThree { get => Chance(2 / 3); }

        /*x// <summary>
        /// Runs a check based on percent chance out of 100
        /// </summary>
        /// <param name="percentage">Between 0 and 100 (inclusive)</param>
        public static bool Chance(int percentage)
        {
            //Ensures 0 will always fail and 100 will always succeed
            var rVal = Rand.Next(1, 101);

            if (rVal <= percentage)//Success condition
                return true;

            return false;
        }*/

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
        /// Returns a random integer between to inclusive values
        /// </summary>
        public static int RandomInt(int inclusiveMin, int inclusiveMax)
        {
            if (inclusiveMin <= inclusiveMax)
                throw new ArgumentOutOfRangeException("Arguments must create a valid range!");

            return Rand.Next(inclusiveMin, inclusiveMax + 1);
        }
    }


    /// <summary>
    /// A class to contain all extension methods for this project
    /// </summary>
    public static class Extentions
    {
        //! String Extensions
        /// <summary>
        /// Repeats the target string a number of times and returns it
        /// </summary>
        /// <param name="count">The number of times to copy the string</param>
        public static string Repeat(this string value, ushort count)
        {
            string result = value;

            for (int i = 0; i < count; i++)
            {
                result += value;
            }

            return result;
        }
        

        //! Char Extensions
        /// <summary>
        /// Copies the target char a number of times and returns it as a string
        /// </summary>
        /// <param name="count">The number of chars to return</param>
        public static string Repeat(this char value, ushort count = 2)
        {
            return new string(value, count);
        }

        //! Number Extensions
        /// <summary>
        /// Clamps any valid *number* between the min and max *number*
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min">Inclusive minimum</param>
        /// <param name="max">Inclusive maximum</param>
        /// <returns></returns>
        public static T Clamp<T>(this T value, T min, T max)
        {
            if (value is not int && value is not float)
                throw new ArgumentException("Generic Type <T> must be a number!");
            
            dynamic v = value;

            if (v > max)
                v = max;
            else if (v < min)
                v = min;

            return v;
        }

        //https://stackoverflow.com/questions/56692/random-weighted-choice
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)new Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default(T);

        }
    }
}