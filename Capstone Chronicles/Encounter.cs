using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Stores data on the enemies and rules for a battle
    /// </summary>
    public class Encounter
    {
        /// <summary>
        /// The unique save key for this encounter
        /// </summary>
        public string? SaveKey { get; private set; }

        private List<Enemy> enemies;
        (int min, int max) amountRange = (0, 0);

        /// <summary>
        /// Can you flee from this encounter?
        /// </summary>
        public bool CanBeEscaped { get; private set; } = true;
        /// <summary>
        /// Can this encounter be repeated once beaten?
        /// </summary>
        public bool CanBeRepeated { get; private set; } = true;

        /// <summary>
        /// Has this encounter been beaten before?
        /// </summary>
        public bool hasBeenBeat = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Encounter"/> class.
        /// </summary>
        /// <param name="enemies">The types of enemies</param>
        /// <param name="numOfEnemies">The min and max number of enemies to spawn | Spawns the exact list if null</param>
        /// <param name="canBeEscaped">If true, can be escaped</param>
        /// <param name="canBeRepeated">If true, can be repeated</param>
        /// <param name="saveKey">The unique save key</param>
        public Encounter(List<Enemy> enemies, (int min, int max)? numOfEnemies = null, bool canBeEscaped = true,
            bool canBeRepeated = true, string? saveKey = null)
        {
            this.enemies = enemies;
            CanBeEscaped = canBeEscaped;
            CanBeRepeated = canBeRepeated;

            if (numOfEnemies != null)
                amountRange = numOfEnemies.Value;
            else
                amountRange = (enemies.Count, enemies.Count);//leave the encounter as is

            //Min must be greater than 1
            amountRange.min.Clamp(1, amountRange.min);
            //If Max is less than 1 then the range will be intensionally ignored

            SaveKey = saveKey;
            if (!string.IsNullOrEmpty(SaveKey))
            {
                GameManager.BeginSave += Save;
                GameManager.BeginSave += Load;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Encounter"/> class.
        /// </summary>
        /// <param name="canBeEscaped">If true, can be escaped</param>
        /// <param name="canBeRepeated">If true, can be repeated</param>
        /// <param name="saveKey">The unique save key</param>
        /// <param name="enemies">The enemies to spawn</param>
        public Encounter(bool canBeEscaped, bool canBeRepeated, string? saveKey, params Enemy[] enemies)
        {
            this.enemies = enemies.ToList();
            CanBeEscaped = canBeEscaped;
            CanBeRepeated = canBeRepeated;

            SaveKey = saveKey;
            if (!string.IsNullOrEmpty(SaveKey))
            {
                GameManager.BeginSave += Save;
                GameManager.BeginLoad += Load;
            }
        }

        /// <summary>
        /// Gets the enemies to spawn
        /// </summary>
        /// <returns>A list of Enemies.</returns>
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

        private void Save(int slot)
        {
            if (string.IsNullOrEmpty(SaveKey))
                return;

            SaveLoad.Save(hasBeenBeat, slot, SaveKey);
        }

        private void Load(int slot)
        {

            if (string.IsNullOrEmpty(SaveKey))
                return;

            hasBeenBeat = SaveLoad.Load<bool>(slot, SaveKey);
        }
    }
}
