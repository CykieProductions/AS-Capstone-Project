using Capstone_Chronicles.GUI;
using Pastel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Contains general information and functions for running the game
    /// </summary>
    internal static partial class GameManager
    {
        public enum State
        {
            Exploring, Battling
        }

        private const string SAVE_KEY = "General Data";

        /// <summary>
        /// The current state of the game
        /// </summary>
        public static State gameState;

        /// <summary>
        /// The currently loaded scene
        /// </summary>
        public static Scene CurrentScene { get; private set; }
        /// <summary>
        /// The previously loaded scene
        /// </summary>
        public static Scene PreviousScene { get; private set; }

        //Events
        /// <summary>
        /// Triggered when the current Scene is changed
        /// </summary>
        public static event Action<Scene, Scene>? SceneChanged;
        /// <summary>
        /// Triggered when the game is saved
        /// </summary>
        public static event Action<int>? BeginSave;
        /// <summary>
        /// Triggered when the game is loaded
        /// </summary>
        public static event Action<int>? BeginLoad;

        /// <summary> The GUI element currently receiving input </summary>
        public static InteractiveGUI? ActiveInteractiveElement { get; set; }

        /// <summary>
        /// Switches the current scene to a new one
        /// </summary>
        /// <param name="scene">The new scene</param>
        public static void ChangeScene(Scene scene)
        {
            PreviousScene = CurrentScene;
            CurrentScene = scene;

            SceneChanged?.Invoke(CurrentScene, PreviousScene);

            PreviousScene?.Exit();
            CurrentScene?.Start();
        }

        /// <summary>
        /// Triggers the game over sequence
        /// </summary>
        public static void GameOver()
        {
            Scene.print("-".Repeat(16) + "\nGame Over\n".Pastel(ConsoleColor.Red) + "-".Repeat(16), false, -1f);
            Area.Current.ModifyProgress(0, true);
        }

        /// <summary>
        /// Save all game data to a save slot
        /// </summary>
        /// <param name="slot">The slot to save to</param>
        public static void SaveGame(int slot)
        {
            SaveLoad.Save(AreaManager.areaList.IndexOf(Area.Current), slot, SAVE_KEY);
            BeginSave?.Invoke(slot);
        }

        /// <summary>
        /// Load all game data from a save slot
        /// </summary>
        /// <param name="slot">The slot to load from</param>
        public static void LoadGame(int slot)
        {
            SceneFactory.Overworld.Area = AreaManager.areaList[SaveLoad.Load<int>(slot, SAVE_KEY)];
            BeginLoad?.Invoke(slot);
            ChangeScene(SceneFactory.Overworld);
        }
    }
}