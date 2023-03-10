using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
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
                    GameManager.ChangeScene(SorecordForest);
                }, false);

                var options = new List<Button>() {
                    new Button("Start", (menu) =>
                    {
                        Console.Beep(100, 60);
                        nameField.SetActive(true, false);
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

                //http://www.patorjk.com/software/taag/#p=display&h=1&v=0&f=Calvin%20S&t=%20Capstone%20%20%20%0A%20%20Chronicles
                var title = @"
  ╔═╗┌─┐┌─┐┌─┐┌┬┐┌─┐┌┐┌┌─┐      
  ║  ├─┤├─┘└─┐ │ │ ││││├┤       
  ╚═╝┴ ┴┴  └─┘ ┴ └─┘┘└┘└─┘      
    ╔═╗┬ ┬┬─┐┌─┐┌┐┌┬┌─┐┬  ┌─┐┌─┐
    ║  ├─┤├┬┘│ ││││││  │  ├┤ └─┐
    ╚═╝┴ ┴┴└─└─┘┘└┘┴└─┘┴─┘└─┘└─┘
";

                return new Scene(new GUIComponent[]
                {
                    new Menu(title, options, true),
                    nameField
                });
            }
        }

        public static OverworldScene SorecordForest
        {
            get
            {
                return new OverworldScene(null, null, Array.Empty<GUIComponent>());
            }
        }

        public static BattleScene GetBattleScene()
        {

            //The components are filled out by the BattleScene itself
            //Switching to this scene will automatically trigger a battle
            return new BattleScene(EnemyFactory.Test, EnemyFactory.Test, EnemyFactory.Test, EnemyFactory.Test, EnemyFactory.Test);
        }
    }

}
