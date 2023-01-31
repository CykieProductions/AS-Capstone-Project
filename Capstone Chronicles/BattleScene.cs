using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Capstone_Chronicles.Program;
using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    internal class BattleScene : Scene
    {
        static int totalExp = 0;

        static Hero curHero = HeroManager.Party[0];
        static List<Enemy> enemies = new();
        static bool openSkillMenu = false;

        //GUI
        static Menu actionMenu = new Menu(
            new Label(dynamicText: () =>
                {
                    actionMenu.Prompt.Text = "What will {0} do?\nHP: {1}/{2} | SP: {3}/{4}";
                    actionMenu.Prompt.SetText(actionMenu.Prompt.Text, 
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
            });

        static Menu skillMenu = new Menu("Select a skill",
            new()
            {

            });

        static Menu targetMenu = new Menu("Select a target",
            new()
            {

            });
        //

        public BattleScene(params Enemy[] inEnemies) 
            : base(Array.Empty<GUIComponent>())
        {
            enemies = inEnemies.ToList();

            //! Rename duplicate enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (i == enemies.Count - 1)//stop if this is the last/only enemy
                    break;
                for (int j = i + 1; j < enemies.Count; j++)
                {
                    if (enemies[j].Name == enemies[i].Name.Replace(" (1)", ""))//Duplicate names where found
                    {
                        if (!enemies[i].Name.Contains(" ("))//If no suffix on previous, then mark it as the first enemy with this name
                            enemies[i].Name += $" (1)";
                        enemies[j].Name += $" ({j + 1})";

                        //Now number this enemy appropriately
                        //var split = enemies[i].Name.Split(' ');
                        //enemies[j].Name = enemies[i].Name.Replace(split[split.Length - 1], $"({j + 1})");
                    }
                }
            }

            AddGUIElement(actionMenu, false);
            AddGUIElement(targetMenu, false);
            AddGUIElement(skillMenu, false);
        }

        public override void Start()
        {
            base.Start();
            RunBattle();
        }

        private void RunBattle()
        {
            while (RunRound());
        }

        /// <summary>
        /// Runs battle logic for one round
        /// </summary>
        /// <returns> True if the battle is still going; False if the battle is over </returns>
        private bool RunRound()
        {
            var heroes = new List<Hero>();

            //! HEROES DECIDE
            for (int i = 0; i < HeroManager.Party.Count; i++)
            {
                //Ensure only active heroes
                if (HeroManager.Party[i].Hp > 0)
                    heroes.Add(HeroManager.Party[i]);
                else
                    continue;

                curHero = HeroManager.Party[i];
                actionMenu.SetActive(true);

                if (openSkillMenu)
                {
                    openSkillMenu = false;

                    skillMenu.ClearOptions(false);
                    foreach (var skill in curHero.skills)
                    {
                        skillMenu.Add(new Button(skill.Name, (menu) =>
                        {
                            curHero.nextAction = skill;
                        }), false);
                    }
                    skillMenu.SetActive(true);
                }

                //! Choose Target
                targetMenu.ClearOptions(false);
                curHero.targets.Clear();

                if (curHero.nextAction.targetType == SkillBase.TargetGroup.TARGET_SINGLE_ALLY)
                {
                    foreach (var h in HeroManager.Party)
                    {
                        targetMenu.Add(new Button(h.Name, (menu) =>
                        {
                            curHero.targets.Add(h);
                        }), false);
                    }
                    targetMenu.SetActive(true);
                }
                else if (curHero.nextAction.targetType == SkillBase.TargetGroup.TARGET_SINGLE_OPPONENT)
                {
                    foreach (var e in enemies)
                    {
                        targetMenu.Add(new Button(e.Name, (menu) =>
                        {
                            curHero.targets.Add(e);
                        }), false);
                    }
                    targetMenu.SetActive(true);
                }
            }

            //TODO ENEMIES DECIDE
            foreach (var enemy in enemies)
            {
                enemy.targets.Clear();
                enemy.nextAction = SkillManager.Poison_Powder;
                enemy.targets.Add(heroes[0]);
            }

            //! DECIDE TURN ORDER
            var allActors = new List<Actor>(heroes);
            allActors.AddRange(enemies);

            allActors = allActors.OrderByDescending(x => x.nextAction?.Priority).ThenByDescending(x => x.Speed).ToList();

            //! PLAY TURN
            void _RunStatusEffectLogic(in Actor curActor)
            {
                for (int i = 0; i < curActor.statusEffects.Count; i++)
                {
                    curActor.statusEffects[i]?.PerformAction(curActor);
                    curActor.statusEffects[i]?.TryRemoveEffect(curActor);
                }
            }

            foreach (var curActor in allActors)
            {
                if (curActor.Hp <= 0)
                    continue;

                List<Actor> targets = curActor.targets;
                var activeEnemies = new List<Enemy>(enemies);

                if (curActor.nextAction == null)
                {
                    SkillManager.Skip_Turn.Use(curActor);
                    _RunStatusEffectLogic(curActor);
                    continue;
                }

                //Self or no target
                if (curActor.nextAction.targetType == SkillBase.TargetGroup.NO_TARGET)
                {
                    (curActor.nextAction as Skill<Actor>).Use(curActor);
                }
                //Single Target
                else if (targets.Count != 0 && (curActor.nextAction.targetType == SkillBase.TargetGroup.TARGET_SINGLE_OPPONENT ||
                    curActor.nextAction.targetType == SkillBase.TargetGroup.TARGET_SINGLE_ALLY))
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
                        else if (targets[0] is Hero)//target the first hero over
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
                    if (curActor.nextAction.targetType == SkillBase.TargetGroup.TARGET_EVERYONE)
                    {
                        //Don't remove from allActors within this loop
                        List<Actor> everyone = new(allActors);
                        everyone.RemoveAll((a) => a.Hp <= 0);

                        (curActor.nextAction as Skill<Actor, List<Actor>>).Use(curActor, everyone);
                    }
                    //Anyone
                    else if (curActor.nextAction.targetType == SkillBase.TargetGroup.TARGET_ANYONE)
                    {
                        List<Actor> everyone = new(allActors);
                        everyone.RemoveAll((a) => a.Hp <= 0);

                        targets.Add(everyone[RNG.RandomInt(0, everyone.Count - 1)]);
                        (curActor.nextAction as Skill<Actor, Actor>).Use(curActor, curActor.targets[0]);
                    }
                    //All Opponents
                    else if (curActor.nextAction.targetType == SkillBase.TargetGroup.TARGET_ALL_OPPONENTS)
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
                    else if (curActor.nextAction.targetType == SkillBase.TargetGroup.TARGET_ALL_ALLIES)
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

                //Don't run the status effect if the battle will end
                activeEnemies.RemoveAll(x => x.Hp == 0);
                heroes.RemoveAll(x => x.Hp == 0);
                if (activeEnemies.Count != 0)
                    _RunStatusEffectLogic(curActor);
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

            if (enemies.Count == 0 || heroes.Count == 0)
                return false;
            return true;
        }
    }
}
