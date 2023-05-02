using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Contains data/references for all Scenes
    /// </summary>
    public static class SceneFactory
    {
        /// <summary>
        /// The title scene
        /// </summary>
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
                            var index = used[i];
                            string info = "";
                            if (SaveLoad.SaveExists(index, HeroManager.PLAYER_SAVE_KEY))
                            {
                                var stats = SaveLoad.Load<Actor.StatsStruct>(index, HeroManager.PLAYER_SAVE_KEY);
                                info = $" [{stats.Name}: LV {stats.Level}]";
                            }

                            Scene.fileSelect.Add(new Button($"Slot {index}" + info, menu =>
                            {
                                slot = index;
                            }), false);
                        }

                        GameManager.ActiveInteractiveElement?.SetActive(false);
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

        /// <summary>
        /// The main out-of-battle Scene
        /// </summary>
        public static OverworldScene Overworld { get; } = new OverworldScene(AreaManager.SorecordForest);

        /// <summary>
        /// Starts a battle against specific enemies
        /// </summary>
        /// <param name="encounter">The encounter data to pull from</param>
        /// <returns></returns>
        public static BattleScene GetBattleScene(Encounter encounter)
        {

            //The components are filled out by the BattleScene itself
            //Switching to this scene will automatically trigger a battle
            return new BattleScene(encounter);
        }

        /// <summary>
        /// The Scene containing the end of the game
        /// </summary>
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

    -Cykie Productions
", false, -1);
                    Program.Exit();
                },
                null, Array.Empty<GUIComponent>());
            }
        }

    }

}
