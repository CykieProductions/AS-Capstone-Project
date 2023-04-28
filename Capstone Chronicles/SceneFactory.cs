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
                    GameManager.ChangeScene(Overworld);
                }, false);

                var options = new List<Button>() {
                    new Button("Start", (menu) =>
                    {
                        nameField.SetActive(true, false);
                    }),
                    new Button("Load Game", menu =>
                    {
                        Scene.fileSelect.ClearOptions(false);
                        int slot = 0;
                        var used = SaveLoad.GetUsedSaveSlots();

                        for (int i = 0; i < used.Count; i++)
                        {
                            var index = i;
                            Scene.fileSelect.Add(new Button($"Slot {used[index]}", menu =>
                            {
                                slot = used[index];
                            }), false);
                        }

                        GameManager.ActiveInteractiveElement.SetActive(false);
                        Scene.fileSelect.SetActive(true);

                        GameManager.LoadGame(slot);
                        Scene.fileSelect.ClearOptions(true);
                    }),
                    new Button("Exit", (menu) =>
                    {
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

        public static OverworldScene Overworld { get; } = new OverworldScene(AreaManager.SorecordForest);


        public static BattleScene GetBattleScene(Encounter encounter)
        {

            //The components are filled out by the BattleScene itself
            //Switching to this scene will automatically trigger a battle
            return new BattleScene(encounter);
        }

        public static Scene EndingScene {
            get
            {
                return new Scene(() =>
                {
                    Scene.Current.ToggleInfoLabels(true);

                    Scene.print(
@$"You made this climb to find out the truth of the legendary Capstone and here it is in front of you.
All {HeroManager.Party.Count} of you look at each other and nod.
{HeroManager.Player.Name} reaches their hand out to touch it and suddenly a blinding light surrounds the team.
The rumors were true, it really is a portal!
"
                    , false, -1, false);

                    Scene.print("It seems the adventure is only just beginning!", false, -1, false);
                    Scene.print(
@"

 ▀█▀ █▄█ ▄▀▄ █▄ █ █▄▀ ▄▀▀   █▀ ▄▀▄ █▀▄   █▀▄ █   ▄▀▄ ▀▄▀ █ █▄ █ ▄▀ 
  █  █ █ █▀█ █ ▀█ █ █ ▄██   █▀ ▀▄▀ █▀▄   █▀  █▄▄ █▀█  █  █ █ ▀█ ▀▄█


", false, -1);
                    Program.Exit();
                },
                null, Array.Empty<GUIComponent>());
            }
        }

    }

}
