using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    internal static partial class GameManager
    {
        public static class SceneFactory
        {
            public static Scene TitleScreen 
            { 
                get
                {
                    InputField nameField = new InputField("What is your name?", input =>
                    {
                        HeroManager.Player.Name = input;
                        ChangeScene(GetBattleScene());
                    }, false);

                    var options = new Button[] {
                    new Button("Start", (menu) =>
                    {
                        Console.Beep(100, 60);
                        nameField.Enabled = true;
                    }),
                    new Button("Options", (menu) =>
                    {
                        Console.Beep(150, 160);
                        menu.closeAfterConfirm = false;
                    }),
                    new Button("Exit", (menu) =>
                    {
                        Console.Beep(250, 20);
                        Program.Exit();
                    })
                    };

                    return new Scene(new GUIComponent[] 
                    {
                        new Menu("Capstone Chronicles", options, true),
                        nameField 
                    });
                } 
            }

            public static BattleScene GetBattleScene()
            {

                //The components are filled out by the BattleScene itself
                //Switching to this scene will automatically trigger a battle
                return new BattleScene(EnemyFactory.Test, EnemyFactory.Test);
            }
        }
    }
}
