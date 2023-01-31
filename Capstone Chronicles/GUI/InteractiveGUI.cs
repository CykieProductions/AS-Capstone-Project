using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles.GUI
{
    internal abstract class InteractiveGUI : GUIComponent
    {
        public Label? Prompt { get; protected set; }

        /// <summary>
        /// The interactive element will close unless this is set false by the ending behaviour
        /// </summary>
        public bool closeAfterConfirm = true;

        protected InteractiveGUI(Label? inPrompt, bool inEnabled)
            : base(inEnabled)
        {
            Prompt = inPrompt;
            Prompt?.TryAttachToInteractiveGUI(this);
        }

        /// <summary>
        /// Ensures that the user only interacts with one element at a time. Always run the base function before an override.
        /// </summary>
        protected virtual void RunInteraction ()
        {
            if (GameManager.ActiveInteractiveElement == this)
                return;

            GameManager.ActiveInteractiveElement = this;
        }

        /// <summary>
        /// Checks if the element was successfully activated and then resets it back to default
        /// </summary>
        /// <returns>True if it succeeded; False if not </returns>
        protected virtual bool TryClose()
        {
            bool result = closeAfterConfirm;
            closeAfterConfirm = true;

            if (result)
            {
                GameManager.ActiveInteractiveElement = null;
                Enabled = false;
            }

            return result;
        }

    }

    /// <summary>
    /// An interactive GUI element that allows the user to input text
    /// </summary>
    internal class InputField : InteractiveGUI
    {
        public string Input { get; protected set; } = "";
        readonly Action<string> OnSend;

        public InputField(Label? inPrompt, Action<string> onSend, bool inEnabled = true) 
            : base (inPrompt, inEnabled)
        {
            OnSend = onSend;
        }

        public override void Display()
        {
            Prompt?.Display();
            RunInteraction();
        }

        protected override void RunInteraction()
        {
            if (!Program.GameIsRunning)
            {
                GameManager.ActiveInteractiveElement = null;
                return;
            }

            base.RunInteraction();

            Input = ReadLine();
            if (Input != null)
                OnSend.Invoke(Input);
            
            TryClose();
        }

    }

    /// <summary>
    /// An interactive GUI element that allows the user to chose from a list of buttons.
    /// </summary>
    internal class Menu : InteractiveGUI
    {
        ColorSet? highlightColors = null;
        List<Button> options;
        int selectedIndex = 0;
        private bool wrapAround = false;


        public Menu(Label? inPrompt, List<Button> inOptions, bool wrap = false,
            ColorSet? altForeColors = null, bool inEnabled = true)
            : base(inPrompt, inEnabled)
        {
            //If you set this then it will override the highlight color for the buttons
            if (altForeColors != null)
                highlightColors = altForeColors.Value;

            Prompt = inPrompt;
            options = inOptions;
            wrapAround = wrap;
        }

        public void Add(Button button, bool refresh)
        {
            options.Add(button);
            if (refresh)
                ParentScreen.Refresh();
        }
        public void Add(params Button[] buttons)
        {
            if (buttons.Length == 1)
                options.Add(buttons[0]);
            else
                options.AddRange(buttons);
            ParentScreen.Refresh();
        }
        public void ClearOptions(bool refresh = true)
        {
            options.Clear();
            if (refresh)
                ParentScreen.Refresh();
        }

        public override void Display()
        {
            Prompt?.Display();

            //ensure button index is in bounds
            if (selectedIndex >= options.Count)
                selectedIndex = options.Count - 1;
            else if (selectedIndex < 0)
                selectedIndex = 0;

            for (int i = 0; i < options.Count; i++)
            {
                if (selectedIndex == i)
                {
                    if (highlightColors != null)
                        options[i].selectColors = highlightColors.Value;
                    options[i].Display(true);
                }
                else
                    options[i].Display();
            }

            RunInteraction();
        }

        /// <summary>
        /// Contains logic for scrolling through options and selecting one
        /// </summary>
        protected override void RunInteraction()
        {
            base.RunInteraction();

            while (GameManager.ActiveInteractiveElement == this)
            {
                var inputKey = ReadKey(true);

                if (inputKey.Key == ConsoleKey.DownArrow)
                {
                    //Keep button index in bounds
                    if (selectedIndex >= options.Count - 1)
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
                            selectedIndex = options.Count - 1;
                        else
                            continue;//No reason to refresh
                    }
                    else
                        selectedIndex--;
                }
                else if (inputKey.Key == ConsoleKey.Enter)
                {
                    if (selectedIndex >= 0 && selectedIndex < options.Count)
                        options[selectedIndex].Press(this);
                    else
                        throw new IndexOutOfRangeException();

                    //The button can stop this from happening if needed
                    TryClose();
                }
                else
                    continue;//Don't refresh if nothing valid was pressed

                ParentScreen.Refresh();
            }
        }

    }
}
