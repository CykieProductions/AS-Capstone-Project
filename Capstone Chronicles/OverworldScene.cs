using Capstone_Chronicles.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    public class OverworldScene : Scene
    {


        static Hero curHero = HeroManager.Party[0];

        #region GUI References
        static Label bottomText = new("");

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
            }),
            new()
            {
                new Button("Proceed", menu =>
                {
                    GameManager.ChangeScene(SceneFactory.GetBattleScene());
                }),
                new Button("Skills", menu =>
                {
                    curHero = RunTargetMenu();
                    if (curHero.skills.Count > 0)
                        RunSkillMenu();
                    else
                    {
                        menu.closeAfterConfirm = false;
                    }
                }),
                new Button("Save", menu =>
                {
                    curHero.nextAction = SkillManager.Guard;
                }),
                new Button("Quit", menu =>
                {
                    GameManager.ChangeScene(SceneFactory.TitleScreen);
                }),
            });
        static Menu actionMenu = ActionMenuTemplate.ShallowCopy();

        static Menu skillMenu = SkillMenuTemplate.ShallowCopy();

        static Menu targetMenu = new Menu("Select a hero",
            new()
            {

            });
        #endregion

        public OverworldScene(Action? startAction, Action? exitAction, GUIComponent[] inElements) 
            : base(startAction, exitAction, inElements)
        {
            AddGUIElement(actionMenu, false);
            AddGUIElement(targetMenu, false);
            AddGUIElement(skillMenu, false);
            AddGUIElement(skillInfoLabel, false);
        }

        public override void Start()
        {
            base.Start();
            RunPauseLogic();
        }

        //todo decide
        static void RunNavagationLogic()
        {
            while (Program.GameIsRunning)
            {
                if (Input.GetKeyDown(Input.PauseKey))
                {
                    //actionMenu.SetActive(false, false);
                    RunPauseLogic();
                }
            }
        }

        static void RunPauseLogic()
        {
            while (Program.GameIsRunning)
            {
                /*if (Input.GetKeyDown(Input.PauseKey))
                {
                    actionMenu.SetActive(false, false);
                    RunNavagationLogic();
                }*/

                actionMenu.SetActive(true);
            }
        }

        static Hero RunTargetMenu()
        {
            Hero target = HeroManager.Party[0];

            targetMenu.ClearOptions(false);
            foreach (var hero in HeroManager.Party)
            {
                targetMenu.Add(new Button(hero.Name + $" | HP: {hero.Hp}/{hero.MaxHp} | SP: {hero.Sp}/{hero.MaxSp}", (menu) =>
                {
                    target = hero;
                }), false);
            }

            GameManager.ActiveInteractiveElement?.SetActive(false);
            targetMenu.SetActive(true);

            return target;
        }

        static void RunSkillMenu()
        {
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
                        (skill as Skill<Actor, Actor>).Use(curHero, RunTargetMenu());
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
            Current.ToggleInfoLabels(false);
        }

    }
}
