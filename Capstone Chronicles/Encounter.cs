using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    public class Encounter
    {
        private List<Enemy> enemies;
        (int min, int max) amountRange = (0, 0);
        public bool CanBeEscaped { get; private set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enemies"></param>
        /// <param name="numOfEnemies"></param>
        /// <param name="canBeEscaped"></param>
        public Encounter(List<Enemy> enemies, (int min, int max)? numOfEnemies = null, bool canBeEscaped = true)
        {
            this.enemies = enemies;
            CanBeEscaped = canBeEscaped;

            if (numOfEnemies != null)
                amountRange = numOfEnemies.Value;
            else
                amountRange = (enemies.Count, enemies.Count);//leave the encounter as is

            //Min must be greater than 1
            amountRange.min.Clamp(1, amountRange.min);
            //If Max is less than 1 then the range will be intensionally ignored
        }

        public Encounter(bool canBeEscaped, params Enemy[] enemies)
        {
            this.enemies = enemies.ToList();
            CanBeEscaped = canBeEscaped;
        }

        //! NOTICE: even though the list is a copy, the actual enemies themselves aren't. Issues may occur
        public List<Enemy> GetEnemies()
        {
            //! The actual enemies must be copies
            List<Enemy> result = new();
            foreach (Enemy enemy in enemies)
                result.Add(new(enemy));

            //if the max is default(0) or if amountRange is equal to the original amount
            if (amountRange.max <= 0 || (amountRange.min == amountRange.max && amountRange.max == result.Count))
                return result;

            var amount = amountRange.max;
            //if range is actually a range
            if (amountRange.min < amountRange.max)
                amount = RNG.RandomInt(amountRange.min, amountRange.max);

            //Add or remove enemies until you reach the proper amount
            while (result.Count != amount)
            {
                if (result.Count < amount)
                    result.Add(new(enemies.GetRandomElement()));//Ensures that the starting encounter is preserved
                else
                    result.RemoveRandomElement();
            }

            return result;
        }
    }
}
