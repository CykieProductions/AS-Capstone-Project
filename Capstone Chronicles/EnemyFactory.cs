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
                new Enemy("Test", new Actor.StatsStruct(1, 10, 20, 2, 0, 1, 0, 2, 4)); }
    }
}
