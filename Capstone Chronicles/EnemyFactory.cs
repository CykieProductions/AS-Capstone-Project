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
                        SkillManager.Vine_Whip,
                        SkillManager.Vine_Whip,//Duped for higher chances
                        SkillManager.Waste_Jumping,
                    }); }
        public static Enemy Caprid { get =>
                new("Caprid", ElementManager.PLANT, new Actor.StatsStruct(level: 1, maxHp: 8, maxSp: 30,
                    attack: 2, defense: 150, special: 2, speed: 0, exp: 15), new()
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
                        SkillManager.Water_Pulse,
                        SkillManager.Attack,
                        SkillManager.Waste_Stare,
                    }); }
        public static Enemy ElderSkoka { get =>
                new("Elder Skoka", ElementManager.NORMAL, new Actor.StatsStruct(level: 5, maxHp: 100, maxSp: 120,
                    attack: 3, defense: 0, special: 2, speed: 1, exp: 32), new()
                    {
                        SkillManager.Leaf_Storm,
                        SkillManager.Leaf_Storm,
                        SkillManager.Water_Pulse,
                        SkillManager.Water_Pulse,
                        SkillManager.Water_Pulse,
                        SkillManager.Attack,
                        SkillManager.Waste_Stare
                    }); }

        public static Enemy Flarix { get =>
                new("Flarix", ElementManager.FIRE, new Actor.StatsStruct(level: 2, maxHp: 40, maxSp: 12,
                    attack: 8, defense: 0, special: 5, speed: 1, exp: 20), new()
                    {
                        SkillManager.L1H1_Fireball,
                        SkillManager.Waste_Stare,
                    }); }
        public static Enemy Plugry { get =>
                new("Plugry", ElementManager.ELECTRIC, new Actor.StatsStruct(level: 2, maxHp: 30, maxSp: 20,
                    attack: 6, defense: 0, special: 8, speed: 2, exp: 28), new()
                    {
                        SkillManager.L1H1_Charge_Bolt,
                        SkillManager.Waste_Short_Circut
                    }); }
        public static Enemy Epho { get =>
                new("Epho", ElementManager.AIR, new Actor.StatsStruct(level: 10, maxHp: 140, maxSp: 20,
                    attack: 14, defense: 5, special: 18, speed: 8, exp: 50), new()
                    {
                        SkillManager.L1Ha_Electro_Wave,
                        SkillManager.Air_Cannon,
                        SkillManager.Air_Cannon,
                        SkillManager.Waste_Stare
                    }); }
        public static Enemy Terradon { get =>
                new("Terradon", ElementManager.GROUND, new Actor.StatsStruct(level: 10, maxHp: 160, maxSp: 20,
                    attack: 28, defense: 25, special: 12, speed: 2, exp: 66), new()
                    {
                        SkillManager.Pebble_Blast,
                        SkillManager.Pebble_Blast,
                        SkillManager.Pebble_Blast,
                        SkillManager.Rock_Slide,
                        SkillManager.Geo_Shift,
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

                        SkillManager.Hydo_Cannon.Use(user, targets[RNG.RandomInt(0, targets.Count - 1)]);
                        user.nextAction = SkillManager.Waste_Roar;
                    }).TryInflict(user, showInflictText: false, performImmediately: true);

                });

                Enemy leviac = new("Leviac", ElementManager.WATER, new Actor.StatsStruct(level: 10, maxHp: 360, maxSp: 80,
                    attack: 5, defense: 1, special: 9, speed: 1, exp: 78), new()
                    {
                        SkillManager.Geo_Shift,
                        SkillManager.Bite,
                        SkillManager.Bite,
                        SkillManager.Water_Pulse,
                        SkillManager.Water_Pulse,
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
        
        #endregion
    }
}
