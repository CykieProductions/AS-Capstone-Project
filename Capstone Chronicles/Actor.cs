using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Capstone_Chronicles.Program;
using System.Xml.Linq;
using static Capstone_Chronicles.Element;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Actors participate in battles. 
    /// </summary>
    public partial class Actor
    {
        //Contains nested struct StatsStruct

        /*? Why are the stats here:
         * I originally used a Stats struct for this, but that meant either making every setter 
         * public, or moving leveling logic into the struct, which would defeat the purpose.
         * Only Actors need access to setting stats anyway, so I just moved the stats here.
         */
        #region Basic Stats
        public Element Element { get; set; }
        public int Level { get; protected set; }
        public int MaxHp { get; protected set; }

        private int _hp;
        public int Hp { get => _hp; set => _hp = value.Clamp(0, MaxHp); }

        public int MaxSp { get; protected set; }

        private int _sp;
        public int Sp { get => _sp; set => _sp = value.Clamp(0, MaxSp); }

        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public int SpAttack { get; protected set; }
        public int SpDefense { get; protected set; }
        public int Speed { get; protected set; }
        public int Exp { get; protected set; }
        #endregion
        //*/

        public string Name { get; set; } = "";
        public bool isGuarding;

        public SkillBase? nextAction;
        public List<Actor> targets = new List<Actor>();

        public List<SkillBase> skills = new List<SkillBase>();

        public List<StatusEffect?> statusEffects = new();

        //Constants
        public const int MAX_LEVEL = 100;
        public const float EFFECT_CAPACITY = 3;

        public Actor(string inName, Element inElement, StatsStruct inStats)
        {
            Name = inName;
            Element = inElement;

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

        /// <summary>
        /// Performs various calculations and then changes HP.
        /// </summary>
        /// <param name="amount">The base value to change by</param>
        /// <param name="skillUsed">The skill causing the change</param>
        /// <param name="atkElmt">Overrides the normal element with this one</param>
        /// <returns>The actual amount the HP changed by</returns>
        public virtual int ModifyHealth(int amount, SkillBase? skillUsed = null, Element? atkElmt = null, bool ignoreGuard = false)
        {
            int startHp = Hp;

            if (skillUsed != null)
                atkElmt ??= skillUsed.element;
            else
                atkElmt ??= ElementManager.OMNI;

            bool isBeingRevived = false;
            bool shouldAffectHp = true;

            //! Healing Logic
            if (amount > 0)
            {
                if (Hp <= 0 && skillUsed != null && skillUsed.actionType == SkillBase.ActionType.REVIVAL)
                {
                    Hp = 1;
                    isBeingRevived = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    print(Name + " was revived!");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                shouldAffectHp = Hp > 0 || isBeingRevived;

                if (shouldAffectHp)//Won't heal if still down
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (Hp + amount < MaxHp)
                        print(Name + " gained " + amount + " HP");
                    else
                        print(Name + "'s HP is maxed out");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                    print("It had no visible effect on " + Name);
            }
            else//Damage logic
            {
                var mult = ElementManager.CalculateAttackMultiplier(atkElmt, this);
                //Deal at least 1 damage unless the multiplier is specifically 0
                if (mult > 0)
                {
                    amount = (int)(amount * mult).Clamp(int.MinValue, -1);

                    if (mult == ElementManager.BONUS_MULT)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        print("Critical damage!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (mult == ElementManager.REDUCED_MULT)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        print("It wasn't very effective");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    //! Apply defense algorithm
                    amount = (int)(amount * (100f / (100f + Defense + Defense * .25f)));

                    if (!ignoreGuard && isGuarding && amount < 0)//Don't bother guarding 0 damage attacks
                    {
                        amount = (int)(amount * 0.6f).Clamp(int.MinValue, -1);
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    print(Name + " took " + -amount + " damage");
                    Console.ForegroundColor = ConsoleColor.White;

                }
                else//Element was blocked
                {
                    shouldAffectHp = false;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    print(Name + " was unfazed by the attack!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            if (shouldAffectHp)
                Hp = (Hp + amount).Clamp(0, MaxHp);

            if (Hp <= 0)
            {
                if (this is Hero)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Cyan;

                print(Name + " was defeated!");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return Hp - startHp;
        }

        public virtual void ModifyConra(int value, Element? attackElement = null)
        {
            if (attackElement == null)
                attackElement = ElementManager.NORMAL;

            if (value > 0)
            {

                Console.ForegroundColor = ConsoleColor.Cyan;
                /*if (Sp + value < MaxSp)
                    print(Name + " gained " + value + " SP");
                else
                    print(Name + "'s SP is maxed out");*/
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                float mult = ElementManager.CalculateAttackMultiplier(attackElement, this);
                value = (int)(value * mult);

                //x print("SP loss has been multiplied by " + mult);

                Console.ForegroundColor = ConsoleColor.Blue;
                //print(Name + " lost " + -value + " SP");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Sp = (Sp + value).Clamp(0, MaxSp);
        }
    }

    /// <summary>
    /// An actor that fights on the player's team
    /// </summary>
    public partial class Hero : Actor
    {
        //Contains nested class StatGrowth

        public Hero(string inName, StatsStruct inStats, StatGrowth growthInfo, Element? inElement = null) 
            : base(inName, inElement == null ? ElementManager.NORMAL : inElement, inStats)
        {
            
            //TODO implement StatGrowth

        }
    }

    /// <summary>
    /// An actor that fights against the player
    /// </summary>
    public class Enemy : Actor
    {
        public Enemy(string inName, Element inElement, StatsStruct inStats) : base(inName, inElement, inStats)
        {
        }
    }
}
