using static System.Console;

namespace Capstone_Chronicles.GUI
{

    public abstract class GUIComponent
    {
        public bool Enabled { get; protected set; }
        public Scene ParentScreen { get; private set; }


        public GUIComponent(bool inEnabled)
        {
            Enabled = inEnabled;
        }

        public virtual void SetActive(bool state, bool refresh = true)
        {
            Enabled = state;
            if (refresh)
                ParentScreen.Refresh();
        }
        public abstract void Display();
        public virtual void Display(params dynamic[] args) { Display(); }

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

    public class Label : GUIComponent
    {
        public string Text { get; set; }
        public InteractiveGUI? AttachedInteractiveGUI { get; private set; }
        private Action? OnDynamicUpdate;
        public ColorSet? colors;

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
        public Label(Action dynamicText, ColorSet? inColorSet = default, bool inEnabled = true) 
            : base(inEnabled)
        {
            if (inColorSet.Equals(default(ColorSet)))
                inColorSet = ColorSet.Normal;

            OnDynamicUpdate = dynamicText;
            colors = inColorSet;
        }

        //Conversions
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
            else //Set the text normally can clear out the dynamics
            {
                Text = format;
                OnDynamicUpdate = null;
            }
        }

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
