using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//TODO remove this
//using static Capstone_Chronicles.Program;/*
using static Capstone_Chronicles.Scene;//*/

namespace Capstone_Chronicles
{
    public class SkillManager
    {
        public static Action<string> callForSkill;
        public static SkillBase requestedSkill;

        #region All Skills
        public static Skill<Actor> Skip_Turn { get; private set; }
        public static Skill<Actor> Sleep { get; private set; }
        public static Skill<Actor> Immobile { get; private set; }
        public static Skill<Actor> Waste_Stare { get; private set; }
        public static Skill<Actor> Waste_Roar { get; private set; }
        public static Skill<Actor> Waste_Short_Circut { get; private set; }
        public static Skill<Actor> Waste_Jumping { get; private set; }


        public static Skill<Actor, Actor> Poison_Powder { get; private set; }
        public static Skill<Actor, List<Actor>> Poison_Cloud { get; private set; }

        //Testing
        public static Skill<Actor, List<Actor>> Damage_Allies_Test { get; private set; }
        public static Skill<Actor, List<Actor>> Poison_Allies_Test { get; private set; }
        public static Skill<Actor, List<Actor>> Ignite_Allies_Test { get; private set; }
        public static Skill<Actor, List<Actor>> Restore_SP_Allies_Test { get; private set; }
        //

        public static Skill<Actor, Actor> Attack { get; private set; }
        public static Skill<Actor> Guard { get; private set; }
        public static Skill<Actor, Actor> Scan_Lash { get; private set; }

        //Fire moves
        /// <summary>Level 1 | Hit one</summary>
        public static Skill<Actor, Actor> Fireball { get; private set; }
        /// <summary>Level 2 | Hit one</summary>
        public static Skill<Actor, Actor> Flame_Burst { get; private set; }
        /// <summary>Level 3 | Hit one</summary>
        public static Skill<Actor, Actor> Eruption { get; private set; }
        /// <summary>Level 1 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Flare_Fall { get; private set; }
        /// <summary>Level 2 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Blazing_Vortex { get; private set; }
        /// <summary>Level 3 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Supernova { get; private set; }

        //Water moves
        /// <summary>Level 1 | Hit one</summary>
        public static Skill<Actor, Actor> Water_Pulse { get; private set; }
        /// <summary>Level 2 | Hit one</summary>
        public static Skill<Actor, Actor> Water_Jet { get; private set; }
        /// <summary>Level 3 | Hit one</summary>
        public static Skill<Actor, Actor> Hydo_Cannon { get; private set; }
        //public static Skill<Actor, Actor> Torpedo { get; private set; }//Physical
        /// <summary>Level 1 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Waterfall { get; private set; }
        /// <summary>Level 2 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Deluge { get; private set; }
        /// <summary>Level 3 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Tsunami { get; private set; }

        //Plant moves
        /// <summary>Level 1 | Hit one</summary>
        public static Skill<Actor, Actor> Vine_Whip { get; private set; }
        /// <summary>Level 2 | Hit one</summary>
        public static Skill<Actor, Actor> Leaf_Cutter { get; private set; }
        /// <summary>Level 3 | Hit one</summary>
        public static Skill<Actor, Actor> Needle_Tomb { get; private set; }
        /// <summary>Level 1 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Leaf_Storm { get; private set; }
        /// <summary>Level 2 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Root_Wave { get; private set; }
        /// <summary>Level 3 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Thorn_Canopy { get; private set; }

        //Ground moves
        /// <summary>Level 1 | Hit one</summary>
        public static Skill<Actor, Actor> Pebble_Blast { get; private set; }
        /// <summary>Level 2 | Hit one</summary>
        public static Skill<Actor, Actor> Geo_Shift { get; private set; }
        /// <summary>Level 3 | Hit one</summary>
        public static Skill<Actor, Actor> Fissure { get; private set; }
        /// <summary>Level 1 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Rock_Slide { get; private set; }
        /// <summary>Level 2 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Spire_Wall { get; private set; }
        /// <summary>Level 3 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Earthquake { get; private set; }

