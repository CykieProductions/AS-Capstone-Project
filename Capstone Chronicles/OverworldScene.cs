using Capstone_Chronicles.GUI;
using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    /// <summary>
    /// The main <see cref="Scene"/> type outside of battles
    /// </summary>
    public class OverworldScene : Scene
    {

        public Area Area { get; set; }

        static Hero curHero = HeroManager.Party[0];

        #region GUI References

        static readonly Menu areaMenu = new("Which Area?", new(), true, false, 4, inEnabled: false);

        static readonly Menu ActionMenuTemplate = new Menu(
            new Label(dynamicText: () =>
            {
                actionMenu.Prompt.Text = "";
                foreach (var hero in HeroManager.Party)
                {
                    var format = "{0}\t HP: {1}/{2} | SP: {3}/{4}";
                    actionMenu.Prompt.SetDynamicText(actionMenu.Prompt.Text + "\n" + format,
                        hero.Name, hero.Hp, hero.MaxHp,
                        hero.Sp, hero.MaxSp);
                }

                actionMenu.Prompt.SetDynamicText($"{actionMenu.Prompt.Text}\n\n" +
                    $"Progress through {Area.Current.Name}: {Area.Current.Progress}/{Area.Current.Length}\n");

                //CanGoBack = false;
            }),
            new()
            {
                new Button("Proceed", menu =>
                {
                    Area area = ((OverworldScene)Current).Area;

                    if (!area.TryGetFixedEncounter(out Encounter encounter))
                    {
                        encounter = area.GetRandomEncounter();
                    }

                    GameManager.ChangeScene(SceneFactory.GetBattleScene(encounter));
                }),
                new Button("Skills", menu =>
                {
                Start:
                    curHero = RunTargetMenu(actionMenu, false);
                    if (curHero == null) return;

                    if (curHero.skills.Count > 0)
                    {
                        if (!RunSkillMenu())
                            goto Start;
                    }
                    else
                    {
                        menu.closeAfterConfirm = false;
                    }
                }),
                new Button("Stats", menu =>
                {
                    curHero = RunTargetMenu(actionMenu);

                    if (curHero!= null)
                        DisplayStats(curHero);
                }),
                new Button("Warp", menu =>
                {
                    Area area;
                    for (int i = 0; i < AreaManager.areaList.Count; i++)
                    {
                        var name = AreaManager.areaList[i].Name;
                        if (!AreaManager.areaList[i].Started)
#if !DEBUG
                            continue;
#else
                            name += " [Skip]";
#endif
                        var index = i;
                        areaMenu.Add(new Button(name, menu =>
                        {
                            area = AreaManager.areaList[index];
                            ((OverworldScene)Current).Area = area;
                            area.ModifyProgress(0, true);
                        }), false);
                    }

                    actionMenu.SetActive(false);
                    areaMenu.SetActive(true);

                    areaMenu.ClearOptions(true);
                    GameManager.ChangeScene(SceneFactory.Overworld);
                }),
                new Button("Save", menu =>
                {
                    Scene.fileSelect.ClearOptions(false);
                    int slot = 0;
                    for (int i = 0; i < SaveLoad.MAX_SAVE_SLOTS; i++)
                    {
                        var index = i;
                        string info = " [Empty]";
                        if (SaveLoad.SaveExists(i + 1, HeroManager.PLAYER_SAVE_KEY))
                        {
                            var stats = SaveLoad.Load<Actor.StatsStruct>(index + 1, HeroManager.PLAYER_SAVE_KEY);
                            info = $" [{stats.Name}: LV {stats.Level}]";
                        }

                        Scene.fileSelect.Add(new Button($"Slot {i + 1}" + info, menu =>
                        {
                            slot = index + 1;
                        }), false);
                    }

                    actionMenu.SetActive(false);
                    Current.ToggleInfoLabels(false);
                    Scene.fileSelect.SetActive(true);
                    
                    GameManager.SaveGame(slot);
                    Scene.fileSelect.ClearOptions(true);
                    Current.ToggleInfoLabels(true);
                }),
                new Button("Quit", menu =>
                {
                    GameManager.ChangeScene(SceneFactory.TitleScreen);
                }),
            }, true, false, 3);


        static Menu actionMenu = ActionMenuTemplate.ShallowCopy();

        static Menu skillMenu = SkillMenuTemplate.ShallowCopy();

        static Menu targetMenu = new Menu("Select a hero",
            new()
            {

            });
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="OverworldScene"/> class.
        /// </summary>
        /// <param name="area">The area to start with</param>
        /// <param name="startAction">The action to trigger when switched to</param>
        /// <param name="exitAction">The action to trigger when switched away from</param>
        /// <param name="inElements">The GUI elements owned by this Scene</param>
        public OverworldScene(Area area, Action? startAction = null, Action? exitAction = null, GUIComponent[]? inElements = null) 
            : base(startAction, exitAction, inElements ?? Array.Empty<GUIComponent>())
        {
            Area = area;

            AddGUIElement(actionMenu, false);
            AddGUIElement(targetMenu, false);
            AddGUIElement(skillMenu, false);
            AddGUIElement(areaMenu, false);
            AddGUIElement(skillInfoLabel, false);
        }

        public override void Start()
        {
            base.Start();
            RunPauseLogic();
        }

        static void RunPauseLogic()
        {
            while (Program.GameIsRunning)
            {
#if DEBUG
                //if (Console.KeyAvailable)
                {
                    var ki = Console.ReadKey();
                    if (int.TryParse(ki.KeyChar.ToString(), out int num))
                    {
                        ki = Console.ReadKey();
                        if (ki.Key == ConsoleKey.OemPlus || ki.Key == ConsoleKey.Add)
                            Area.Current.ModifyProgress(num);
                        else if (ki.Key == ConsoleKey.OemMinus || ki.Key == ConsoleKey.Subtract)
                            Area.Current.ModifyProgress(-num);
                        else
                            Area.Current.ModifyProgress(num, true);
                    }
                }
#endif
                actionMenu.SetActive(true);
            }
        }

        static Hero? RunTargetMenu(Menu? previous, bool includeInactive = true)
        {
            if (previous != null)
                CanGoBack = true;

            Hero target = HeroManager.Party[0];

            targetMenu.ClearOptions(false);
            foreach (var hero in HeroManager.Party)
            {
                if (!includeInactive && hero.Hp <= 0)
                    continue;

                targetMenu.Add(new Button(hero.Name + $" | HP: {hero.Hp}/{hero.MaxHp} | SP: {hero.Sp}/{hero.MaxSp}", (menu) =>
                {
                    target = hero;
                }), false);
            }

            GameManager.ActiveInteractiveElement?.SetActive(false);
            targetMenu.SetActive(true);

            if (goBack)
            {
                targetMenu.SetActive(false);

                if (previous == skillMenu)
                    RunSkillMenu();
                else
                    previous?.SetActive(true);

                return null;
            }

            return target;
        }

        private static void DisplayStats(Hero curHero)
        {
            Current.ToggleInfoLabels(true);

            string effectsText = "";

            for (int i = 0; i < curHero.statusEffects.Count; i++)
            {
                if (effectsText != "")
                    effectsText += ", ";
                effectsText += curHero.statusEffects[i]?.Name;
            }

            if (effectsText != "")
                effectsText = "\nCondition: " + effectsText;

            print(
$@"    {curHero.Name}
Level {curHero.Level}
Element: {curHero.Element.Name}
".Pastel(ConsoleColor.Cyan), true, 0);

            print(effectsText.Pastel(ConsoleColor.Magenta), true, 0);

            print(
$@"
{$"HP: {curHero.Hp}/{curHero.MaxHp}".Pastel(curHero.Hp == 0 ? Color.Red : Color.LightGreen)}
{$"SP: {curHero.Sp}/{curHero.MaxSp}".Pastel(curHero.Sp == 0 ? Color.Red : Color.LightBlue)}

Attack: {curHero.Attack}
Defense: {curHero.Defense}
Special: {curHero.Special}
Speed: {curHero.Speed}

{$"Exp to Next Level: {curHero.Exp}/{curHero.NeededExp}".Pastel(Color.Gold)}",
            true, -1);
            Console.ResetColor();

            Current.ToggleInfoLabels(false);
        }

        static bool RunSkillMenu()
        {
            CanGoBack = true;

            skillMenu.ClearOptions(false);
            foreach (var skill in curHero.skills)
            {
                if (skill.actionType != SkillBase.ActionType.HEALING
                    && skill.actionType != SkillBase.ActionType.CURING && skill.actionType != SkillBase.ActionType.REVIVAL)
                    continue;

                skillMenu.Add(new Button(skill.Name, (menu) =>
                {
                    skillMenu.SetActive(false);
                    Current.ToggleInfoLabels(true);

                    if (skill.targetType == SkillBase.TargetGroup.ONE_ALLY)
                    {
                        var target = RunTargetMenu(skillMenu);
                        if (target != null)
                            (skill as Skill<Actor, Actor>).Use(curHero, target);
                    }
                    else if (skill.targetType == SkillBase.TargetGroup.ALL_ALLIES)
                    {
                        (skill as Skill<Actor, List<Actor>>).Use(curHero, HeroManager.Party.ToList<Actor>());
                    }


                }), false);
            }

            Console.SetCursorPosition(0, skillMenu.GetBottomCursorPosistion());
            TryUpdateSkillDescription(skillMenu);//Sets bottom text to the correct value immediately
            skillInfoLabel.SetActive(true);
            skillMenu.SetActive(true);

            if (goBack)
            {
                skillMenu.SetActive(false);
                return false;
            }

            Current.ToggleInfoLabels(false);
            return true;
        }
    }
}
