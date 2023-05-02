using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles.GUI
{
    /// <summary>
    /// Defines the Foreground (text) and Background color for displayed text
    /// </summary>
    public struct ColorSet
    {
        /// <summary>
        /// The default text color
        /// </summary>
        public const ConsoleColor DEFAULT_TEXT_COLOR = ConsoleColor.White;
        /// <summary>
        /// The default background color
        /// </summary>
        public const ConsoleColor DEFAULT_BACK_COLOR = ConsoleColor.Black;
        /// <summary>
        /// Gets a ColorSet using the default colors
        /// </summary>
        public static ColorSet Normal { get => new ColorSet(DEFAULT_TEXT_COLOR, DEFAULT_BACK_COLOR); }
        /// <summary>
        /// Gets a ColorSet using the swapped default colors
        /// </summary>
        public static ColorSet Inverse { get => new ColorSet(DEFAULT_BACK_COLOR, DEFAULT_TEXT_COLOR); }

        /// <summary>
        /// The text color
        /// </summary>
        public ConsoleColor foreground = DEFAULT_TEXT_COLOR;
        /// <summary>
        /// The background color
        /// </summary>
        public ConsoleColor background = DEFAULT_BACK_COLOR;

        /// <summary>
        /// Constructs a new <see cref="ColorSet"/>
        /// </summary>
        /// <param name="inForeColor">The text color</param>
        /// <param name="inBackColor">The background color</param>
        public ColorSet(ConsoleColor inForeColor = DEFAULT_TEXT_COLOR, ConsoleColor inBackColor = DEFAULT_BACK_COLOR)
        {
            foreground = inForeColor;
            background = inBackColor;
        }

        /// <summary>
        /// Converts a tuple to a ColorSet
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ColorSet((ConsoleColor, ConsoleColor) value)
        {
            return new ColorSet(value.Item1, value.Item2);
        }
    }
}
