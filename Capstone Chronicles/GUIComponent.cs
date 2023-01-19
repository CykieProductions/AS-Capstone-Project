using System.Net.Http.Headers;
using static System.Console;

namespace Capstone_Chronicles
{
    internal abstract class GUIComponent
    {
        //x public bool enabled;
        public Screen ParentScreen { get; private set; }

        public abstract void Display();
        public virtual void Display(params dynamic[] args) { Display(); }

        /// <summary>
        /// Should only be used in the Screen's constructor
        /// </summary>
        public void SetParentScreen(Screen inScreen)
        {
            ParentScreen = inScreen;
        }
    }

    internal class Label : GUIComponent
    {
        public string Text { get; private set; }
        ColorSet colors;

        public Label(string inText, ColorSet inColorSet = default)
        {
            if (inColorSet.Equals(default(ColorSet)))
                inColorSet = ColorSet.Normal;

            Text = inText;
            colors = inColorSet;
        }

        //Conversions
        public static implicit operator Label(string value) => new Label(value);

        public override void Display()
        {
            ForegroundColor = colors.foreground;
            BackgroundColor = colors.background;
            WriteLine(Text);
            ForegroundColor = ColorSet.DEFAULT_TEXT_COLOR;
            BackgroundColor = ColorSet.DEFAULT_BACK_COLOR;
        }

        /// <summary>
        /// Displays text with the specified colors
        /// </summary>
        /// <param name="args">[0] is the new foreground color. [1] is the new background color</param>
        public override void Display(params dynamic[] args)
        {
            if (args[0] is ConsoleColor && args[1] is ConsoleColor)
            {
                ForegroundColor = args[0];
                BackgroundColor = args[1];
                WriteLine(Text);
                ForegroundColor = ColorSet.DEFAULT_TEXT_COLOR;
                BackgroundColor = ColorSet.DEFAULT_BACK_COLOR;
            }
            else
                throw new ArgumentException();
        }
    }
    internal class Button : Label
    {
        ColorSet altColors;
        string prefix;
        readonly Action OnPress;

        public Button(string inText, Action inAction, string inPrefix = ">>", ColorSet inColorSet = default, ColorSet? inAltColorSet = null) : base (inText, inColorSet)
        {
            if (inColorSet.Equals(default(ColorSet)))
                inColorSet = ColorSet.Normal;
            //Inverse colors
            if (inAltColorSet == null)
                inAltColorSet = ColorSet.Inverse;

            prefix = inPrefix;
            OnPress = inAction;
            altColors = inAltColorSet.Value;
        }

        /// <summary>
        /// Displays text with the specified colors
        /// </summary>
        /// <param name="args">[0] is the new foreground color. [1] is the new background color</param>
        public override void Display(params dynamic[] args)
        {
            if (args[0] is ConsoleColor && args[1] is ConsoleColor)
            {
                Write(prefix);
                base.Display(args);
            }
            else
                throw new ArgumentException();
        }

        internal void Press()
        {
            OnPress.Invoke();
        }
    }

    internal class Menu : GUIComponent
    {
        Label? prompt;
        Button[] options;
        int selectedIndex = 0;
        bool wrapAround = false;

        ColorSet highlightColors;

        public Menu(Label? inPrompt, Button[] inOptions, 
            ColorSet? altForeColors = null) 
        {
            if (altForeColors == null)
                altForeColors = ColorSet.Inverse;

            prompt = inPrompt;
            options = inOptions;

            highlightColors = altForeColors.Value;
        }

        public override void Display()
        {
            prompt?.Display();

            for (int i = 0; i < options.Length; i++)
            {
                if (selectedIndex == i)
                    options[i].Display(highlightColors.foreground, highlightColors.background);
                else
                    options[i].Display();
            }

            WaitForChoice();
        }

        void WaitForChoice()
        {
            if (Screen.ActiveMenu == this)
                return;

            Screen.ActiveMenu = this;
            while(Screen.ActiveMenu == this)
            {
                var inputKey = ReadKey();
                if (inputKey.Key == ConsoleKey.DownArrow)
                {
                    //Keep button index in bounds
                    if (selectedIndex >= options.Length - 1)
                    {
                        if (wrapAround)
                            selectedIndex = 0;
                        else
                            continue;//No reason to refresh
                    }
                    else
                        selectedIndex++;
                }
                else if (inputKey.Key == ConsoleKey.UpArrow)
                {
                    //Keep button index in bounds
                    if (selectedIndex <= 0)
                    {
                        if (wrapAround)
                            selectedIndex = options.Length - 1;
                        else
                            continue;//No reason to refresh
                    }
                    else
                        selectedIndex--;
                }
                else if (inputKey.Key == ConsoleKey.Enter)
                {
                    options[selectedIndex].Press();
                }
                else
                    continue;//Don't refresh if nothing valid was pressed

                ParentScreen.Refresh();
            }
        }

    }
}
