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
    internal class Scene
    {
        readonly Action? OnStart = null;
        readonly Action? OnExit = null;
        private List<GUIComponent> elements;

        /// <summary>
        /// For shared behavior between Scene constructors
        /// </summary>
        private void _BaseConstructor(GUIComponent[] inElements)
        {
            elements = inElements.ToList();

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
            newElement.Enabled = enabled;
            newElement.SetParentScreen(this);
            elements.Add(newElement);
        }

        public void Refresh()
        {
            Console.Clear();
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Enabled)
                    elements[i].Display();
            }
        }

        /// <summary>
        /// Called when the scene is first switched to
        /// </summary>
        public virtual void Start()
        {
            Refresh();
            OnStart?.Invoke();
        }

        /// <summary>
        /// Called right before the scene is switched.
        /// </summary>
        public virtual void Exit()
        {
            OnExit?.Invoke();
        }
    }
}
