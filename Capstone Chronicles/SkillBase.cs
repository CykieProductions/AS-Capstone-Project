using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Capstone_Chronicles.SkillBase;

namespace Capstone_Chronicles
{
    public class SkillBase
    {
        public string Name { get; protected set; } = "";
        //TODO decide if you want to use this
        public string Info { get; protected set; } = "";

        public int Cost { get; protected set; } = 0;

        /// <summary>
        /// 0: Based on speed;
        /// -1: Always last;
        /// 1: Always first;
        /// </summary>
        public int Priority { get; protected set; } = 0;

        public enum ActionType
        {
            DAMAGING, INFLICTING, HEALING, CURING, REVIVAL
        }
        public enum TargetGroup
        {
            SELF, ONE_OPPONENT, ALL_OPPONENTS, ONE_ALLY, ALL_ALLIES, ANYONE, EVERYONE
        }

        public ActionType actionType { get; protected set; } = ActionType.DAMAGING;
        public TargetGroup targetType { get; set; } = TargetGroup.SELF;
        public Element element = ElementManager.NORMAL;

        //Constructor
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

            SkillManager.callForSkill += TryGetSkill;//get skill by name without using reflection
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

    public class Skill<T1> : SkillBase //Skill with 1 argument
    {
        public Action<T1> action { get; protected set; }

        public Skill(string name, Action<T1> action, int cost = 0, TargetGroup targetType = TargetGroup.SELF
            , Element? elmt = null, int pri = 0, string info = "") : base(name, cost, targetType, elmt, info)
        {
            Priority = pri;
            this.action = action;
        }
        public Skill(Skill<T1> skill) : base(name: "", cost: 0, targetType: TargetGroup.SELF, elmt: null, info: "")
        {
            Name = skill.Name;
            Info = skill.Info;
            targetType = skill.targetType;
            Cost = skill.Cost;
            element = skill.element;

            Priority = skill.Priority;

            action = skill.action;
            actionType = skill.actionType;
        }

        public void Use(T1 t1)
        {
            action.Invoke(t1);
        }
    }

    public class Skill<T1, T2> : SkillBase //Skill with 2 arguments
    {
        public Action<T1, T2> action { get; private set; }

        public Skill(string name, Action<T1, T2> action, int cost = 0, TargetGroup targetType = TargetGroup.ONE_OPPONENT
            , Element? elmt = null, ActionType skillType = ActionType.DAMAGING, int pri = 0, string info = "") : base(name, cost, targetType, elmt, info)
        {
            Priority = pri;
            this.action = action;
            actionType = skillType;
        }
        public Skill(Skill<T1, T2> skill) : base(name: "", cost: 0, targetType: TargetGroup.ONE_OPPONENT, elmt: null, info: "")
        {
            Name = skill.Name;
            Info = skill.Info;
            targetType = skill.targetType;
            Cost = skill.Cost;
            element = skill.element;

            Priority = skill.Priority;

            action = skill.action;
            actionType = skill.actionType;
        }

        public void Use(T1 t1, T2 t2)
        {
            action.Invoke(t1, t2);
        }
    }
}
