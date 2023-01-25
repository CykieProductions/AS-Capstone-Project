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
    }

    public struct ColorSet
    {
        public const ConsoleColor DEFAULT_TEXT_COLOR = ConsoleColor.White;
        public const ConsoleColor DEFAULT_BACK_COLOR = ConsoleColor.Black;
        public static ColorSet Normal { get => new ColorSet(DEFAULT_TEXT_COLOR, DEFAULT_BACK_COLOR); }
        public static ColorSet Inverse { get => new ColorSet(DEFAULT_BACK_COLOR, DEFAULT_TEXT_COLOR); }


        public ConsoleColor foreground = DEFAULT_TEXT_COLOR;
        public ConsoleColor background = DEFAULT_BACK_COLOR;

        public ColorSet(ConsoleColor inForeColor = DEFAULT_TEXT_COLOR, ConsoleColor inBackColor = DEFAULT_BACK_COLOR)
        {
            foreground = inForeColor;
            background = inBackColor;
        }

        public static implicit operator ColorSet((ConsoleColor, ConsoleColor) value)
        {
            return new ColorSet(value.Item1, value.Item2);
        }
    }

    /// <summary>
    /// A class to contain all extention methods for this project
    /// </summary>
    public static class Extentions
    {
        //! String Extentions
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
        

        //! Char Extentions
        /// <summary>
        /// Copies the target char a number of times and returns it as a string
        /// </summary>
        /// <param name="count">The number of chars to return</param>
        public static string Repeat(this char value, ushort count = 2)
        {
            return new string(value, count);
        }
        
    }
}