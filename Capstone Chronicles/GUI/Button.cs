using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles.GUI
{
    /// <summary>
    /// A type of <see cref="Label"/> that can be interacted with as a part of a <see cref="Menu"/>
    /// </summary>
    public class Button : Label
    {
        /// <summary>
        /// Colors for the text when this Button is selected
        /// </summary>
        public ColorSet selectColors;
        string selectedPrefix;
        string normalPrefix;
        readonly Action<Menu>? OnPress;

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="inText">The text to display</param>
        /// <param name="inBehavior">The behavior trigger when pressed</param>
        /// <param name="inNormPrefix">The normal prefix when not selected</param>
        /// <param name="inSelPrefix">The prefix when selected</param>
        /// <param name="inColorSet">The normal text and background colors</param>
        /// <param name="inAltColorSet">The text and background colors when selected</param>
        public Button(string inText, Action<Menu>? inBehavior, 
            string inNormPrefix = "  ", string inSelPrefix = " > ", 
            ColorSet inColorSet = default, ColorSet? inAltColorSet = null) : base(inText, inColorSet)
        {
            if (inColorSet.Equals(default(ColorSet)))
                inColorSet = ColorSet.Normal;
            //Inverse colors
            if (inAltColorSet == null)
                inAltColorSet = ColorSet.Inverse;

            normalPrefix = inNormPrefix;
            selectedPrefix = inSelPrefix;
            OnPress = inBehavior;
            colors = inColorSet;
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
                    Write(selectedPrefix);
                    base.Display(selectColors.foreground, selectColors.background);
                }
                else
                {
                    Write(normalPrefix);
                    base.Display((colors ?? ColorSet.Normal).foreground, (colors ?? ColorSet.Normal).background);
                }
            }
            else
                throw new ArgumentException("This function takes a bool value");
        }

        /// <summary>
        /// Trigger this button's main action
        /// </summary>
        /// <param name="menu">The Menu this Button belongs to</param>
        /// <returns></returns>
        public bool Press(Menu menu)
        {
            if (OnPress != null)
            {
                OnPress.Invoke(menu);
                return true;
            }

            return false;
        }

    }
}
