using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Acts as the mediator between the GUI and the Logic
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// Same as GameManager.CurrentScene
        /// </summary>
        public static Scene Current { get => GameManager.CurrentScene; }
        public static Scene Previous { get => GameManager.PreviousScene; }
        public static bool CanGoBack { get; protected set; }

        /// <summary>
        /// Set false when the scene refreshes or changes
        /// </summary>
        protected static bool goBack;
        public static bool EnablePrinting { get; set; } = true;

        readonly Action? OnStart = null;
        readonly Action? OnExit = null;
        private List<GUIComponent> elements;

        public Action? OnRenderComplete;

        //! Gameplay 
        protected static Label skillInfoLabel = new("");

        protected static readonly Menu SkillMenuTemplate = new Menu("Select a skill",
            new()
            {

            }, wrap: true, remember: false, inRowLimit: 6, moveAction: TryUpdateSkillDescription, onClose: () =>
            {
                skillInfoLabel.SetActive(false, false);
            });

        //

        //Default way to print stuff to the screen
        protected Label[] infoLabels = new Label[]
        {
            new Label("", null, false),
            new Label("", null, false),
        };

        /// <summary>
        /// For shared behavior between Scene constructors
        /// </summary>
        private void _BaseConstructor(GUIComponent[] inElements)
        {
            elements = inElements.ToList();
            for (int i = 0; i < infoLabels.Length; i++)
            {
                AddGUIElement(infoLabels[i], false);
            }

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].SetParentScreen(this);
            }
        }
        public Scene(GUIComponent[] inElements)
        {
            _BaseConstructor(inElements);
        }
        public Scene(Action? startAction, Action? exitAction, GUIComponent[] inElements)
        {
            OnStart = startAction;
            OnExit = exitAction;
            _BaseConstructor(inElements);
        }
        
        public void AddGUIElement(GUIComponent newElement, bool enabled)
        {
            newElement.SetParentScreen(this);
            if (!elements.Contains(newElement))
                elements.Add(newElement);
            newElement.SetActive(enabled, false);
        }

        public static void BackButtonPressed()
        {
            if (CanGoBack)
                goBack = true;
        }

        public void Refresh()
        {
            Console.Clear();
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Enabled)
                    elements[i].Display();
            }

            goBack = false;
            OnRenderComplete?.Invoke();
        }

        protected static void TryUpdateSkillDescription(Menu menu)
        {
            if (skillInfoLabel.ParentScreen == null)
                return;

            var skill = SkillManager.GetSkillByName(menu.GetOptionAt(menu.SelectedIndex)?.Text);
            if (skill == null)
            {
                skillInfoLabel.Text = "-----";
                return;
            }

            string targetText = skill.targetType.ToString().Replace('_', ' ');

            skillInfoLabel.Text = $"Skill Info: \nCost: {skill.Cost} | Element: {skill.element.Name} " +
                $"| Type: {skill.actionType} | Targets: {targetText}";

        }

        /// <summary>
        /// Displays information using the appropriate logic for the current scene type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="singleLine"></param>
        /// <param name="delayAfter"></param>
        public static void print<T>(T message, bool singleLine = false, float delayAfter = 0.75f)
        {
            if (EnablePrinting)
                GameManager.CurrentScene?.Log(message, singleLine, delayAfter);
        }

        protected virtual void Log<T>(T message, bool singleLine = false, float delayAfter = 0.75f)
        {
            infoLabels[1].colors = (Console.ForegroundColor, Console.BackgroundColor);
            infoLabels[1].Text += message?.ToString() + (singleLine ? "" : '\n');

            if (infoLabels[0].Enabled || infoLabels[1].Enabled)
                GameManager.CurrentScene.Refresh();

            if (!singleLine)
            {
                infoLabels[0].colors = infoLabels[1].colors;
                infoLabels[0].Text = infoLabels[1].Text;
                infoLabels[1].Text = "";
            }

            //Allow player to skip the delay between lines
            if (delayAfter > 0)
            {
                int iterations = 5;
                for (int i = 0; i < iterations; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(delayAfter / iterations));
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        break;
                    }
                }
            }
            else if (delayAfter < 0)
            {
                Console.ReadKey(true);
            }
        }

        public void ToggleInfoLabels(bool enabled)
        {
            if (enabled == true)
            {
                for (int i = 0; i < infoLabels.Length; i++)
                {
                    infoLabels[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < infoLabels.Length; i++)
                {
                    infoLabels[i].Text = "";
                    infoLabels[i].SetActive(false);
                }
            }
        }

        /// <summary>
        /// Called when the scene is first switched to
        /// </summary>
        public virtual void Start()
        {
            EnablePrinting = true;
            Refresh();
            OnStart?.Invoke();
        }

        /// <summary>
        /// Called right before the scene is switched.
        /// </summary>
        public virtual void Exit()
        {
            goBack = false;
            CanGoBack = false;
            OnExit?.Invoke();
        }
    }
}
