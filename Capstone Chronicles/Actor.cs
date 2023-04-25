using System;
using System.Collections.Generic;
using System.Linq;
using static Capstone_Chronicles.Scene;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Actors participate in battles. 
    /// </summary>
    public abstract partial class Actor
    {
        //Contains nested struct*/ StatsStruct
        //Contains nested class*/ StatGrowth

        /*? Why are the stats here:
         * I originally used a Stats struct for this, but that meant either making every setter 
         * public, or moving leveling logic into the struct, which would defeat the purpose.
         * Only Actors need access to setting stats anyway, so I just moved the stats here.
         */
        #region Basic Stats
        public int Level { get; protected set; } = 1;
        public int MaxHp { get; protected set; }

        private int _hp;
        public int Hp { get => _hp; protected set => _hp = value.Clamp(0, MaxHp); }

        public int MaxSp { get; protected set; }

        private int _sp;
        public int Sp { get => _sp; set => _sp = value.Clamp(0, MaxSp); }

        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public int Special { get; protected set; }
        public int Speed { get; protected set; }
        public int Exp { get; set; }
        #endregion
        //*/

        public string Name { get; set; } = "";
        public Element Element { get; set; }
        public bool isGuarding;

        public SkillBase? nextAction;
        public List<Actor> targets = new List<Actor>();

        /// <summary>All current skills</summary>
        public List<SkillBase> skills = new List<SkillBase>();
        /// <summary>Skills that can be learned by leveling up</summary>
        protected Dictionary<int, SkillBase> levelUpSkillDictionary;

        public List<StatusEffect?> statusEffects = new();

        //Constants
        public const int MAX_LEVEL = 100;
        public const float EFFECT_CAPACITY = 3;

        /// <summary>
        /// Fills in basic info and will update the usable skill list based on the inputted level and skill dictionary
        /// </summary>
        /// <param name="inName"></param>
        /// <param name="inElement"></param>
        /// <param name="inStats">A struct containing all stat data</param>
        /// <param name="inSkillDict"></param>
        public Actor(string inName, Element inElement, StatsStruct inStats, Dictionary<int, SkillBase> inSkillDict)
        {
            Name = inName;
            Element = inElement;
            levelUpSkillDictionary = inSkillDict;

            //Learn any skills lower than or equal to current level
            for (int i = 0; i <= Level; i++)
            {
                levelUpSkillDictionary.TryGetValue(i, out SkillBase? newSkill);
                if (newSkill != null)
                    LearnSkill(newSkill, false);
            }

            Level = inStats.Level;
            MaxHp = inStats.MaxHp;
            Hp = inStats.Hp;
            MaxSp = inStats.MaxSp;
            Sp = inStats.Sp;
            Attack = inStats.Attack;
            Defense = inStats.Defense;
            Special = inStats.Special;
            Speed = inStats.Speed;
            Exp = inStats.Exp;
        }

        /// <summary>
        /// Adds a new skill to the Actor's usable skill list
        /// </summary>
        /// <param name="newSkill">The new skill to be learned</param>
        /// <param name="display">Should the player be alerted about the new skill</param>
        public virtual void LearnSkill(SkillBase newSkill, bool display = false)
        {
            skills.Add(newSkill);
            if (display)
                print($"{Name} learned {newSkill.Name}!");
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
            //! Damage Logic
            else
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

            //Defeat
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
        public void SetHealth (int value)
        {
            Hp = value.Clamp(0, MaxHp);
        }

        public virtual void ModifyStamina(int value)
        {
            if (value > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                if (Sp + value < MaxSp)
                    print(Name + " gained " + value + " SP");
                else
                    print(Name + "'s SP is maxed out");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Sp = (Sp + value).Clamp(0, MaxSp);
        }
        public void SetStamina(int value)
        {
            Sp = value.Clamp(0, MaxSp);
        }
    }

    /// <summary>
    /// An actor that fights on the player's team
    /// </summary>
    public class Hero : Actor
    {
        public int NeededExp { get; private set; } = 4;
        public StatGrowth GrowthInfo { get; private set; }
        public static event Action<StatGrowth, Actor>? AfterInit;

        public Hero(string inName, StatsStruct inStats, StatGrowth inGrowthInfo, 
            Dictionary<int, SkillBase> inSkillDict, Element? inElement = null) 
            : base(inName, inElement ?? ElementManager.NORMAL, inStats, inSkillDict)
        {
            //TODO implement StatGrowth
            GrowthInfo = inGrowthInfo;

            AfterInit?.Invoke(GrowthInfo, this);
        }

        private void SortSkills()
        {
            skills = skills.OrderBy(x => x.actionType).ThenBy(x => x.element.Name).
                ThenBy(x => x.targetType).ThenBy(x => x.Cost).ToList();
        }

        public override void LearnSkill(SkillBase newSkill, bool display = true)
        {
            if (!skills.Contains(newSkill)) //Heroes shouldn't have duplicate skills, but enemies can
            {
                base.LearnSkill(newSkill);
            }

            SortSkills();
        }

        /// <summary>
        /// The hero will attempt to level up as many times as they can
        /// </summary>
        public void TryLevelUp()
        {
            while (Exp >= NeededExp)
            {
                Level++;
                Exp -= NeededExp;

                Console.ForegroundColor = ConsoleColor.Green;
                print($"{Name} has reached LV {Level}!");
                Console.ForegroundColor = ConsoleColor.White;

                //! Stat Growth

                MaxHp = GrowthInfo.StatGain(StatType.Max_HP);
                Hp = MaxHp;
                MaxSp = GrowthInfo.StatGain(StatType.Max_SP);
                Sp = MaxSp;
                Attack = GrowthInfo.StatGain(StatType.Attack);
                Defense = GrowthInfo.StatGain(StatType.Defense);
                Special = GrowthInfo.StatGain(StatType.Special);
                Speed = GrowthInfo.StatGain(StatType.Speed);
                NeededExp = GrowthInfo.StatGain(StatType.Exp, false);


                //! Skills
                levelUpSkillDictionary.TryGetValue(Level, out SkillBase? newSkill);
                if (newSkill != null)
                    LearnSkill(newSkill);
            }
        }

    }//End of Hero

    /// <summary>
    /// An actor that fights against the player
    /// </summary>
    public class Enemy : Actor
    {
        /// <summary>
        /// Modify this to give the enemy custom behavior
        /// </summary>
        public Func<SkillBase> decideTurnAction;

        public Enemy(string inName, Element inElement, StatsStruct inStats, List<SkillBase>? inSkills,
            Dictionary<int, SkillBase>? inSkillDict = null, Func<SkillBase>? ai = null)
            : base(inName, inElement, inStats, inSkillDict ?? new())
        {
            //Add the list of default skills to any learned through "level up"
            if (inSkills != null)
                skills.AddRange(inSkills);

            //Normal attack should come last
            skills.Add(SkillManager.Attack);

            if (ai != null)
                decideTurnAction = ai;
            else
                decideTurnAction = () => { return DecideTurnAction(); };
        }
        /// <summary>
        /// Create a duplicate of an enemy, but with a different name and/or AI
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="inName"></param>
        /// <param name="ai"></param>
        public Enemy(Enemy enemy, string? inName = null, Func<SkillBase>? ai = null)
            : base(inName ?? enemy.Name, enemy.Element, default, enemy.levelUpSkillDictionary)
        {
            Level = enemy.Level;
            MaxHp = enemy.MaxHp;
            Hp = enemy.Hp;
            MaxSp = enemy.MaxSp;
            Sp = enemy.Sp;
            Attack = enemy.Attack;
            Defense = enemy.Defense;
            Special = enemy.Special;
            Speed = enemy.Speed;
            Exp = enemy.Exp;

            skills = enemy.skills;

            //Normal attack should come last
            skills.Add(SkillManager.Attack);

            if (ai != null)
                decideTurnAction = ai;
            else
                decideTurnAction = () => { return DecideTurnAction(); };
        }

        SkillBase DecideTurnAction()
        {
            var skillPool = skills.ToList();//copy of the original
            foreach (var skill in skills)
            {
                if (skill.Cost > Sp && RNG.OneInTwo)//Leaves a small chance to use a skill that costs too much
                    skillPool.Remove(skill);
            }

            var randInt = RNG.RandomInt(0, skillPool.Count);
            if (randInt == skillPool.Count)//Basic Attack is last so this creates an increased chance for that
                randInt = skillPool.Count - 1;

            return skillPool[randInt];
        }
        public void ChooseTarget(List<Actor> actorPool, 
            SkillBase.TargetGroup targetType = SkillBase.TargetGroup.ONE_OPPONENT)
        {
            targets.Clear();
            for (int i = 0; i < actorPool.Count; i++)//Remove defeated actors from potential targets list
            {
                if (actorPool[i].Hp <= 0 
                    || (targetType == SkillBase.TargetGroup.ONE_OPPONENT && actorPool[i] is Enemy)
                    || (targetType == SkillBase.TargetGroup.ONE_ALLY && actorPool[i] is Hero))
                    actorPool.RemoveAt(i);

            }

            if (targetType == SkillBase.TargetGroup.ONE_OPPONENT 
                || targetType == SkillBase.TargetGroup.ONE_ALLY)
            {
                targets.Add(actorPool[RNG.RandomInt(0, actorPool.Count - 1)]);
            }
            else if (targetType != SkillBase.TargetGroup.SELF)
                targets = actorPool;
        }

    }
}
