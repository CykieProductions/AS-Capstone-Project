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


using Capstone_Chronicles.GUI;
using static System.Console;

namespace Capstone_Chronicles
{
    internal static class Program
    {
        public static string Name { get; private set; } = "Capstone Chronicles";
        public static bool GameIsRunning { get; private set; } = true;
        public static Action? OnExit;

        static void Main(string[] args)
        {
            Console.Title = Name;
#if !DEBUG
            GameManager.ChangeScene(SceneFactory.TitleScreen);
#else
            GameManager.ChangeScene(SceneFactory.SorecordForest);
#endif

            //This means the game stopped without properly exiting
            if (GameIsRunning)
            {
                WriteLine("The program has ended unexpectedly...");
                ReadKey();
            }
        }

        public static void Exit()
        {
            OnExit?.Invoke();
            GameIsRunning = false;
        }

        [Obsolete("This will only work in debug mode. Use a Label for actual text!", false)]
        public static void print<T>(T message, bool singleLine = false)
        {
#if DEBUG
            if (GetCursorPosition().Left == 0)
                Write("DEBUG: ");
            Write(message?.ToString() + (singleLine ? null : '\n'));
            //Thread.Sleep(TimeSpan.FromSeconds(2));
            ReadKey(true);
#endif
        }
    }

    public static class Input
    {
        public static ConsoleKey PauseKey { get; private set; } = ConsoleKey.Escape;

        public static bool GetKeyDown(ConsoleKey key)
        {
            bool result = false;

            if (KeyAvailable && ReadKey(true).Key == key)
                result = true;

            return result;
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
        public static T Clamp<T>(this T value, T min, T max) where T : struct
        {
            if (value is not int && value is not float && value is not double)
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