using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles.GUI
{
    internal class Button : Label
    {
        public ColorSet selectColors;
        string prefix;
        readonly Action<Menu> OnPress;

        public Button(string inText, Action<Menu> inBehavior, string inPrefix = ">>", ColorSet inColorSet = default, ColorSet? inAltColorSet = null) : base(inText, inColorSet)
        {
            if (inColorSet.Equals(default(ColorSet)))
                inColorSet = ColorSet.Normal;
            //Inverse colors
            if (inAltColorSet == null)
                inAltColorSet = ColorSet.Inverse;

            prefix = inPrefix;
            OnPress = inBehavior;
            selectColors = inAltColorSet.Value;
        }

        /// <summary>
        /// Displays text with the specified colors and a prefix. Takes a bool.
        /// </summary>
        /// <param name="args">[0] is a bool for determining if this is selected or not</param>
        public override void Display(params dynamic[] args)
        {
            if (args[0] is bool)
            {
                if (args[0] == true)
                {
                    Write(prefix);
                    base.Display(selectColors.foreground, selectColors.background);
                }
                else
                    base.Display();
            }
            else
                throw new ArgumentException("This function takes a bool value");
        }

        internal void Press(Menu menu)
        {
            OnPress.Invoke(menu);
        }

    }
}
