using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    //TODO Implement class
    internal class EnemyFactory
    {
        public static Enemy Test { get =>
                new("Test", ElementManager.NORMAL, new Actor.StatsStruct(level: 1, maxHp: 10, maxSp: 20,
                    attack: 2, defense: 0, special: 1, speed: 2, exp: 40), null); }

        public static Enemy Growfa { get =>
                new("Growfa", ElementManager.PLANT, new Actor.StatsStruct(level: 1, maxHp: 12, maxSp: 30,
                    attack: 3, defense: 0, special: 3, speed: 1, exp: 4), new()
                    {
                        SkillManager.L1H1_Vine_Whip,
                        SkillManager.L1H1_Vine_Whip,//Duped for higher chances
                        SkillManager.Waste_Jumping,
                    }); }
        public static Enemy Caprid { get =>
                new("Caprid", ElementManager.PLANT, new Actor.StatsStruct(level: 1, maxHp: 8, maxSp: 30,
                    attack: 2, defense: 150, special: 2, speed: 0, exp: 10), new()
                    {
                        SkillManager.Poison_Powder,
                        SkillManager.Bite,
                        SkillManager.Bite,
                        SkillManager.Waste_Stare,
                    }); }
        public static Enemy Skoka { get =>
                new("Skoka", ElementManager.NORMAL, new Actor.StatsStruct(level: 1, maxHp: 16, maxSp: 30,
                    attack: 4, defense: 0, special: 1, speed: 3, exp: 8), new()
                    {
                        SkillManager.L1H1_Water_Pulse,
                        SkillManager.Attack,
                        SkillManager.Waste_Stare,
                    }); }
        public static Enemy ElderSkoka { get =>
                new("Elder Skoka", ElementManager.NORMAL, new Actor.StatsStruct(level: 5, maxHp: 100, maxSp: 120,
                    attack: 3, defense: 0, special: 2, speed: 1, exp: 32), new()
                    {
                        SkillManager.L1Ha_Leaf_Storm,
                        SkillManager.L1Ha_Leaf_Storm,
                        SkillManager.L1H1_Water_Pulse,
                        SkillManager.L1H1_Water_Pulse,
                        SkillManager.L1H1_Water_Pulse,
                        SkillManager.Attack,
                        SkillManager.Waste_Stare
                    }); }

        public static Enemy Flarix { get =>
                new("Flarix", ElementManager.FIRE, new Actor.StatsStruct(level: 2, maxHp: 55, maxSp: 12,
                    attack: 8, defense: 2, special: 5, speed: 7, exp: 20), new()
                    {
                        SkillManager.L1H1_Fireball,
                        SkillManager.Waste_Stare,
                    }); }
        public static Enemy Plugry { get =>
                new("Plugry", ElementManager.ELECTRIC, new Actor.StatsStruct(level: 2, maxHp: 44, maxSp: 20,
                    attack: 6, defense: 1, special: 8, speed: 5, exp: 28), new()
                    {
                        SkillManager.L1H1_Charge_Bolt,
                        SkillManager.Waste_Short_Circut
                    }); }
        public static Enemy Epho { get =>
                new("Epho", ElementManager.AIR, new Actor.StatsStruct(level: 10, maxHp: 140, maxSp: 20,
                    attack: 14, defense: 5, special: 12, speed: 8, exp: 50), new()
                    {
                        SkillManager.L1Ha_Electro_Wave,
                        SkillManager.L2H1_Air_Cannon,
                        SkillManager.L2H1_Air_Cannon,
                        SkillManager.Waste_Stare
                    }); }
        public static Enemy Terradon { get =>
                new("Terradon", ElementManager.GROUND, new Actor.StatsStruct(level: 10, maxHp: 160, maxSp: 20,
                    attack: 28, defense: 25, special: 12, speed: 2, exp: 66), new()
                    {
                        SkillManager.L1H1_Pebble_Blast,
                        SkillManager.L1H1_Pebble_Blast,
                        SkillManager.L1H1_Pebble_Blast,
                        SkillManager.L1Ha_Rock_Slide,
                        SkillManager.L2H1_Geo_Shift,
                        SkillManager.Bite,
                        SkillManager.Bite,
                        SkillManager.Bite,
                        SkillManager.Waste_Roar
                    }); }

        #region BOSSES
        public static Enemy Boss_Leviac
        {
            get
            {
                //Custom moves
                var diveAction = new Skill<Actor>("Dive", (user) =>
                {
                    Scene.print($"{user.Name} dove underwater");
                    for (int i = 0; i < user.statusEffects.Count; i++)
                    {
                        user.statusEffects[i]?.TryRemoveEffect(user, true);
                    }

                    new StatusEffect("SUBMERGED", 3, 4, (x) =>
                    {
                        //New element that acts like WATER, except it dodges everything except ELECTRIC
                        user.Element = new Element(ElementManager.WATER, "X WATER", new Element[]
                        {
                            ElementManager.NORMAL,
                            ElementManager.FIRE,
                            ElementManager.WATER,
                            ElementManager.PLANT,
                            ElementManager.GROUND,
                            ElementManager.AIR
                        });

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Scene.print($"{user.Name} is submerged");
                        Console.ForegroundColor = ConsoleColor.White;
                        user.nextAction = new Skill<Actor>("Preparing", (y) => Scene.print($"{user.Name} is preparing something"), pri: 2);

                        if (RNG.Chance(2f / 3))
                            user.ModifyStamina((int)(user.MaxSp * 0.25f));
                    }, "DON'T SHOW THIS", "{0} resurfaced",
                    (x) =>
                    {
                        user.Element = ElementManager.WATER;

                        List<Actor> targets = new(HeroManager.Party);
                        targets.RemoveAll(x => x.Hp <= 0);

                        SkillManager.L3H1_Hydo_Cannon.Use(user, targets[RNG.RandomInt(0, targets.Count - 1)]);
                        user.nextAction = SkillManager.Waste_Roar;
                    }).TryInflict(user, showInflictText: false, performImmediately: true);

                });

                Enemy leviac = new("Leviac", ElementManager.WATER, new Actor.StatsStruct(level: 10, maxHp: 390, maxSp: 80,
                    attack: 5, defense: 1, special: 9, speed: 1, exp: 108), new()
                    {
                        SkillManager.L2H1_Geo_Shift,
                        SkillManager.Bite,
                        SkillManager.Bite,
                        SkillManager.L1H1_Water_Pulse,
                        SkillManager.L1H1_Water_Pulse,
                        SkillManager.Poison_Cloud,
                        SkillManager.Waste_Roar,
                        diveAction,
                        diveAction,
                    });

                return new(leviac, ai: () =>
                    {
                        var skillPool = leviac.skills.ToList();//copy of the list

                        //Program.curHeroes;//test
                        foreach (var skill in leviac.skills)
                        {
                            if (skill == SkillManager.Attack && RNG.Chance(1f / 1f))//Don't normal attack
                                skillPool.Remove(SkillManager.Attack);
                            else if (skill.Cost > leviac.Sp && RNG.Chance(1f / 4))//Leaves a small chance to use a skill that costs too much
                                skillPool.Remove(skill);
                        }

                        var randInt = RNG.RandomInt(0, skillPool.Count - 1);

                        return skillPool[randInt];
                    });
            }
        }
        
        public static Enemy Boss_Hology
        {
            get
            {
                var focusAction = new Skill<Actor>("Focus", (user) =>
                {
                    Scene.print($"{user.Name} is focused");

                    if (RNG.Chance(2f / 3))
                    {
                        user.ModifyStamina(30);
                    }
                    else
                    {
                        user.nextAction = SkillManager.L3Ha_Gigawatt_Dischage;
                    }
                });

                Enemy hology = new("Hology", ElementManager.NORMAL, new Actor.StatsStruct(level: 20, maxHp: 1500, maxSp: 100,
                    attack: 24, defense: 50, special: 14, speed: 6, exp: 10000), new()
                    {
                        SkillManager.L2Ha_Spire_Wall,
                        SkillManager.L2Ha_Sky_Crusher,
                        SkillManager.L2H1_Flame_Burst,
                        SkillManager.L3H1_Needle_Tomb,
                        SkillManager.Confuse_Ray,
                        focusAction,
                    });

                return new(hology, ai: () =>
                    {
                        var skillPool = hology.skills.ToList();//copy of the list

                        //Program.curHeroes;//test
                        foreach (var skill in hology.skills)
                        {
                            if (skill.Cost > hology.Sp && RNG.Chance(1f / 4))//Leaves a small chance to use a skill that costs too much
                                skillPool.Remove(skill);

                            if (HeroManager.Player.Hp < HeroManager.Player.MaxHp / 10
                                && skill.targetType != SkillBase.TargetGroup.ALL_OPPONENTS)
                            {
                                skillPool.Remove(skill);
                            }
                                
                        }

                        var randInt = RNG.RandomInt(0, skillPool.Count - 1);

                        return skillPool[randInt];
                    });
            }
        }
        
        #endregion
    }
}