        //Air moves
        /// <summary>Level 1 | Hit one</summary>
        public static Skill<Actor, Actor> Wind_Slash { get; private set; }
        /// <summary>Level 2 | Hit one</summary>
        public static Skill<Actor, Actor> Air_Cannon { get; private set; }
        /// <summary>Level 3 | Hit one</summary>
        public static Skill<Actor, Actor> Sonic_Boom { get; private set; }
        /// <summary>Level 1 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Slash_Storm { get; private set; }
        /// <summary>Level 2 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Sky_Crusher { get; private set; }
        /// <summary>Level 3 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Hurricane { get; private set; }

        //Electric moves
        /// <summary>Level 1 | Hit one</summary>
        public static Skill<Actor, Actor> Charge_Bolt { get; private set; }
        /// <summary>Level 2 | Hit one</summary>
        public static Skill<Actor, Actor> Taser_Grip { get; private set; }
        /// <summary>Level 3 | Hit one</summary>
        public static Skill<Actor, Actor> Ion_Overload { get; private set; }
        /// <summary>Level 1 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Electro_Wave { get; private set; }
        /// <summary>Level 2 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Tesla_Cannon { get; private set; }
        /// <summary>Level 3 | Hit all</summary>
        public static Skill<Actor, List<Actor>> Gigawatt_Dischage { get; private set; }

        //Healing
        public static Skill<Actor, Actor> Healing_Powder { get; private set; }
        public static Skill<Actor, Actor> Super_Healing_Powder { get; private set; }
        public static Skill<Actor, Actor> Ultra_Healing_Powder { get; private set; }
        public static Skill<Actor, List<Actor>> Healing_Cloud { get; private set; }
        public static Skill<Actor, List<Actor>> Ultra_Healing_Cloud { get; private set; }

        public static Skill<Actor, Actor> Curing_Powder { get; private set; }
        public static Skill<Actor, List<Actor>> Curing_Cloud { get; private set; }
        public static Skill<Actor, Actor> Phoenix_Powder { get; private set; }
        public static Skill<Actor, Actor> Ultra_Phoenix_Powder { get; private set; }
        public static Skill<Actor, List<Actor>> Pheonix_Cloud { get; private set; }
        #endregion


        #region Construction Helper Functions
        public static int SpecialAttackFormula(Actor user, int rank = 1, int baseValue = 32, float scalar = 0.36f, bool varyDamage = true)
        {
            float rankDamper = 0.75f;
            if (rank >= 3)
                rankDamper = 0.64f;

            int amount = (int)(baseValue * Math.Sqrt(Math.Pow(user.SpAttack, (rank * rankDamper).Clamp(1, 50))) * scalar);
            if (varyDamage)
                amount += RNG.RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
            return amount;
        }
        public static int BasicAttackFormula(Actor user, int rank = 1, float fluxPercent = 0.1f, int fluxMin = 2)
        {
            float rankDamper = 1.4f;

            //print((rank * rankDamper).Clamp(1, 50));
            int damage = (int)((user.Attack * Math.Sqrt(rank).Clamp(1, float.MaxValue)) + (1.25f * Math.Sqrt(user.Level * 0.75f).Clamp(1, 100)) * Math.Pow(rank, rankDamper).Clamp(1, 50));
            damage += RNG.RandomInt(-(int)(damage * fluxPercent).Clamp(fluxMin, float.MaxValue), (int)(damage * fluxPercent).Clamp(fluxMin, float.MaxValue));

            if (rank >= 3)
                damage += (int)(user.Attack * Math.Sqrt(rank).Clamp(1, float.MaxValue));

            return damage;
        }

        public static int SpecialHealingFormula(Actor user, int rank = 1, int baseValue = 30, float scalar = 1f)
        {
            float rankDamper = 1f;
            if (rank >= 3)
                rankDamper = 1.2f;

            return (int)(baseValue * Math.Sqrt(rank * rankDamper).Clamp(1, 50) * scalar * (rank * 0.75f).Clamp(1, 50));
        }

        public static void OutOfSP(string n)
        {
            print($"{n} didn't have enough SP");
        }
        #endregion//

