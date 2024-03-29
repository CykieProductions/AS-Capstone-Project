﻿using System;
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
        /// <summary>
        /// Same as GameManager.PreviousScene
        /// </summary>
        public static Scene Previous { get => GameManager.PreviousScene; }
        /// <summary>
        /// Will the back button work?
        /// </summary>
        public static bool CanGoBack { get; protected set; }

        /// <summary>
        /// Set false when the scene refreshes or changes
        /// </summary>
        protected static bool goBack;
        /// <summary>
        /// Should I be able to display text through the print function?
        /// </summary>
        public static bool EnablePrinting { get; set; } = true;

        readonly Action? OnStart = null;
        readonly Action? OnExit = null;
        private List<GUIComponent> elements;

        /// <summary>
        /// Triggered when a the screen is redisplayed
        /// </summary>
        public Action? OnRenderComplete;

        public static readonly Menu fileSelect = new("Which File?", new(), true, false, 4, inEnabled: false);

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

        /// <summary>
        /// Default way to print stuff to the screen
        /// </summary>
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
            if (!Program.GameIsRunning)
                return;

            elements = inElements.ToList();
            for (int i = 0; i < infoLabels.Length; i++)
            {
                AddGUIElement(infoLabels[i], false);
            }
            AddGUIElement(fileSelect, false);

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].SetParentScreen(this);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class.
        /// </summary>
        /// <param name="inElements">The GUI elements in this Scene</param>
        public Scene(GUIComponent[] inElements)
        {
            _BaseConstructor(inElements);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class.
        /// </summary>
        /// <param name="startAction">The start action</param>
        /// <param name="exitAction">The exit action</param>
        /// <param name="inElements">The GUI elements in this Scene</param>
        public Scene(Action? startAction, Action? exitAction, GUIComponent[] inElements)
        {
            OnStart = startAction;
            OnExit = exitAction;
            _BaseConstructor(inElements);
        }

        /// <summary>
        /// Adds a GUI element to the Scene
        /// </summary>
        /// <param name="newElement">The new element</param>
        /// <param name="enabled">If true, display the new element</param>
        public void AddGUIElement(GUIComponent newElement, bool enabled)
        {
            newElement.SetParentScreen(this);
            if (!elements.Contains(newElement))
                elements.Add(newElement);
            newElement.SetActive(enabled, false);
        }

        /// <summary>
        /// The player pressed the back button, so try to go back
        /// </summary>
        public static void BackButtonPressed()
        {
            if (CanGoBack)
                goBack = true;
        }

        /// <summary>
        /// Clear out and then redisplays all GUI elements
        /// </summary>
        public void Refresh()
        {
            Console.Clear();
            if (!Program.GameIsRunning)
                return;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Enabled)
                    elements[i].Display();
            }

            goBack = false;
            OnRenderComplete?.Invoke();
        }

        /// <summary>
        /// Display information on the currently selected skill
        /// </summary>
        /// <param name="menu"></param>
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
        /// <param name="message">The message to display</param>
        /// <param name="singleLine">Should I stay on the same line afterward</param>
        /// <param name="delayAfter">Delay before moving on to the next line (-1 means manual)</param>
        /// <param name="instant">If true, don't print the message char by char</param>
        public static void print<T>(T message, bool singleLine = false, float delayAfter = 0.75f, bool instant = true)
        {
            if (EnablePrinting)
                GameManager.CurrentScene?.Log(message, singleLine, delayAfter, instant ? 0 : 0.02f);
        }
        /// <summary>
        /// Displays information using the appropriate logic for the current scene type
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="singleLine">Should I stay on the same line afterward</param>
        /// <param name="delayAfter">Delay before moving on to the next line (-1 means manual)</param>
        /// <param name="textSpeed">The delay before printing another char</param>
        protected virtual void Log<T>(T message, bool singleLine = false, float delayAfter = 0.75f, float textSpeed = 0)
        {
            var text = message?.ToString();
            infoLabels[1].colors = (Console.ForegroundColor, Console.BackgroundColor);

            if (textSpeed <= 0)
            {
                infoLabels[1].Text += text + (singleLine ? "" : '\n');

                if (infoLabels[0].Enabled || infoLabels[1].Enabled)
                    GameManager.CurrentScene.Refresh();
            }
            else if (!string.IsNullOrEmpty(text)) //write out text char by char
            {

                for (int i = 0; i < text.Length; i++)
                {
                    infoLabels[1].Text += text[i];

                    float wait = textSpeed;
                    if (text[i] == '.' || text[i] == '!' || text[i] == '?')
                        wait *= 50;

                    if (Console.KeyAvailable)
                    {
                        //Skip to the end
                        if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                        {
                            infoLabels[1].Text += text[++i..] + (singleLine ? "" : '\n');
                            GameManager.CurrentScene.Refresh();
                            break;
                        }
                        else
                        {
                            wait = 0f;
                        }
                    }

                    if (infoLabels[0].Enabled || infoLabels[1].Enabled)
                        GameManager.CurrentScene.Refresh();
                    if (wait > 0f)
                        Thread.Sleep(TimeSpan.FromSeconds(wait));
                }
                infoLabels[1].Text += singleLine ? "" : '\n';
            }

            if (!singleLine)
            {
                infoLabels[0].colors = infoLabels[1].colors;
                infoLabels[0].Text = infoLabels[1].Text;
                infoLabels[1].Text = "";
            }

            if (!infoLabels[0].Enabled || !infoLabels[1].Enabled)
                return;

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

        /// <summary>
        /// Toggles the info labels on or off
        /// </summary>
        /// <param name="enabled">If true, display the info labels</param>
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
        /// Called when the scene is switched to
        /// </summary>
        public virtual void Start()
        {
            EnablePrinting = true;
            Refresh();
            OnStart?.Invoke();
        }

        /// <summary>
        /// Called right before the scene is switched away from
        /// </summary>
        public virtual void Exit()
        {
            goBack = false;
            CanGoBack = false;
            OnExit?.Invoke();
        }
    }
}
