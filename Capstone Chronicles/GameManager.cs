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
    internal static partial class GameManager
    {
        public enum State
        {
            Exploring, Battling
        }

        private const string SAVE_KEY = "General Data";
        public static State gameState;

        public static Scene CurrentScene { get; private set; }
        public static Scene PreviousScene { get; private set; }

        //Events
        public static event Action<Scene, Scene>? SceneChanged;
        public static event Action<int>? BeginSave;
        public static event Action<int>? BeginLoad;

        /// <summary> The GUI element currently receiving input </summary>
        public static InteractiveGUI? ActiveInteractiveElement { get; set; }

        public static void ChangeScene(Scene scene)
        {
            PreviousScene = CurrentScene;
            CurrentScene = scene;

            SceneChanged?.Invoke(CurrentScene, PreviousScene);

            PreviousScene?.Exit();
            CurrentScene?.Start();
        }

        //TODO implement a proper game over sequence and LOADING
        public static void GameOver()
        {
            Scene.print("-".Repeat(16) + "\nGame Over\n".Pastel(ConsoleColor.Red) + "-".Repeat(16), false, -1f);
            Area.Current.ModifyProgress(0, true);
        }

        public static void SaveGame(int slot)
        {
            SaveLoad.Save(AreaManager.areaList.IndexOf(Area.Current), slot, SAVE_KEY);
            BeginSave?.Invoke(slot);
        }
        public static void LoadGame(int slot)
        {
            SceneFactory.Overworld.Area = AreaManager.areaList[SaveLoad.Load<int>(slot, SAVE_KEY)];
            BeginLoad?.Invoke(slot);
            ChangeScene(SceneFactory.Overworld);
        }
    }
}