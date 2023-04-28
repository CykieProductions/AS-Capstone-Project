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
https://github.com/CykieProductions/AS-Capstone-Project

Course Page: 
https://hcc.instructure.com/courses/121338

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
        public static event Action? OnExit;

        static void Main(string[] args)
        {
            Console.Title = Name;
#if !DEBUG
            GameManager.ChangeScene(SceneFactory.TitleScreen);
#else
            GameManager.ChangeScene(SceneFactory.Overworld);
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
#if DEBUG//fix
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

        /// <summary>
        /// Adds a new element to a list if it wasn't already present
        /// </summary>
        /// <returns>True if successfully added, otherwise False</returns>
        public static bool AddIfUnique<T>(this IList<T> sequence, T obj)
        {
            if (!sequence.Contains(obj))
            {
                sequence.Add(obj);
                return true;
            }

            return false;
        }
        public static T GetRandomElement<T>(this IEnumerable<T> sequence)
        {
            return sequence.ElementAt(RNG.Rand.Next(0, sequence.Count()));
        }
        public static void RemoveRandomElement<T>(this ICollection<T> sequence)
        {
            sequence.Remove(sequence.ElementAt(RNG.Rand.Next(0, sequence.Count)));
        }

        
    }
}