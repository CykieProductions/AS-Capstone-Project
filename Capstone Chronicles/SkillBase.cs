using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Chronicles
{
    /// <summary>
    /// The base that all skills inherit from
    /// </summary>
    public abstract class SkillBase
    {
        /// <summary>
        /// The name of the skill
        /// </summary>
        public string Name { get; protected set; } = "";
        /// <summary>
        /// A description of the skill
        /// </summary>
        [Obsolete("Not currently in use")]
        public string Info { get; protected set; } = "";

        /// <summary>
        /// The SP cost of the skill
        /// </summary>
        public int Cost { get; protected set; } = 0;

        /// <summary>
        /// Helps determine what order Actors act in
        /// <para>
        /// 0: Based on speed;
        /// -1: Always last;
        /// 1: Always first;
        /// </para>
        /// </summary>
        public int Priority { get; protected set; } = 0;

        /// <summary>
        /// The primary function of this skill
        /// </summary>
        public enum ActionType
        {
            DAMAGING, INFLICTING, HEALING, CURING, REVIVAL
        }
        /// <summary>
        /// The amount and type of Actor to target
        /// </summary>
        public enum TargetGroup
        {
            SELF, ONE_OPPONENT, ALL_OPPONENTS, ONE_ALLY, ALL_ALLIES, ANYONE, EVERYONE
        }

        /// <summary>
        /// The primary function of this skill
        /// </summary>
        public ActionType actionType { get; protected set; } = ActionType.DAMAGING;
        /// <summary>
        /// The amount and type of Actor to target
        /// </summary>
        public TargetGroup targetType { get; set; } = TargetGroup.SELF;
        /// <summary>
        /// The element of the skill
        /// </summary>
        public Element element = ElementManager.NORMAL;


        /// <summary>
        /// Initializes a new instance of the <see cref="SkillBase"/> class.
        /// </summary>
        /// <param name="name">The name of the skill</param>
        /// <param name="cost">The SP cost</param>
        /// <param name="targetType">The amount and type of Actor to target</param>
        /// <param name="elmt">The <see cref="Element"/></param>
        /// <param name="info">A description of the skill</param>
        public SkillBase(string name, int cost = 0, TargetGroup targetType = TargetGroup.SELF, Element? elmt = null, string info = "")
        {
            Name = name;
            //_skillDescription = d;
            Info = info;
            this.targetType = targetType;

            Cost = cost;

            if (elmt == null)
                elmt = ElementManager.NORMAL;
            element = elmt;

            actionType = ActionType.DAMAGING;

            Priority = 0;

            SkillManager.CallForSkill += TryGetSkill;//get skill by name without using reflection
        }

        //Methods
        SkillBase? TryGetSkill(string name)//called from SkillManager.GetSkillByName
        {
            if (name == Name)
            {
                return this;
            }

            return null;
        }
    }

    /// <summary>
    /// A <see cref="SkillBase"/> with one argument
    /// </summary>
    /// <typeparam name="T1">Typically the <see cref="Actor"/> that used the skill</typeparam>
    public class Skill<T1> : SkillBase where T1 : Actor
    {
        /// <summary>
        /// The action to perform when the skill is used
        /// </summary>
        public Action<T1> action { get; protected set; }

        /// <summary>
        /// Initializes a new instance of <see cref="SkillBase"/> with one argument
        /// </summary>
        /// <param name="name">The name of the skill</param>
        /// <param name="action">The skill's behavior</param>
        /// <param name="cost">The SP cost</param>
        /// <param name="targetType">The amount and type of Actor to target</param>
        /// <param name="elmt">The element</param>
        /// <param name="pri">The priority</param>
        /// <param name="info">The description</param>
        public Skill(string name, Action<T1> action, int cost = 0, TargetGroup targetType = TargetGroup.SELF
            , Element? elmt = null, int pri = 0, string info = "") : base(name, cost, targetType, elmt, info)
        {
            Priority = pri;
            this.action = action;
        }

        /// <summary>
        /// Trigger the skill
        /// </summary>
        /// <param name="t1">The user</param>
        public void Use(T1 t1)
        {
            action.Invoke(t1);
        }
    }

    /// <summary>
    /// A <see cref="SkillBase"/> with two argument
    /// </summary>
    /// <typeparam name="T1">Typically the <see cref="Actor"/> that used the skill</typeparam>
    /// <typeparam name="T2">Typically an <see cref="Actor"/> or a List of them, that are the targets of the skill</typeparam>
    public class Skill<T1, T2> : SkillBase where T1 : Actor
    {
        /// <summary>
        /// The action to perform when the skill is used
        /// </summary>
        public Action<T1, T2> action { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="SkillBase"/> with two arguments
        /// </summary>
        /// <param name="name">The name of the skill</param>
        /// <param name="action">The skill's behavior</param>
        /// <param name="cost">The SP cost</param>
        /// <param name="targetType">The amount and type of Actor to target</param>
        /// <param name="elmt">The element</param>
        /// <param name="skillType">The primary function of the skill</param>
        /// <param name="pri">The priority</param>
        /// <param name="info">The description</param>
        public Skill(string name, Action<T1, T2> action, int cost = 0, TargetGroup targetType = TargetGroup.ONE_OPPONENT
            , Element? elmt = null, ActionType skillType = ActionType.DAMAGING, int pri = 0, string info = "") : base(name, cost, targetType, elmt, info)
        {
            Priority = pri;
            this.action = action;
            actionType = skillType;
        }

        /// <summary>
        /// Trigger the skill
        /// </summary>
        /// <param name="t1">The user</param>
        /// <param name="t2">The target(s)</param>
        public void Use(T1 t1, T2 t2)
        {
            action.Invoke(t1, t2);
        }
    }
}
