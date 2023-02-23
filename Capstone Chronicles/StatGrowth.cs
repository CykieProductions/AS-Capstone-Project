using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Chronicles
{
    public partial class Actor
    {
        /// <summary>
        /// Contains info on how stats should grow with the actor's level
        /// </summary>
        public class StatGrowth
        {
            private Actor self;

            public StatGrowth()
            {
                Hero.AfterInit += Init;
            }

            //Dependency injection
            private void Init(StatGrowth @this, Actor inActor)
            {
                self = inActor;
                Hero.AfterInit -= Init;
            }

            public void AttackGain(bool showText = true)
            {
                int amount;
                amount = (int)( Logarithmic(self.Attack, 1.25f) + (RNG.RandomInt(-20, 20) / 10f) ).Clamp(0, 20);

                self.Attack += amount;

                if (showText && amount > 0)
                    Scene.print($"{self.Name}'s attack increased by {amount}");
            }

            int Logarithmic(int stat, float scalar)
            {
                return (int)(MathF.Log(stat) * scalar);
            }
        }
    }
}
