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
        public static Scene CurrentScene { get; private set; }
        public static Scene PreviousScene { get; private set; }

        public static event Action<Scene, Scene>? SceneChanged;

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
            foreach (var hero in HeroManager.Party)
            {
                hero.SetHealth(hero.MaxHp);
                hero.SetStamina(hero.MaxSp);
            }
            ChangeScene(SceneFactory.TitleScreen);
        }
    }
}