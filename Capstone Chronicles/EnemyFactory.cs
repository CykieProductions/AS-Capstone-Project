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
                new Enemy("Test", ElementManager.AIR, new Actor.StatsStruct(level: 1, maxHp: 10, maxSp: 20, 
                    attack: 2, defense: 0, spAttack: 1, spDefense: 0, speed: 2, exp: 40)); }
    }
}
