﻿using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles.GUI
{

    /// <summary>
    /// A <see cref="GUIComponent"/> that the user can interact with
    /// </summary>
    public abstract class InteractiveGUI : GUIComponent
    {
        /// <summary>
        /// Info on what the user is expected to do
        /// </summary>
        public Label? Prompt { get; protected set; }

        /// <summary>
        /// The interactive element will close unless this is set false by the ending behavior
        /// </summary>
        public bool closeAfterConfirm = true;
        /// <summary>
        /// The event to trigger when closing the GUI element
        /// </summary>
        protected Action? OnClose = null;

        /// <summary>
        /// Constructs a new <see cref="InteractiveGUI"/>
        /// </summary>
        /// <param name="inPrompt">The prompt to display</param>
        /// <param name="inEnabled">If true, display</param>
        /// <param name="onClose">The action to trigger when closing</param>
        protected InteractiveGUI(Label? inPrompt, bool inEnabled, Action? onClose = null)
            : base(inEnabled)
        {
            Prompt = inPrompt;
            Prompt?.TryAttachToInteractiveGUI(this);
            OnClose = onClose;
        }

        /// <summary>
        /// Ensures that the user only interacts with one element at a time. Always run the base function before an override.
        /// </summary>
        protected virtual void RunInteraction ()
        {
            if (!Program.GameIsRunning)
                GameManager.ActiveInteractiveElement = null;

            if (GameManager.ActiveInteractiveElement == this)
                return;

            GameManager.ActiveInteractiveElement = this;
        }

        ///<inheritdoc/>
        public override void SetActive(bool state, bool refresh = true)
        {
            if (state == false)
                TryClose(true);

            base.SetActive(state, refresh);
        }

        /// <summary>
        /// Checks if the element was successfully activated and then resets it back to default
        /// </summary>
        /// <returns>True if it succeeded; False if not </returns>
        protected virtual bool TryClose(bool forced = false)
        {
            bool result = forced || closeAfterConfirm;
            closeAfterConfirm = true;

            if (result)
            {
                OnClose?.Invoke();
                GameManager.ActiveInteractiveElement = null;
                Enabled = false;
            }

            return result;
        }

    }

    /// <summary>
    /// An interactive GUI element that allows the user to input text
    /// </summary>
    public class InputField : InteractiveGUI
    {
        /// <summary>
        /// The inputted string
        /// </summary>
        public string Input { get; protected set; } = "";
        readonly Action<string> OnSend;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputField"/> class.
        /// </summary>
        /// <param name="inPrompt">The prompt</param>
        /// <param name="onSend">The event to trigger when sending your input</param>
        /// <param name="inEnabled">If true, in display</param>
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
    /// Also use to create a 
    /// </summary>
    public class Menu : InteractiveGUI
    {
        ColorSet? highlightColors = null;
        List<Button> options;

        int selectedIndex = 0;
        /// <summary>
        /// The index of the currently selected option
        /// </summary>
        public int SelectedIndex { get => selectedIndex;
            set
            {
                selectedIndex = value.Clamp(0, options.Count - 1);
                ParentScreen.Refresh();
            }
        }

        /// <summary>
        /// The event to trigger when changing selections
        /// </summary>
        public readonly Action<Menu>? OnMove = null;

        private readonly bool wrapAround = false;
        private readonly bool rememberLastIndex = false;
        private readonly int rowLimit = 0;//Zero means no limit


        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        /// <param name="inPrompt">The prompt</param>
        /// <param name="inOptions">The options</param>
        /// <param name="wrap">If true, enable wrapping around to the first or last option</param>
        /// <param name="remember">If true, continue the last selected option when redisplayed</param>
        /// <param name="inRowLimit">The row limit</param>
        /// <param name="moveAction">The event to trigger when changing selections</param>
        /// <param name="onClose">The event to trigger when closing</param>
        /// <param name="altSelectColors">The alternate selection colors</param>
        /// <param name="inEnabled">If true, in display</param>
        public Menu(Label? inPrompt, List<Button> inOptions, bool wrap = false, bool remember = false, int inRowLimit = 0,
            Action<Menu>? moveAction = null, Action? onClose = null, ColorSet? altSelectColors = null, bool inEnabled = true)
            : base(inPrompt, inEnabled)
        {
            //If you set this then it will override the highlight color for the buttons
            if (altSelectColors != null)
                highlightColors = altSelectColors.Value;

            Prompt = inPrompt;
            options = inOptions;
            wrapAround = wrap;
            rememberLastIndex = remember;
            rowLimit = inRowLimit < 0 ? 0 : inRowLimit;//minimum of 0
            OnMove = moveAction;
            OnClose = onClose;
        }

        /// <summary>
        /// Creates a clone of this object
        /// </summary>
        /// <returns>A copy of this Menu with the all the same values</returns>
        public Menu ShallowCopy()
        {
            return (Menu)MemberwiseClone();
        }

        /// <summary>
        /// Get the option at an index
        /// </summary>
        /// <param name="i">The index</param>
        /// <returns>A Button</returns>
        public Button GetOptionAt(int i)
        {
            if (options.Count == 0)
                return null;

            return options[i.Clamp(0, options.Count - 1)];
        }

        /// <summary>
        /// Copies the options from another Menu
        /// </summary>
        /// <param name="menu"></param>
        public void SetOptions(Menu menu)
        {
            options = menu.options.ToList();
        }
        /// <summary>
        /// Add an option
        /// </summary>
        /// <param name="button">The Button to add</param>
        /// <param name="refresh">If true, refresh the scene immediately</param>
        public void Add(Button button, bool refresh)
        {
            options.Add(button);
            if (refresh)
                ParentScreen.Refresh();
        }
        /// <summary>
        /// Adds a list of Buttons
        /// </summary>
        /// <param name="buttons">The Buttons to add</param>
        public void Add(params Button[] buttons)
        {
            if (buttons.Length == 1)
                options.Add(buttons[0]);
            else
                options.AddRange(buttons);
            ParentScreen.Refresh();
        }

        /// <summary>
        /// Removes an option by name
        /// </summary>
        /// <param name="name">The name of the Button to remove</param>
        /// <param name="refresh">If true, refresh the scene immediately</param>
        public void Remove(string name, bool refresh)
        {
            options.RemoveAll(x => x.Text == name);
            if (refresh)
                ParentScreen.Refresh();
        }
        /// <summary>
        /// Removes an option at an index
        /// </summary>
        /// <param name="index">The index of the Button to remove</param>
        /// <param name="refresh">If true, refresh the scene immediately</param>
        public void Remove(int index, bool refresh)
        {
            options.RemoveAt(index);
            if (refresh)
                ParentScreen.Refresh();
        }

        /// <summary>
        /// Removes all options
        /// </summary>
        /// <param name="refresh">If true, refresh the scene immediately</param>
        public void ClearOptions(bool refresh = true)
        {
            options.Clear();
            if (refresh)
                ParentScreen.Refresh();
        }

        protected override void OnAttachToScene()
        {
            ParentScreen.OnRenderComplete += RunInteraction;
        }

        public override void Display()
        {
            //Intercept any buffered inputs
            while (Console.KeyAvailable) Console.ReadKey(true);

            if (!Program.GameIsRunning)
                TryClose(true);

            Prompt?.Display();

            //ensure button index is in bounds
            if (selectedIndex >= options.Count)
                selectedIndex = options.Count - 1;
            else if (selectedIndex < 0)
                selectedIndex = 0;

            int longestInCurColumn = 0;
            int totalColumnOffset = 0;
            int top = GetCursorPosition().Top;
            int curColumn = 0;

            for (int i = 0; i < options.Count; i++)
            {
                if (rowLimit > 0 && i >= rowLimit && (float)(i + rowLimit) % rowLimit == 0)//form a new row if needed
                {
                    curColumn++;
                    totalColumnOffset += longestInCurColumn + 5;
                    longestInCurColumn = 0;
                }

                if (rowLimit > 0 && options[i].Text.Length > longestInCurColumn)
                    longestInCurColumn = options[i].Text.Length;

                if (rowLimit > 0 && i >= rowLimit)
                {
                    SetCursorPosition(totalColumnOffset, top + i % rowLimit);
                }

                if (selectedIndex == i)
                {
                    if (highlightColors != null)
                        options[i].selectColors = highlightColors.Value;
                    options[i].Display(true);
                }
                else
                    options[i].Display(false);

            }

            SetCursorPosition(0, GetBottomCursorPosistion(top));

            //RunInteraction(); //Now waits for the OnRenderComplete event
        }

        /// <summary>
        /// Calculates the lowest point that the cursor will reach when displaying this menu.
        /// </summary>
        /// <param name="start">Where to start calculating from. Auto-calculates if less than 0.</param>
        /// <returns></returns>
        public int GetBottomCursorPosistion(int start = -1)
        {
            if (start < 0 && Prompt != null)//Auto-calculate top from the number of lines in the Prompt if it exist
            {
                var text = Prompt.Text;
                //new line marker can be "\n" or "\r\n"
                start = (text.Length - text.Replace(Environment.NewLine, string.Empty).Length) / Environment.NewLine.Length;
            }
            else
                start = 0;

            var height = options.Count;
            if (rowLimit > 0)//if rows are restricted 
                height = Math.Min(rowLimit, options.Count);

            return start + height + 2;
        }

        /// <summary>
        /// Contains logic for scrolling through options and selecting one
        /// </summary>
        protected override void RunInteraction()
        {
            if (!Enabled)
                return;

            base.RunInteraction();

            while (GameManager.ActiveInteractiveElement == this)
            {
                var inputKey = ReadKey(true);

                //! Back Function
                if (inputKey.Key == ConsoleKey.Escape && Scene.CanGoBack)
                {
                    Scene.BackButtonPressed();
                    TryClose(true);
                    return;
                }

                bool down = inputKey.Key == ConsoleKey.DownArrow;
                bool up = inputKey.Key == ConsoleKey.UpArrow;
                bool right = rowLimit > 0 && inputKey.Key == ConsoleKey.RightArrow;
                bool left = rowLimit > 0 && inputKey.Key == ConsoleKey.LeftArrow;

                if (down)
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
                else if (up)
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
                else if (right)
                {
                    if (wrapAround && selectedIndex + rowLimit > options.Count - 1)//overflow
                    {
                        var prev = selectedIndex;
                        selectedIndex = (selectedIndex + rowLimit) % rowLimit;
                        //if you would wrap around to yourself then skip to the end
                        selectedIndex = selectedIndex == prev ? options.Count - 1 : selectedIndex;
                    }
                    else if (selectedIndex + rowLimit < options.Count)//normal
                    {
                        selectedIndex += rowLimit;
                    }
                    else
                        continue;
                }
                else if (left)
                {
                    if (wrapAround && selectedIndex - rowLimit < 0)//wrap
                    {
                        var prev = selectedIndex;
                        selectedIndex = options.Count - 1 - ((selectedIndex + rowLimit) % rowLimit);
                        //if you would wrap around to yourself then skip to the end
                        selectedIndex = selectedIndex == prev ? options.Count - 1 : selectedIndex + 1;
                    }
                    else if (selectedIndex - rowLimit >= 0)//normal
                    {
                        selectedIndex -= rowLimit;
                    }
                    else
                        continue;
                }
                else if (inputKey.Key == ConsoleKey.Enter)
                {
                    if (selectedIndex >= 0 && selectedIndex < options.Count)
                    {
                        if (!options[selectedIndex].Press(this))//If the button has no function just continue
                            continue;
                    }

                    if (!rememberLastIndex)
                        selectedIndex = 0;
                    //The button can stop this from happening if needed
                    TryClose();
                }
                else
                    continue;//Don't refresh if nothing valid was pressed

                if (down || up || right || left)
                    OnMove?.Invoke(this);

                ParentScreen.Refresh();
            }
        }

    }
}
