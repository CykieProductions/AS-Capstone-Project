using System;
using System.Collections.Generic;
using System.Linq;
using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    public class BattleScene : Scene
    {
        public enum RoundResult
        {
            IN_COMBAT, WIN, LOSE, FLED
        }

        private int totalExp = 0;
        private bool canBeEscaped = true;

        static Hero? curHero = HeroManager.Party[0];

        static List<Enemy> enemies = new();
        public static Enemy? DecidingEnemy { get; private set; }

        static bool openSkillMenu = false;

        #region GUI References
        static readonly Menu ActionMenuTemplate = new Menu(
            new Label(dynamicText: () =>
                {
                    var format = "What will {0} do?\nHP: {1}/{2} | SP: {3}/{4}";
                    actionMenu.Prompt.SetDynamicText(format, 
                        curHero.Name, curHero.Hp, curHero.MaxHp, curHero.Sp, curHero.MaxSp);
                }),
            new()
            {
                new Button("Attack", menu =>
                {
                    curHero.nextAction = SkillManager.Attack;
                }),
                new Button("Skills", menu =>
                {
                    if (curHero.skills.Count > 0)
                        openSkillMenu = true;
                    else
                    {
                        openSkillMenu = false;
                        menu.closeAfterConfirm = false;
                    }
                }),
                new Button("Guard", menu =>
                {
                    curHero.nextAction = SkillManager.Guard;
                }),
                new Button("Flee", menu =>
                {
                    actionMenu.SetActive(false);

                    Current.ToggleInfoLabels(true);

                    print("You attempted to run", delayAfter: .25f);
                    print(".", true, delayAfter: .25f);
                    print(".", true, delayAfter: .25f);
                    print(".", true, delayAfter: .75f);

                    if (RNG.OneInThree)
                    {
                        print(" But your path was blocked");
                        curHero.nextAction = SkillManager.Skip_Turn;
                    }
                    else
                    {
                        print(" And succeeded!");
                        curHero = null;
                    }

                    GameManager.CurrentScene.ToggleInfoLabels(false);
                }),
            });
        static Menu actionMenu = ActionMenuTemplate.ShallowCopy();

        
        static Menu skillMenu = SkillMenuTemplate.ShallowCopy();

        static readonly Menu TargetMenuTemplate = new Menu("Select a target",
            new()
            {

            }, wrap: true, remember: true, inRowLimit: 4);
        static Menu targetMenu = TargetMenuTemplate.ShallowCopy();

        #endregion


        public BattleScene(Encounter encounter) 
            : base(Array.Empty<GUIComponent>())
        {
            enemies = encounter.GetEnemies();
            canBeEscaped = encounter.CanBeEscaped;

            //! Rename duplicate enemies
            HashSet<string> usedNames = new();
            for (int u = 0; u < enemies.Count; u++)
            {
                if (enemies.Count <= 1)
                    break;

                //Once the name is found once, every enemy with that name is handled
                if (usedNames.Contains(enemies[u].Name.Split(" (")[0]))
                    continue;

                //A new name was found, now check if more enemies have it
                usedNames.Add(enemies[u].Name);
                var dupes = enemies.FindAll(x => x.Name == enemies[u].Name);

                if (dupes.Count <= 1) continue;

                for (int i = 0; i < dupes.Count; i++)
                {
                    dupes[i].Name += $" ({i + 1})";
                }
            }

            AddGUIElement(actionMenu, false);
            AddGUIElement(targetMenu, false);
            AddGUIElement(skillMenu, false);
            AddGUIElement(skillInfoLabel, false);
        }

        public override void Start()
        {
            base.Start();
            RunBattle();
            GameManager.ChangeScene(Previous);
        }


        private void RunBattle()
        {
            RoundResult result;
            while (RunRound(out result));

            //! END OF BATTLE
            if (result == RoundResult.LOSE)
            {
                GameManager.GameOver();
                return;
            }
            else if (result == RoundResult.FLED)
                return;

            Area.Current.ModifyProgress(1);
            foreach (var hero in HeroManager.Party)
            {
                if (hero.Hp == 0)
                    continue;

                hero.Exp += totalExp;
                hero.TryLevelUp();
            }
        }

        /// <summary>
        /// Runs battle logic for one round
        /// </summary>
        /// <returns> True if the battle is still going; False if the battle is over </returns>
        private bool RunRound(out RoundResult roundResult)
        {
            roundResult = RoundResult.IN_COMBAT;
            Current.ToggleInfoLabels(false);
            var heroes = new List<Hero>(HeroManager.Party);

            //! HEROES DECIDE
            for (int i = 0; i < HeroManager.Party.Count; i++)
            {
                //Ensure only active heroes act
                if (HeroManager.Party[i].Hp <= 0)
                {
                    heroes.Remove(HeroManager.Party[i]);
                    continue;
                }

            HeroDecisionStart:
                actionMenu.SetOptions(ActionMenuTemplate);

                curHero = HeroManager.Party[i];

                if (curHero != heroes[0] || !canBeEscaped)
                {
                    actionMenu.Remove("Flee", false);
                    CanGoBack = true;
                }
                else
                    CanGoBack = false;

                if (curHero.skills.Count == 0)
                    actionMenu.Remove("Skills", false);

                actionMenu.SetActive(true);
                //Back to Previous Active Hero
                if (goBack)
                {
                    var og = i;
                    i--;
                    while (HeroManager.Party[i].Hp <= 0 || HeroManager.Party[i].nextAction == SkillManager.Skip_Turn)
                    {
                        i--;
                        if (i < 0)
                            i = og;
                    }

                    goto HeroDecisionStart;
                }

                if (curHero == null)//Player successfully fled
                {
                    roundResult = RoundResult.FLED;
                    return false;
                }

                bool hasOpenedSkillMenu = false;

            HeroSkillMenu:
                CanGoBack = true;

                if (openSkillMenu)
                {
                    openSkillMenu = false;
                    hasOpenedSkillMenu = true;

                    skillMenu.ClearOptions(false);
                    foreach (var skill in curHero.skills)
                    {
                        skillMenu.Add(new Button(skill.Name, (menu) =>
                        {
                            curHero.nextAction = skill;
                        }), false);
                    }

                    Console.SetCursorPosition(0, skillMenu.GetBottomCursorPosistion());
                    TryUpdateSkillDescription(skillMenu);//Sets bottom text to the correct value immediately
                    skillInfoLabel.SetActive(true);
                    skillMenu.SetActive(true);

                    //Back to Action Select
                    if (goBack)
                    {
                        skillMenu.SetActive(false);
                        skillInfoLabel.SetActive(false);
                        goto HeroDecisionStart;
                    }

                    skillInfoLabel.SetActive(false);
                }

                //! Hero Choose Target
                targetMenu.ClearOptions(false);
                curHero.targets.Clear();

                if (curHero.nextAction.targetType == SkillBase.TargetGroup.ONE_ALLY)
                {
                    foreach (var h in HeroManager.Party)
                    {
                        targetMenu.Add(new Button($"{h.Name} | HP: {h.Hp}/{h.MaxHp}", (menu) =>
                        {
                            curHero.targets.Add(h);
                        }), false);
                    }
                    targetMenu.SetActive(true);
                }
                else if (curHero.nextAction.targetType == SkillBase.TargetGroup.ONE_OPPONENT)
                {
                    foreach (var e in enemies)
                    {
                        targetMenu.Add(new Button($"{e.Name} | HP: {e.Hp}/{e.MaxHp}", (menu) =>
                        {
                            curHero.targets.Add(e);
                        }), false);
                    }
                    targetMenu.SetActive(true);
                }

                if (goBack)
                {
                    targetMenu.SetActive(false);
                    //Back to Skill Select
                    if (hasOpenedSkillMenu)
                    {
                        openSkillMenu = true;
                        goto HeroSkillMenu;
                    }
                    //Back to Action Select
                    goto HeroDecisionStart;
                }

            }

            //! ENEMIES DECIDE
            foreach (var enemy in enemies)
            {
                DecidingEnemy = enemy;

                //The enemy's next action may have already been decided due to special AI
                enemy.nextAction ??= enemy.decideTurnAction.Invoke();

                enemy.ChooseTarget(heroes.ToList<Actor>(), enemy.nextAction.targetType);
            }

            //! DECIDE TURN ORDER
            var allActors = new List<Actor>(heroes);
            allActors.AddRange(enemies);

            allActors = allActors.OrderByDescending(x => x.nextAction?.Priority).ThenByDescending(x => x.Speed).ToList();

            //! PLAY TURN
            Current.ToggleInfoLabels(true);
            foreach (var curActor in allActors)
            {
                if (curActor.Hp <= 0)
                    continue;

                List<Actor> targets = curActor.targets;
                var activeEnemies = new List<Enemy>(enemies);

                // This uses an out parameter for clarity
                void _EndOfTurnLogic(in Actor curActor, out bool endBattle)
                {
                    activeEnemies.RemoveAll(x => x.Hp == 0);
                    heroes.RemoveAll(x => x.Hp == 0);
                    if (activeEnemies.Count != 0 && heroes.Count != 0)//Continue battle
                    {
                        endBattle = false;
                    }
                    else //no actors left
                        endBattle = true;

                    for (int i = 0; i < curActor.statusEffects.Count; i++)
                        curActor.statusEffects[i]?.TryRemoveEffect(curActor);
                }

                //Trigger Effect before turn
                for (int i = 0; i < curActor.statusEffects.Count; i++)
                {
                    curActor.statusEffects[i]?.PerformAction(curActor);
                    if (curActor.Hp == 0)
                    {
                        if (curActor is Hero)
                            heroes.Remove((Hero)curActor);
                        if (curActor is Enemy)
                            activeEnemies.Remove((Enemy)curActor);

                        curActor.nextAction = null;
                    }
                    //Try to get rid of it at the end of the turn
                }

                //Auto Skip if null
                if (curActor.nextAction == null)
                {
                    _EndOfTurnLogic(curActor, out bool stop);

                    if (stop)
                        break;
                    continue;
                }

                //Self or no target
                if (curActor.nextAction.targetType == SkillBase.TargetGroup.SELF)
                {
                    (curActor.nextAction as Skill<Actor>).Use(curActor);
                }
                //Single Target
                else if (targets.Count != 0 && (curActor.nextAction.targetType == SkillBase.TargetGroup.ONE_OPPONENT ||
                    curActor.nextAction.targetType == SkillBase.TargetGroup.ONE_ALLY))
                {
                    //Re-target if target was already gone
                    if (targets[0].Hp <= 0 && curActor.nextAction.actionType != SkillBase.ActionType.REVIVAL)
                    {
                        if (targets[0] is Enemy)//target the first enemy instead
                        {
                            activeEnemies.RemoveAll(x => x.Hp == 0);

                            print(targets[0].Name + " isn't there. ", true);
                            if (activeEnemies.Count != 0)
                            {
                                targets[0] = activeEnemies[0];
                                print("Targeting " + targets[0].Name + " instead");
                            }
                            else
                            {
                                curActor.nextAction = SkillManager.Skip_Turn;
                                print("");
                            }
                        }
                        else if ((curActor is Hero) && heroes.Contains(targets[0]))//if a fellow hero was the target, then target self
                        {
                            targets[0] = curActor;
                        }
                        else if (targets[0] is Hero)//target the first hero
                        {
                            heroes.RemoveAll(x => x.Hp == 0);
                            print(targets[0].Name + " isn't there. ", true);
                            if (heroes.Count != 0)
                            {
                                targets[0] = heroes[0];
                                print("Targeting " + targets[0].Name + " instead");
                            }
                            else
                            {
                                curActor.nextAction = SkillManager.Skip_Turn;
                                print("");
                            }
                        }
                    }

                    //Use Single target skill
                    (curActor.nextAction as Skill<Actor, Actor>)?.Use(curActor, targets[0]);
                }
                //Multi-target (Specified)
                else if (targets.Count != 0)
                {
                    List<Actor> moddedTargetList = new(targets);
                    for (int t = 0; t < targets.Count; t++)
                    {
                        if ((targets[t].Hp <= 0 && curActor.nextAction.actionType != SkillBase.ActionType.REVIVAL) || (targets[t] is Enemy && !enemies.Contains(targets[t])))
                            moddedTargetList.Remove(targets[t]);
                    }
                    (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, moddedTargetList);
                }
                //Target(s) Not Specified
                else
                {
                    //Everyone
                    if (curActor.nextAction.targetType == SkillBase.TargetGroup.EVERYONE)
                    {
                        //Don't remove from allActors within this loop
                        List<Actor> everyone = new(allActors);
                        everyone.RemoveAll((a) => a.Hp <= 0);

                        (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, everyone);
                    }
                    //Anyone
                    else if (curActor.nextAction.targetType == SkillBase.TargetGroup.ANYONE)
                    {
                        List<Actor> everyone = new(allActors);
                        everyone.RemoveAll((a) => a.Hp <= 0);

                        targets.Add(everyone[RNG.RandomInt(0, everyone.Count - 1)]);
                        (curActor.nextAction as Skill<Actor, Actor>).Use(curActor, curActor.targets[0]);
                    }
                    //All Opponents
                    else if (curActor.nextAction.targetType == SkillBase.TargetGroup.ALL_OPPONENTS)
                    {
                        List<Actor> newTargets;
                        if (curActor is Enemy)
                            newTargets = heroes.ToList<Actor>();
                        else
                            newTargets = enemies.ToList<Actor>();

                        newTargets.RemoveAll((a) => a.Hp <= 0);
                        targets = newTargets;

                        (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, targets);
                    }
                    //All Allies
                    else if (curActor.nextAction.targetType == SkillBase.TargetGroup.ALL_ALLIES)
                    {
                        List<Actor> newTargets;
                        if (curActor is Enemy)
                            newTargets = enemies.ToList<Actor>();
                        else
                            newTargets = heroes.ToList<Actor>();

                        newTargets.RemoveAll((a) => a.Hp <= 0);
                        targets = newTargets;

                        (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, targets);
                    }
                    else
                    {
                        curActor.nextAction = SkillManager.Skip_Turn;
                        (curActor.nextAction as Skill<Actor>).Use(curActor);
                    }
                }

                curActor.nextAction = null;
                _EndOfTurnLogic(curActor, out bool endBattle);

                if (endBattle)
                    break;
                continue;
            }

            //! EXP
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Hp > 0)
                    continue;
                totalExp += enemies[i].Exp;
                enemies[i] = null;
            }
            enemies.RemoveAll(x => x == null);
            heroes.RemoveAll(x => x.Hp <= 0);
            //curHero = HeroManager.Party[1];

            if (enemies.Count == 0)
                roundResult = RoundResult.WIN;
            else if (heroes.Count == 0)
                roundResult = RoundResult.LOSE;

            if (roundResult != RoundResult.IN_COMBAT)
                return false;
            return true;
        }

    }
}
