using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Capstone_Chronicles.Scene;//*/

namespace Capstone_Chronicles
{
    /// <summary>
    /// Contains data on all normal <see cref="StatusEffect"/>s
    /// </summary>
    public class StatusEffectManager
    {
        static StatusEffect poisoned = new StatusEffect("POISONED", 5, 8, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.Name + " felt the effects of the poison");
            Console.ForegroundColor = ConsoleColor.White;

            var baseDamage = (target.MaxHp / 10f);
            target.ModifyHealth(-(int)(baseDamage + RNG.RandomInt(-(int)(baseDamage / 5).Clamp(2, 10),
                (int)(baseDamage / 5).Clamp(2, 10))).Clamp(1, float.MaxValue), atkElmt: ElementManager.OMNI, 
                ignoreGuard: true);

        }, "{0} got poisoned", "{0} recovered from the poison");

        static StatusEffect flaming = new StatusEffect("FLAMING", 1, 4, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.Name + " got burned by the flames");
            Console.ForegroundColor = ConsoleColor.White;

            var baseDamage = (target.MaxHp / 8f).Clamp(1, 30);
            target.ModifyHealth(-(int)(baseDamage + RNG.RandomInt(-(int)(baseDamage / 5).Clamp(2, 10),
                (int)(baseDamage / 5).Clamp(2, 10))), atkElmt: ElementManager.FIRE,
                ignoreGuard: true);

        }, "{0} got set on fire", "{0} is no longer on fire");

        static StatusEffect sleeping = new StatusEffect("SLEEPING", 2, 4, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.Name + " is asleep");
            Console.ForegroundColor = ConsoleColor.White;

            target.ModifyStamina(RNG.RandomInt(1, (int)(target.MaxSp / 10f).Clamp(5, 20)));

            target.nextAction = SkillManager.Sleep;
            target.targets.Clear();
        }, "{0} got put in a deep sleep", "{0} woke up");

        static StatusEffect paralyzed = new StatusEffect("PARALYZED", 7, 12, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.Name + " is fighting the paralysis");
            Console.ForegroundColor = ConsoleColor.White;

            if (RNG.OneInTwo)
            {
                target.nextAction = SkillManager.Immobile;
                target.targets.Clear();
            }
        }, "{0} got paralyzed", "{0} is free of the paralysis");

        static StatusEffect confused = new StatusEffect("CONFUSED", 2, 5, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.Name + " is confused");
            Console.ForegroundColor = ConsoleColor.White;

            if (RNG.OneInThree)//ignore command and do a normal attack on anyone
            {
                target.nextAction = SkillManager.Attack;
                target.nextAction.targetType = SkillBase.TargetGroup.ANYONE;
                target.targets.Clear();
            }
            else if (target.nextAction.targetType != SkillBase.TargetGroup.EVERYONE && RNG.OneInFour)//use intended move on the wrong target(s)
            {
                if (target.targets.Count == 1 && (target.nextAction.targetType == SkillBase.TargetGroup.ONE_OPPONENT
                    || target.nextAction.targetType == SkillBase.TargetGroup.ONE_ALLY))//Hit anyone
                {
                    target.nextAction.targetType = SkillBase.TargetGroup.ANYONE;
                }
                else//Target the opposite group from the intended targets
                {
                    //target.nextAction = new Skill<Actor, List<Actor>>(target.nextAction as Skill<Actor, List<Actor>>);
                    if (target.nextAction.targetType == SkillBase.TargetGroup.ALL_ALLIES)
                        target.nextAction.targetType = SkillBase.TargetGroup.ALL_OPPONENTS;
                    else if (target.nextAction.targetType == SkillBase.TargetGroup.ALL_OPPONENTS)
                        target.nextAction.targetType = SkillBase.TargetGroup.ALL_ALLIES;
                }
                target.targets.Clear();
            }

        }, "{0} got confused", "{0} came to their senses");

        /// <summary>
        /// A condition that inflicts damage
        /// </summary>
        public static StatusEffect POISONED { get { return poisoned; } }
        /// <summary>
        /// A condition that inflicts <see cref="ElementManager.FIRE"/> damage
        /// </summary>
        public static StatusEffect FLAMING { get { return flaming; } }
        /// <summary>
        /// A condition that stops an Actor from acting, but recovers a bit of their SP
        /// </summary>
        public static StatusEffect SLEEPING { get { return sleeping; } }
        /// <summary>
        /// A condition that may stop an Actor from acting
        /// </summary>
        public static StatusEffect PARALYZED { get { return paralyzed; } }
        /// <summary>
        /// A condition that may scramble an Actors actions and targets
        /// </summary>
        public static StatusEffect CONFUSED { get { return confused; } }
    }


    /// <summary>
    /// Contains data on what to do to an actor when they are inflicted
    /// </summary>
    public class StatusEffect
    {
        /// <summary>
        /// The name of the condition
        /// </summary>
        public string Name;

        Action<Actor> turnAction;
        Action<Actor>? recoverAction;
        int minPossibleTurns = 1;
        int maxPossibleTurns = 8;

        int turnLimit;
        private int turnsActive = 0;

        string inflictText = "{0} isn't feeling well";
        string removeText = "{0} went back to normal";

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusEffect"/> class.
        /// </summary>
        /// <param name="n">The name</param>
        /// <param name="minPT">The minimum possible turns to stay</param>
        /// <param name="maxPT">The maximum possible turns to stay</param>
        /// <param name="tAction">The action to trigger each turn</param>
        /// <param name="iText">The text to display after infliction</param>
        /// <param name="rtext">The text to display after recovery</param>
        /// <param name="recAction">An extra action to trigger after recovery</param>
        public StatusEffect(string n, int minPT, int maxPT, Action<Actor> tAction, string iText = "{0} isn't feeling well", string rtext = "{0} went back to normal", Action<Actor>? recAction = null)
        {
            Name = n;
            turnAction = tAction;

            if (maxPT < 0)
                maxPT = int.MaxValue;

            minPossibleTurns = minPT;
            maxPossibleTurns = maxPT;

            inflictText = iText;
            removeText = rtext;

            recoverAction = recAction;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusEffect"/> class based on another
        /// </summary>
        /// <param name="dupe">The StatusEffect to clone</param>
        public StatusEffect(StatusEffect dupe)
        {
            Name = dupe.Name;
            turnAction = dupe.turnAction;

            minPossibleTurns = dupe.minPossibleTurns;
            maxPossibleTurns = dupe.maxPossibleTurns;

            removeText = dupe.removeText;
            recoverAction = dupe.recoverAction;
        }

        /// <summary>
        /// Attempt to give the target this status effect
        /// </summary>
        /// <param name="target"></param>
        /// <param name="duration">The amount of turn it should last (-1 means random)</param>
        /// <param name="showInflictText"></param>
        /// <param name="performImmediately">Should the status effect perform its action now, even if it's out of turn</param>
        /// <returns></returns>
        public bool TryInflict(Actor target, int duration = -1, bool showInflictText = true, bool performImmediately = false)
        {
            if (target.Hp <= 0)
                return false;

            Thread.Sleep(TimeSpan.FromSeconds(0.01));//For randomizing
            target.statusEffects.RemoveAll((m) => m == null);
            //print(target.Name + " has " + target.statusEffects.Count + " status effects. Can add more: " + (target.statusEffects.Count < Actor.statusEffectCapacity));

            bool alreadyInflicted = target.statusEffects.Exists((n) => n.Name == Name);
            bool successful = target.statusEffects.Count < Actor.EFFECT_CAPACITY && !alreadyInflicted;//if list small enough and not already inflicted

            if (successful)
            {
                var dupe = new StatusEffect(this);
                target.statusEffects.Add(dupe);
                if (duration < 0)
                    dupe.turnLimit = RNG.RandomInt(minPossibleTurns, maxPossibleTurns);
                else
                    dupe.turnLimit = duration;

                if (showInflictText)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    print(string.Format(inflictText, target.Name));
                    Console.ForegroundColor = ConsoleColor.White;
                }

                if (performImmediately)
                {
                    PerformAction(target);
                    turnsActive--;
                }

            }
            else if (alreadyInflicted)
                print(target.Name + " is already " + Name.ToLower());

            return successful;
        }

        /// <summary>
        /// Triggers <see cref="turnAction"/> and increments the turn counter
        /// </summary>
        /// <param name="target">The target to trigger on</param>
        public void PerformAction(Actor target)
        {
            if (target.Hp <= 0)
                return;

            turnAction.Invoke(target);

            turnsActive++;
            //Check for duration at the end of turn
        }
        /// <summary>
        /// Removes the effect if it's been active for enough turns
        /// </summary>
        /// <param name="target">The target to remove from</param>
        /// <param name="forceRemove">If true, removes the effect no matter what</param>
        /// <param name="activateRecoverAction">If true, trigger the special recovery action if it exists</param>
        public void TryRemoveEffect(Actor target, bool forceRemove = false, bool activateRecoverAction = true)
        {
            if (target.Hp <= 0)
                return;

            if (turnsActive >= turnLimit || forceRemove)
            {
                target.statusEffects[target.statusEffects.IndexOf(this)] = null;

                Console.ForegroundColor = ConsoleColor.Blue;
                print(string.Format(removeText, target.Name));
                Console.ForegroundColor = ConsoleColor.White;

                if (activateRecoverAction)
                    recoverAction?.Invoke(target);
            }
        }

    }
}