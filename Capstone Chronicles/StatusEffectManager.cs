using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//TODO remove this
using static Capstone_Chronicles.Program;

namespace Capstone_Chronicles
{
    public class StatusEffectManager
    {
        static StatusEffect poisoned = new StatusEffect("POISONED", 5, 8, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.Name + " felt the effects of the poison");
            Console.ForegroundColor = ConsoleColor.White;

            var baseDamage = (target.MaxHp / 10f);
            target.ModifyHealth(-(int)(baseDamage + RNG.RandomInt(-(int)(baseDamage / 5).Clamp(2, 10),
                (int)(baseDamage / 5).Clamp(2, 10))).Clamp(1, float.MaxValue), atkElmt: ElementManager.OMNI);

        }, "{0} got poisoned", "{0} recovered from the poison");

        static StatusEffect flaming = new StatusEffect("FLAMING", 1, 4, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.Name + " got burned by the flames");
            Console.ForegroundColor = ConsoleColor.White;

            var baseDamage = (target.MaxHp / 3f).Clamp(1, 40);
            target.ModifyHealth(-(int)(baseDamage + RNG.RandomInt(-(int)(baseDamage / 5).Clamp(2, 10),
                (int)(baseDamage / 5).Clamp(2, 10))), atkElmt: ElementManager.FIRE);

        }, "{0} got set on fire", "{0} is no longer on fire");

        static StatusEffect sleeping = new StatusEffect("SLEEPING", 2, 4, (target) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            print(target.Name + " is asleep");
            Console.ForegroundColor = ConsoleColor.White;

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
                target.nextAction = new Skill<Actor, Actor>(SkillManager.Attack);
                target.nextAction.targetType = SkillBase.TargetGroup.TARGET_ANYONE;
                target.targets.Clear();
            }
            else if (target.nextAction.targetType != SkillBase.TargetGroup.TARGET_EVERYONE && RNG.OneInFour)//use intended move on the wrong target(s)
            {
                if (target.targets.Count == 1 && (target.nextAction.targetType == SkillBase.TargetGroup.TARGET_SINGLE_OPPONENT
                    || target.nextAction.targetType == SkillBase.TargetGroup.TARGET_SINGLE_ALLY))//Hit anyone
                {
                    target.nextAction = new Skill<Actor, Actor>(target.nextAction as Skill<Actor, Actor>);
                    target.nextAction.targetType = SkillBase.TargetGroup.TARGET_ANYONE;
                }
                else//Target the opposite group from the intended targets
                {
                    target.nextAction = new Skill<Actor, List<Actor>>(target.nextAction as Skill<Actor, List<Actor>>);
                    if (target.nextAction.targetType == SkillBase.TargetGroup.TARGET_ALL_ALLIES)
                        target.nextAction.targetType = SkillBase.TargetGroup.TARGET_ALL_OPPONENTS;
                    else if (target.nextAction.targetType == SkillBase.TargetGroup.TARGET_ALL_OPPONENTS)
                        target.nextAction.targetType = SkillBase.TargetGroup.TARGET_ALL_ALLIES;
                }
                target.targets.Clear();
            }

        }, "{0} got confused", "{0} came to their senses");//change name to delusional?


        //public static StatusEffect NONE { get { return none; } }
        public static StatusEffect POISONED { get { return poisoned; } }
        public static StatusEffect FLAMING { get { return flaming; } }
        public static StatusEffect SLEEPING { get { return sleeping; } }
        public static StatusEffect PARALYZED { get { return paralyzed; } }
        public static StatusEffect CONFUSED { get { return confused; } }
    }

    [Serializable]
    public class StatusEffect
    {
        public string Name;

        Action<Actor> turnAction;
        Action<Actor> recoverAction;
        int minPossibleTurns = 1;
        int maxPossibleTurns = 8;

        int turnLimit;
        public int turnsActive = 0;

        string inflictText = "{0} isn't feeling well";
        string removeText = "{0} went back to normal";

        public StatusEffect(string n, int minPT, int maxPT, Action<Actor> tAction, string iText = "{0} isn't feeling well", string rtext = "{0} went back to normal", Action<Actor> recAction = null)
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
        public StatusEffect(StatusEffect dupe)
        {
            Name = dupe.Name;
            turnAction = dupe.turnAction;

            minPossibleTurns = dupe.minPossibleTurns;
            maxPossibleTurns = dupe.maxPossibleTurns;

            removeText = dupe.removeText;
            recoverAction = dupe.recoverAction;
        }


        public bool TryInflict(Actor target, int duration = -1, bool showInflictText = true, bool performImmediately = false)
        {
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

        public void PerformAction(Actor target)
        {
            turnAction.Invoke(target);

            turnsActive++;
            //Check for duration at the end of turn
        }
        public void TryRemoveEffect(Actor target, bool forceRemove = false, bool activateRecoverAction = true)
        {
            if (turnsActive >= turnLimit || forceRemove)
            {
                target.statusEffects[target.statusEffects.IndexOf(this)] = null;

                Console.ForegroundColor = ConsoleColor.Blue;
                print(string.Format(removeText, target.Name));
                Console.ForegroundColor = ConsoleColor.White;

                if (activateRecoverAction && recoverAction != null)
                    recoverAction.Invoke(target);
            }
        }

    }
}