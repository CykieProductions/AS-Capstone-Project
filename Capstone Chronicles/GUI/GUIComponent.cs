using static System.Console;

namespace Capstone_Chronicles.GUI
{
    /// <summary>
    /// The parent class for anything that displays to the screen
    /// </summary>
    public abstract class GUIComponent
    {
        /// <summary>
        /// Should I be displayed?
        /// </summary>
        public bool Enabled { get; protected set; }
        /// <summary>
        /// The <see cref="Scene"/> I'm registered to
        /// </summary>
        public Scene ParentScreen { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="GUIComponent"/> class.
        /// </summary>
        /// <param name="inEnabled">If true, display</param>
        public GUIComponent(bool inEnabled)
        {
            Enabled = inEnabled;
        }

        /// <summary>
        /// Displays or hides this GUI element
        /// </summary>
        /// <param name="state">If true, display</param>
        /// <param name="refresh">If true, refresh the screen to display immediately</param>
        public virtual void SetActive(bool state, bool refresh = true)
        {
            Enabled = state;
            if (refresh)
                ParentScreen.Refresh();
        }

        /// <summary>
        /// Displays the GUI element
        /// </summary>
        public abstract void Display();
        /// <summary>
        /// Displays the GUI element
        /// </summary>
        /// <param name="args">A list of possible arguments</param>
        public virtual void Display(params dynamic[] args) { Display(); }

        /// <summary>
        /// Action to take when this GUI element is registered to a <see cref="Scene"/>
        /// </summary>
        protected virtual void OnAttachToScene() { }

        /// <summary>
        /// Should only be used in the Screen's constructor
        /// </summary>
        public void SetParentScreen(Scene inScreen)
        {
            ParentScreen = inScreen;
            OnAttachToScene();
        }
    }

    /// <summary>
    /// A <see cref="GUIComponent"/> for displaying text
    /// </summary>
    public class Label : GUIComponent
    {
        /// <summary>
        /// The text to display
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The <see cref="InteractiveGUI"/> that this Label is connected to, if any
        /// </summary>
        public InteractiveGUI? AttachedInteractiveGUI { get; private set; }
        private Action? OnDynamicUpdate;
        /// <summary>
        /// The color and background color of the text
        /// </summary>
        public ColorSet? colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="inText">The text to display</param>
        /// <param name="inColorSet">The text and background colors to use</param>
        /// <param name="inEnabled">If true, display</param>
        public Label(string inText, ColorSet? inColorSet = default, bool inEnabled = true) 
            : base(inEnabled)
        {
            if (inColorSet.Equals(default(ColorSet)))
                inColorSet = ColorSet.Normal;

            Text = inText;
            colors = inColorSet;
        }
        /// <summary>
        /// Used to set up self-updating text.
        /// </summary>
        /// <param name="dynamicText">Function that updates the text each time it displays</param>
        /// <param name="inColorSet">The text and background colors to use</param>
        /// <param name="inEnabled">If true, display</param>
        public Label(Action dynamicText, ColorSet? inColorSet = default, bool inEnabled = true) 
            : base(inEnabled)
        {
            if (inColorSet.Equals(default(ColorSet)))
                inColorSet = ColorSet.Normal;

            OnDynamicUpdate = dynamicText;
            colors = inColorSet;
        }

        //Conversions
        /// <summary>
        /// string to Label
        /// </summary>
        public static implicit operator Label(string value) => new Label(value);

        /// <summary>
        /// Gives the Label a reference to its parent InteractiveGUI if it doesn't have one yet
        /// </summary>
        public void TryAttachToInteractiveGUI(InteractiveGUI target)
        {
            if (AttachedInteractiveGUI == null)
                AttachedInteractiveGUI = target;
        }

        /// <summary>
        /// Allow you to set the display text which can update with inputed data.
        /// </summary>
        /// <param name="format">The text format to display</param>
        /// <param name="data">Optional data to be inputted into the text</param>
        public void SetDynamicText(string format, params object[] data)
        {
            if (data != null && data.Length != 0)
            {
                Text = string.Format(format, data);
            }
            else
                Text = format;
            
        }
        /// <summary>
        /// Remove the self-updating text
        /// </summary>
        public void ClearDynamics()
        {
            OnDynamicUpdate = null;
        }

        ///<inheritdoc/>
        public override void Display()
        {
            if (colors != null)
            {
                ForegroundColor = colors.Value.foreground;
                BackgroundColor = colors.Value.background;
            }
            OnDynamicUpdate?.Invoke();
            if (Text != "")
                WriteLine(Text);
            if (colors != null)
            {
                ForegroundColor = ColorSet.DEFAULT_TEXT_COLOR;
                BackgroundColor = ColorSet.DEFAULT_BACK_COLOR;
            }
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
    
}
