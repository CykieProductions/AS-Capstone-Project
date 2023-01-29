using Capstone_Chronicles.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Capstone_Chronicles
{
    internal static partial class GameManager
    {
        public enum State
        {
            Exploring, Battling
        }

        public static State gameState;
        public static Scene? CurrentScene { get; private set; }
        /// <summary> The GUI element currently recieving input </summary>
        public static InteractiveGUI? ActiveInteractiveElement { get; set; }

        public static void ChangeScene(Scene scene)
        {
            CurrentScene?.Exit();
            CurrentScene = scene;
            CurrentScene?.Start();
        }
    }
}