        //Static constructor
        static SkillManager()
        {

            Scan_Lash = new Skill<Actor, Actor>("Scan Lash", (user, target) =>
            {
                print($"{user.Name} used {Scan_Lash.Name} on {target.Name}");
                //user.Attack(target);
                target.ModifyHealth(-2, Scan_Lash);

                Console.ForegroundColor = ConsoleColor.Cyan;
                print('-'.Repeat(36));
                print($"Scan results for {target.Name}:");
                print($"LV: {target.Level}");
                print($"ELMT: {target.Element.NameFromEnum}");

                target.statusEffects.RemoveAll(x => x == null);
                if (target.statusEffects.Count > 0)
                {
                    print($"STATUS: ", true);
                    for (int i = 0; i < target.statusEffects.Count; i++)
                    {
                        if (i > 0)
                            print(", ", true);
                        print(target.statusEffects[i].Name, true);
                    }
                    print("");
                }

                print($"HP: {target.Hp}/{target.MaxHp}");
                print($"SP: {target.Sp}/{target.MaxSp}");
                //print("------------------------------------");//36
                print('-'.Repeat(36));
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey(true);
            }
            , 0, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.OMNI);

            Guard = new Skill<Actor>("Guard", (user) =>
            {
                //this move should ignore speed and happen before anything
                //it stops when the next turn order is decided
                print(user.Name + " is guarding");
                user.isGuarding = true;
            }, pri: 2);

            #region TURN WASTING
            Skip_Turn = new Skill<Actor>("Do Nothing", (user) =>
            {
                if (user.Hp > 0)
                    print($"{user.Name} did nothing");
            });

            Sleep = new Skill<Actor>("Sleep", (user) =>
            {
                print($"{user.Name} is fast asleep");
                user.ModifyConra(RNG.RandomInt(1, 3));
            });

            Immobile = new Skill<Actor>("Can't Move", (user) =>
            {
                print($"{user.Name} couldn't move");
            });

            Waste_Stare = new Skill<Actor>("Stare", (user) =>
            {
                print($"{user.Name} stared you down");
            });

            Waste_Roar = new Skill<Actor>("Roar", (user) =>
            {
                print($"{user.Name} let out an intense roar");
            });

            Waste_Short_Circut = new Skill<Actor>("Short Circuit", (user) =>
            {
                print($"{user.Name} short circuited");
            });

            Waste_Jumping = new Skill<Actor>("Jump Around", (user) =>
            {
                print($"{user.Name} is jumping around");
            });
            #endregion

            #region TESTING
            Damage_Allies_Test = new Skill<Actor, List<Actor>>("Damage Allies", (user, targets) =>
            {
                if (user.Sp >= Damage_Allies_Test.Cost)
                {
                    print($"{user.Name} used {Damage_Allies_Test.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 1);

                    foreach (var target in targets)
                    {
                        amount += RNG.RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(-amount);
                    }

                    user.Sp -= Damage_Allies_Test.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 0, SkillBase.TargetGroup.ALL_ALLIES, skillType: SkillBase.ActionType.HEALING);
            Poison_Allies_Test = new Skill<Actor, List<Actor>>("Poison Allies", (user, targets) =>
            {
                var curSkill = Poison_Allies_Test;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = RNG.RandomInt(1, 3);
                        target.ModifyHealth(-amount, curSkill);

                        if (RNG.Chance(1f))
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            StatusEffectManager.POISONED.TryInflict(target);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 0, SkillBase.TargetGroup.ALL_ALLIES, ElementManager.OMNI, SkillBase.ActionType.HEALING);
            Ignite_Allies_Test = new Skill<Actor, List<Actor>>("Ignite Allies", (user, targets) =>
            {
                var curSkill = Ignite_Allies_Test;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = RNG.RandomInt(1, 3);
                        target.ModifyHealth(-amount, curSkill);

                        if (RNG.Chance(1f))
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            StatusEffectManager.FLAMING.TryInflict(target);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 0, SkillBase.TargetGroup.ALL_ALLIES, ElementManager.OMNI, SkillBase.ActionType.HEALING);
            Restore_SP_Allies_Test = new Skill<Actor, List<Actor>>("Restore SP of Allies", (user, targets) =>
            {
                var curSkill = Restore_SP_Allies_Test;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        target.ModifyConra(target.MaxSp);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 0, SkillBase.TargetGroup.ALL_ALLIES, ElementManager.OMNI, SkillBase.ActionType.HEALING);
            #endregion

            #region STATUS EFFECT GIVING
            Poison_Powder = new Skill<Actor, Actor>("Poison Powder", (user, target) =>
            {
                var curSkill = Poison_Powder;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} produced {curSkill.Name}");

                    int amount = RNG.RandomInt(1, 3);
                    int actual = target.ModifyHealth(-amount, curSkill);

                    if (actual < 0 && RNG.Chance(4f / 5))//Use must actually do damage
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        StatusEffectManager.POISONED.TryInflict(target);
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 5, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.PLANT, SkillBase.ActionType.INFLICTING);

            Poison_Cloud = new Skill<Actor, List<Actor>>("Poison Cloud", (user, targets) =>
            {
                var curSkill = Poison_Cloud;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} produced {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = RNG.RandomInt(1, 3);
                        int actual = target.ModifyHealth(-amount, curSkill);

                        if (actual < 0 && RNG.Chance(2f / 3))
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            StatusEffectManager.POISONED.TryInflict(target);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 14, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.PLANT, SkillBase.ActionType.INFLICTING);
            #endregion

            #region ATTACKS
            Attack = new Skill<Actor, Actor>("Attack", (user, target) =>
            {
                print($"{user.Name} attacked {target.Name}");
                //user.Attack(target);
                target.ModifyHealth(-BasicAttackFormula(user, 1), Attack);
            }
            , 0, SkillBase.TargetGroup.ONE_OPPONENT);

            #region FIRE
            Fireball = new Skill<Actor, Actor>("Fireball", (user, target) =>
            {
                var curSkill = Fireball;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} shot out a fireball");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user);
                    amount = target.ModifyHealth(-amount, curSkill);

                    if (amount < 0 && RNG.Chance(1f / 6))
                        StatusEffectManager.FLAMING.TryInflict(target);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 4, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.FIRE);

            Flame_Burst = new Skill<Actor, Actor>("Flame Burst", (user, target) =>
            {
                var curSkill = Flame_Burst;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 2);
                    amount = target.ModifyHealth(-amount, curSkill);

                    if (amount < 0 && RNG.Chance(1f / 6))
                        StatusEffectManager.FLAMING.TryInflict(target);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.FIRE);

            Eruption = new Skill<Actor, Actor>("Eruption", (user, target) =>
            {
                var curSkill = Eruption;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 3);
                    amount = target.ModifyHealth(-amount, curSkill);

                    if (amount < 0 && RNG.Chance(1f / 3))
                        StatusEffectManager.FLAMING.TryInflict(target);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 24, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.FIRE);

            Flare_Fall = new Skill<Actor, List<Actor>>("Flare Fall", (user, targets) =>
            {
                var curSkill = Flare_Fall;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 1);
                        amount = target.ModifyHealth(-amount, curSkill);

                        if (amount < 0 && RNG.Chance(1f / 8))
                            StatusEffectManager.FLAMING.TryInflict(target);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.FIRE);

            Blazing_Vortex = new Skill<Actor, List<Actor>>("Blazing Vortex", (user, targets) =>
            {
                var curSkill = Blazing_Vortex;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 2);
                        amount = target.ModifyHealth(-amount, curSkill);

                        if (amount < 0 && RNG.Chance(1f / 4))
                            StatusEffectManager.FLAMING.TryInflict(target);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 16, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.FIRE);

            Supernova = new Skill<Actor, List<Actor>>("Supernova", (user, targets) =>
            {
                var curSkill = Supernova;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 3);
                        amount = target.ModifyHealth(-amount, curSkill);

                        if (amount < 0 && RNG.Chance(1f / 2))
                            StatusEffectManager.FLAMING.TryInflict(target);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 50, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.FIRE);
            #endregion

            #region WATER
            Water_Pulse = new Skill<Actor, Actor>("Water Pulse", (user, target) =>
            {
                var curSkill = Water_Pulse;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    target.ModifyHealth(-SpecialAttackFormula(user, 1), curSkill);
                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 4, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.WATER);

            Water_Jet = new Skill<Actor, Actor>("Water Jet", (user, target) =>
            {
                var curSkill = Water_Jet;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    target.ModifyHealth(-SpecialAttackFormula(user, 2), curSkill);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.WATER);

            Hydo_Cannon = new Skill<Actor, Actor>("Hydro Cannon", (user, target) =>
            {
                var curSkill = Hydo_Cannon;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = (int)(SpecialAttackFormula(user, 3) * 0.75f);
                    amount = target.ModifyHealth(-amount, curSkill);

                    if (!target.isGuarding && amount < 0 && RNG.Chance(1f / 3))
                        StatusEffectManager.CONFUSED.TryInflict(target);


                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 28, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.WATER);

            Waterfall = new Skill<Actor, List<Actor>>("Waterfall", (user, targets) =>
            {
                var curSkill = Waterfall;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 1);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 9, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.WATER);

            Deluge = new Skill<Actor, List<Actor>>("Deluge", (user, targets) =>
            {
                var curSkill = Deluge;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 2);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 16, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.WATER);

            Tsunami = new Skill<Actor, List<Actor>>("Tsunami", (user, targets) =>
            {
                var curSkill = Tsunami;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = (int)(SpecialAttackFormula(user, 3) * 0.75f);
                        amount = target.ModifyHealth(-amount, curSkill);

                        if (amount < 0 && RNG.Chance(1f / 3))
                            StatusEffectManager.CONFUSED.TryInflict(target);

                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 45, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.WATER);
            #endregion

            #region PLANT
            Vine_Whip = new Skill<Actor, Actor>("Vine Whip", (user, target) =>
            {
                var curSkill = Vine_Whip;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    target.ModifyHealth(-BasicAttackFormula(user, 2), curSkill);
                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 3, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.PLANT);

            Leaf_Cutter = new Skill<Actor, Actor>("Leaf Cutter", (user, target) =>
            {
                var curSkill = Leaf_Cutter;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    int amount = (int)(SpecialAttackFormula(user, 1) * 0.5 + BasicAttackFormula(user, 2));
                    target.ModifyHealth(-amount, curSkill);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 9, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.PLANT);

            Needle_Tomb = new Skill<Actor, Actor>("Needle Tomb", (user, target) =>
            {
                var curSkill = Needle_Tomb;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = (int)(SpecialAttackFormula(user, 1) * 0.75 + BasicAttackFormula(user, 3));
                    amount = target.ModifyHealth(-amount, curSkill);

                    if (amount < 0 && RNG.Chance(1f / 5))
                        StatusEffectManager.PARALYZED.TryInflict(target);


                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 18, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.PLANT);

            Leaf_Storm = new Skill<Actor, List<Actor>>("Leaf Storm", (user, targets) =>
            {
                var curSkill = Leaf_Storm;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.1f + BasicAttackFormula(user, 2));
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 9, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.PLANT);

            Root_Wave = new Skill<Actor, List<Actor>>("Root Wave", (user, targets) =>
            {
                var curSkill = Root_Wave;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.5 + BasicAttackFormula(user, 2));
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 16, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.PLANT);

            Thorn_Canopy = new Skill<Actor, List<Actor>>("Thorn Canopy", (user, targets) =>
            {
                var curSkill = Thorn_Canopy;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = (int)(SpecialAttackFormula(user, 1) * 0.75 + BasicAttackFormula(user, 3));
                        amount = target.ModifyHealth(-amount, curSkill);

                        if (amount < 0 && RNG.Chance(1f / 5))
                            StatusEffectManager.PARALYZED.TryInflict(target);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 38, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.PLANT);
            #endregion

            #region GROUND
            Pebble_Blast = new Skill<Actor, Actor>("Pebble Blast", (user, target) =>
            {
                var curSkill = Pebble_Blast;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}", true);

                    int total = 0;
                    for (int i = 0; i < 6; i++)//can hit 6 times
                    {
                        int amount = (int)(BasicAttackFormula(user, 1) / 3f).Clamp(2, int.MaxValue);
                        if (i <= 1)//2 hits guaranteed
                        {
                            total += amount;
                            target.ModifyHealth(-amount, curSkill);
                        }
                        else if (RNG.Chance(1f / 2))
                        {
                            total += amount;
                            target.ModifyHealth(-amount, curSkill);
                        }
                        Thread.Sleep(10);//For randomization
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    print($"{total} total damage on {target.Name}!");
                    Console.ForegroundColor = ConsoleColor.White;

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 4, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.GROUND);

            Geo_Shift = new Skill<Actor, Actor>("Geo Shift", (user, target) =>
            {
                var curSkill = Geo_Shift;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    int amount = BasicAttackFormula(user, 3) + (int)(SpecialAttackFormula(user, 1) * 0.25f);
                    target.ModifyHealth(-amount, curSkill);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.GROUND);

            Fissure = new Skill<Actor, Actor>("Fissure", (user, target) =>
            {
                var curSkill = Fissure;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    int amount = BasicAttackFormula(user, 4) + (int)(SpecialAttackFormula(user, 1) * 0.75f);
                    target.ModifyHealth(-amount, curSkill);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 24, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.GROUND);

            Rock_Slide = new Skill<Actor, List<Actor>>("Rock Slide", (user, targets) =>
            {
                var curSkill = Rock_Slide;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int total = 0;
                        for (int i = 0; i < 4; i++)//can hit 4 times
                        {
                            int amount = (int)(BasicAttackFormula(user, 1) / 2f).Clamp(2, int.MaxValue);
                            if (i <= 1)//2 hits guaranteed
                            {
                                total += amount;
                                target.ModifyHealth(-amount, curSkill);
                            }
                            else if (RNG.Chance(1f / 2))
                            {
                                total += amount;
                                target.ModifyHealth(-amount, curSkill);
                            }
                            Thread.Sleep(10);//For randomization
                        }
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        print($"{total} total damage on {target.Name}!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.GROUND);

            Spire_Wall = new Skill<Actor, List<Actor>>("Spire Wall", (user, targets) =>
            {
                var curSkill = Spire_Wall;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = BasicAttackFormula(user, 3) + (int)(SpecialAttackFormula(user, 1) * 0.25f);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 16, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.GROUND);

            Earthquake = new Skill<Actor, List<Actor>>("Earthquake", (user, targets) =>
            {
                var curSkill = Earthquake;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = BasicAttackFormula(user, 4) + (int)(SpecialAttackFormula(user, 1) * 0.75f);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 50, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.GROUND);
            #endregion

            #region AIR
            Wind_Slash = new Skill<Actor, Actor>("Wind Slash", (user, target) =>
            {
                var curSkill = Wind_Slash;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    target.ModifyHealth(-SpecialAttackFormula(user, 1), curSkill);
                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 4, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.AIR);

            Air_Cannon = new Skill<Actor, Actor>("Air Cannon", (user, target) =>
            {
                var curSkill = Air_Cannon;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    target.ModifyHealth(-SpecialAttackFormula(user, 2), curSkill);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.AIR);

            Sonic_Boom = new Skill<Actor, Actor>("Sonic Boom", (user, target) =>
            {
                var curSkill = Sonic_Boom;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = (SpecialAttackFormula(user, 3));

                    target.ModifyHealth(-amount, curSkill);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 28, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.AIR);

            Slash_Storm = new Skill<Actor, List<Actor>>("Slash Storm", (user, targets) =>
            {
                var curSkill = Slash_Storm;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 1);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 7, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.AIR);

            Sky_Crusher = new Skill<Actor, List<Actor>>("Sky Crusher", (user, targets) =>
            {
                var curSkill = Sky_Crusher;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 2);
                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 14, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.AIR);

            Hurricane = new Skill<Actor, List<Actor>>("Hurricane", (user, targets) =>
            {
                var curSkill = Hurricane;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 3);

                        target.ModifyHealth(-amount, curSkill);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 38, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.AIR);
            #endregion

            #region ELECTRIC
            Charge_Bolt = new Skill<Actor, Actor>("Charge Bolt", (user, target) =>
            {
                var curSkill = Charge_Bolt;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user);
                    amount = target.ModifyHealth(-amount, curSkill);

                    if (amount < 0 && RNG.Chance(1f / 6))
                        StatusEffectManager.PARALYZED.TryInflict(target);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 4, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.ELECTRIC);

            Taser_Grip = new Skill<Actor, Actor>("Taser Grip", (user, target) =>
            {
                var curSkill = Taser_Grip;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 2);
                    amount = target.ModifyHealth(-amount, curSkill);

                    if (amount < 0 && RNG.Chance(1f / 4))
                        StatusEffectManager.PARALYZED.TryInflict(target);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.ELECTRIC);

            Ion_Overload = new Skill<Actor, Actor>("Ion Overload", (user, target) =>
            {
                var curSkill = Ion_Overload;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    int amount = SpecialAttackFormula(user, 3);
                    amount = target.ModifyHealth(-amount, curSkill);

                    if (amount < 0 && RNG.Chance(1f / 3))
                        StatusEffectManager.PARALYZED.TryInflict(target);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 28, SkillBase.TargetGroup.ONE_OPPONENT, ElementManager.ELECTRIC);

            Electro_Wave = new Skill<Actor, List<Actor>>("Electro Wave", (user, targets) =>
            {
                var curSkill = Electro_Wave;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);

                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 1);
                        amount = target.ModifyHealth(-amount, curSkill);

                        if (amount < 0 && RNG.Chance(1f / 6))
                            StatusEffectManager.PARALYZED.TryInflict(target);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.ELECTRIC);

            Tesla_Cannon = new Skill<Actor, List<Actor>>("Tesla Cannon", (user, targets) =>
            {
                var curSkill = Tesla_Cannon;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 2);
                        amount = target.ModifyHealth(-amount, curSkill);

                        if (amount < 0 && RNG.Chance(1f / 4))
                            StatusEffectManager.PARALYZED.TryInflict(target);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 16, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.ELECTRIC);

            Gigawatt_Dischage = new Skill<Actor, List<Actor>>("Gigawatt_Dischage", (user, targets) =>
            {
                var curSkill = Gigawatt_Dischage;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} used {curSkill.Name}");

                    //int amount = (int)(50 * (user.specialAttack / 15f)).Clamp(12, int.MaxValue);
                    foreach (var target in targets)
                    {
                        int amount = SpecialAttackFormula(user, 3);
                        amount = target.ModifyHealth(-amount, curSkill);

                        if (amount < 0 && RNG.Chance(1f / 2))
                            StatusEffectManager.PARALYZED.TryInflict(target);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 50, SkillBase.TargetGroup.ALL_OPPONENTS, ElementManager.ELECTRIC);
            #endregion

            #endregion

            #region HEALING AND CURING
            Healing_Powder = new Skill<Actor, Actor>("Healing Powder", (user, target) =>
            {
                if (user.Sp >= Healing_Powder.Cost)
                {
                    print($"{user.Name} produced {Healing_Powder.Name}");

                    int amount = SpecialHealingFormula(user);
                    amount += RNG.RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                    target.ModifyHealth(amount);
                    user.Sp -= Healing_Powder.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 3, SkillBase.TargetGroup.ONE_ALLY, skillType: SkillBase.ActionType.HEALING);

            Super_Healing_Powder = new Skill<Actor, Actor>("Super Healing Powder", (user, target) =>
            {
                if (user.Sp >= Super_Healing_Powder.Cost)
                {
                    print($"{user.Name} produced {Super_Healing_Powder.Name}");

                    int amount = SpecialHealingFormula(user, 2);
                    amount += RNG.RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                    target.ModifyHealth(amount);
                    user.Sp -= Super_Healing_Powder.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 8, SkillBase.TargetGroup.ONE_ALLY, skillType: SkillBase.ActionType.HEALING);

            //Full heal
            Ultra_Healing_Powder = new Skill<Actor, Actor>("Ultra Healing Powder", (user, target) =>
            {
                if (user.Sp >= Ultra_Healing_Powder.Cost)
                {
                    print($"{user.Name} produced {Ultra_Healing_Powder.Name}");

                    /*int amount = SpecialHealingFormula(user, 4);
                    amount += RNG.RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f) );
                    target.ModifyHealth(amount);*/
                    target.ModifyHealth(target.MaxHp);
                    user.Sp -= Ultra_Healing_Powder.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 24, SkillBase.TargetGroup.ONE_ALLY, skillType: SkillBase.ActionType.HEALING);

            Healing_Cloud = new Skill<Actor, List<Actor>>("Healing Cloud", (user, targets) =>
            {
                if (user.Sp >= Healing_Cloud.Cost)
                {
                    print($"{user.Name} produced a {Healing_Cloud.Name}");

                    int amount = SpecialHealingFormula(user, 2);

                    foreach (var target in targets)
                    {
                        amount += RNG.RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(amount);
                    }

                    user.Sp -= Healing_Cloud.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 18, SkillBase.TargetGroup.ALL_ALLIES, skillType: SkillBase.ActionType.HEALING);

            Ultra_Healing_Cloud = new Skill<Actor, List<Actor>>("Ultra Healing Cloud", (user, targets) =>
            {
                if (user.Sp >= Ultra_Healing_Cloud.Cost)
                {
                    print($"{user.Name} produced an {Ultra_Healing_Cloud.Name}");

                    int amount = SpecialHealingFormula(user, 4);

                    foreach (var target in targets)
                    {
                        amount += RNG.RandomInt(-(int)(amount * 0.1f), (int)(amount * 0.1f));
                        target.ModifyHealth(amount);
                    }

                    user.Sp -= Ultra_Healing_Cloud.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 50, SkillBase.TargetGroup.ALL_ALLIES, skillType: SkillBase.ActionType.HEALING);


            Curing_Powder = new Skill<Actor, Actor>("Curing Powder", (user, target) =>
            {
                var curSkill = Curing_Powder;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} produced {curSkill.Name}");

                    target.statusEffects.RemoveAll((m) => m == null);
                    if (target.statusEffects.Count > 0)
                    {
                        for (int i = 0; i < target.statusEffects.Count; i++)
                        {
                            target.statusEffects[i].TryRemoveEffect(target, true);
                        }
                    }
                    else
                        print("It had no visible effect on " + target.Name);

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 5, SkillBase.TargetGroup.ONE_ALLY, skillType: SkillBase.ActionType.CURING);

            Curing_Cloud = new Skill<Actor, List<Actor>>("Curing Cloud", (user, targets) =>
            {
                var curSkill = Curing_Cloud;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} produced a {curSkill.Name}");
                    foreach (var target in targets)
                    {
                        target.statusEffects.RemoveAll((m) => m == null);
                        if (target.statusEffects.Count > 0)
                        {
                            for (int i = 0; i < target.statusEffects.Count; i++)
                            {
                                target.statusEffects[i].TryRemoveEffect(target, true);
                            }
                        }
                        else
                            print("It had no visible effect on " + target.Name);
                    }

                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 22, SkillBase.TargetGroup.ALL_ALLIES, skillType: SkillBase.ActionType.CURING);


            Phoenix_Powder = new Skill<Actor, Actor>("Phoenix Powder", (user, target) =>
            {
                var curSkill = Phoenix_Powder;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} produced {curSkill.Name}");

                    target.ModifyHealth((int)(target.MaxHp / 2f), curSkill);
                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 20, SkillBase.TargetGroup.ONE_ALLY, skillType: SkillBase.ActionType.REVIVAL);

            //Revive to full health 
            Ultra_Phoenix_Powder = new Skill<Actor, Actor>("Ultra Phoenix Powder", (user, target) =>
            {
                var curSkill = Ultra_Phoenix_Powder;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} produced {curSkill.Name}");

                    target.ModifyHealth((target.MaxHp), curSkill);
                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 30, SkillBase.TargetGroup.ONE_ALLY, skillType: SkillBase.ActionType.REVIVAL);

            Pheonix_Cloud = new Skill<Actor, List<Actor>>("Phoenix Cloud", (user, targets) =>
            {
                var curSkill = Pheonix_Cloud;
                if (user.Sp >= curSkill.Cost)
                {
                    print($"{user.Name} produced {curSkill.Name}");

                    foreach (var target in targets)
                    {
                        target.ModifyHealth((int)(target.MaxHp / 2f), curSkill);
                    }
                    user.Sp -= curSkill.Cost;
                }
                else
                    OutOfSP(user.Name);
            }
            , 42, SkillBase.TargetGroup.ALL_ALLIES, skillType: SkillBase.ActionType.REVIVAL);

            #endregion

        }

        public static SkillBase? GetSkillByName(string? name)
        {
            if (name == null)
                return null;

            callForSkill.Invoke(name);
            var skill = requestedSkill;

            if (skill == null)//use reflections as back up
            {
                name = name.Replace(' ', '_');
                var field = typeof(SkillManager).GetField(name, BindingFlags.Public | BindingFlags.Static);

                if (field.GetValue(null) is SkillBase)
                {
                    //print((field.GetValue(null) as SkillBase).Name);
                    return field.GetValue(null) as SkillBase;
                }
                return null;
            }
            else
            {
                requestedSkill = null;
                return skill;
            }
        }
    }
}
