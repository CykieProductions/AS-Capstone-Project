using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    public partial class Actor
    {
        public enum StatType
        {
            Max_HP, Max_SP, Attack, Defense, Special, Speed, Exp
        }

        /// <summary> Used to initialize Actors in a cleaner looking way </summary>
        public struct StatsStruct
        {
            public int Level { get; set; } = 1;
            public int MaxHp { get; set; } = 10;
            public int Hp { get; set; } = 10;
            public int MaxSp { get; set; } = 20;
            public int Sp { get; set; } = 20;

            public int Attack { get; set; } = 2;
            public int Defense { get; set; } = 0;
            public int Special { get; set; } = 1;
            public int Speed { get; set; } = 1;
            public int Exp { get; set; } = 5;

            //Fully custom constructor
            public StatsStruct(int level, int maxHp, int hp, int maxSp, int sp,
                int attack, int defense, int special, int speed, int exp)
            {
                Level = level;
                MaxHp = maxHp;
                Hp = hp;
                MaxSp = maxSp;
                Sp = sp;
                Attack = attack;
                Defense = defense;
                Special = special;
                Speed = speed;
                Exp = exp;
            }
            //Simplified constructor
            public StatsStruct(int level, int maxHp, int maxSp,
                int attack, int defense, int special, int speed, int exp = 0)
            {
                Level = level;
                MaxHp = maxHp;
                Hp = MaxHp;
                MaxSp = maxSp;
                Sp = MaxSp;
                Attack = attack;
                Defense = defense;
                Special = special;
                Speed = speed;
                Exp = exp;
            }
            //From Actor constructor
            public StatsStruct(Actor actor, int lv = 0)
            {
                Level =     lv <= 0 ? actor.Level : lv;
                MaxHp =     actor.MaxHp;
                Hp =        actor.Hp;
                MaxSp =     actor.MaxSp;
                Sp =        actor.Sp;
                Attack =    actor.Attack;
                Defense =   actor.Defense;
                Special =   actor.Special;
                Speed =     actor.Speed;
                Exp =       actor.Exp;
            }

        }
    }
}
