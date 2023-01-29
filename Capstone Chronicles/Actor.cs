using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Capstone_Chronicles.Element;
using System.Xml.Linq;

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
        public int Hp { get; protected set; }
        public int MaxSp { get; protected set; }

        private int _sp;
        public int Sp { get => _sp; set 
            {    
                _sp = value;
                if (_sp < 0)
                    _sp = 0;
                else if (_sp > MaxSp)
                    _sp = MaxSp;
            } }

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

        public SkillBase nextAction;
        public List<Actor> targets = new List<Actor>();

        public List<SkillBase> skills = new List<SkillBase>();

        public List<StatusEffect> statusEffects;

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

        public virtual void ModifyHealth(int amount, SkillBase? skillUsed = null, Element? atkElmt = null)
        {
            if (skillUsed != null)
                atkElmt ??= skillUsed.element;
            else
                atkElmt ??= ElementManager.OMNI;

            amount = (int)(amount * ElementManager.CalculateAttackMultiplier(atkElmt, this));

            Hp += amount;

            if (Hp < 0)
                Hp = 0;
            else if (Hp > MaxHp)
                Hp = MaxHp;
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
