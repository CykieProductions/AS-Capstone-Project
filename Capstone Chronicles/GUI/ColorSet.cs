using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles.GUI
{
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
}
