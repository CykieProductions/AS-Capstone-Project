using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    public partial class Actor
    {
        /// <summary> Used to initialize Actors in a cleaner looking way </summary>
        public struct StatsStruct
        {
            public int Level { get; private set; } = 1;
            public int MaxHp { get; private set; } = 10;
            public int Hp { get; private set; } = 10;
            public int MaxSp { get; private set; } = 20;
            public int Sp { get; private set; } = 20;

            public int Attack { get; private set; } = 2;
            public int Defense { get; private set; } = 0;
            public int SpAttack { get; private set; } = 1;
            public int SpDefense { get; private set; } = 0;
            public int Speed { get; private set; } = 1;
            public int Exp { get; private set; } = 5;

            //Fully custom constructor
            public StatsStruct(int level, int maxHp, int hp, int maxSp, int sp,
                int attack, int defense, int spAttack, int spDefense, int speed, int exp)
            {
                Level = level;
                MaxHp = maxHp;
                Hp = hp;
                MaxSp = maxSp;
                Sp = sp;
                Attack = attack;
                Defense = defense;
                SpAttack = spAttack;
                SpDefense = spDefense;
                Speed = speed;
                Exp = exp;
            }
            //Simplified constructor
            public StatsStruct(int level, int maxHp, int maxSp,
                int attack, int defense, int spAttack, int spDefense, int speed, int exp = 0)
            {
                Level = level;
                MaxHp = maxHp;
                Hp = MaxHp;
                MaxSp = maxSp;
                Sp = MaxSp;
                Attack = attack;
                Defense = defense;
                SpAttack = spAttack;
                SpDefense = spDefense;
                Speed = speed;
                Exp = exp;
            }
        }
    }
}
