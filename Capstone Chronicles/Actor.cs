using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Actors participate in battles. 
    /// </summary>
    public partial class Actor
    {
        //Contains nested struct StatsStruct
        public string Name { get; set; } = "";

        /*? Why are the stats here:
         * I originally used a Stats struct for this, but that meant either making every setter 
         * public, or moving leveling logic into the struct, which would defeat the purpose.
         * Only Actors need access to setting stats anyway, so I just moved the stats here.
         */
        #region Basic Stats
        public int Level { get; protected set; }
        public int MaxHp { get; protected set; }
        public int Hp { get; protected set; }
        public int MaxSp { get; protected set; }
        public int Sp { get; protected set; }

        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public int SpAttack { get; protected set; }
        public int SpDefense { get; protected set; }
        public int Speed { get; protected set; }
        public int Exp { get; protected set; }
        #endregion

        public Actor(string inName, StatsStruct inStats)
        {
            Name = inName;

            Level = inStats.Level;
            MaxHp = inStats.MaxHp;
            Hp = inStats.Hp;
            MaxSp = inStats.MaxSp;
            Sp = inStats.Sp;
            Attack = inStats.Attack;
            Defense = inStats.Defense;
            SpAttack = inStats.SpAttack;
            SpDefense = inStats.SpDefense;
            Speed = inStats.Speed;
            Exp = inStats.Exp;
        }

        public void ModifyHP(int amount)
        {
            Hp += amount;

            if (Hp < 0)
                Hp = 0;
            else if (Hp > MaxHp)
                Hp = MaxHp;
        }
    }

    /// <summary>
    /// An actor that fights on the player's team
    /// </summary>
    public partial class Hero : Actor
    {
        //Contains nested class StatGrowth
        public Hero(string inName, StatsStruct inStats, StatGrowth growthInfo) : base(inName, inStats)
        {
            //TODO implement StatGrowth
        }
    }

    /// <summary>
    /// An actor that fights against the player
    /// </summary>
    public class Enemy : Actor
    {
        public Enemy(string inName, StatsStruct inStats) : base(inName, inStats)
        {
        }
    }
}
