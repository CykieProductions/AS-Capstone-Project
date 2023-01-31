using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    public static class ElementManager
    {
        #region All Elements
        /// <summary>
        /// Weak To: nothing |
        /// Resists: nothing
        /// </summary>
        public static Element OMNI { get; private set; } = new Element(Element.Name.OMNI);

        /// <summary>
        /// Weak To: nothing |
        /// Resists: nothing
        /// </summary>
        public static Element NORMAL { get; private set; } = new Element(Element.Name.NORMAL);

        /// <summary>
        /// Weak To: water, ground, air |
        /// Resists: fire, plant, electric
        /// </summary>
        public static Element FIRE { get; private set; } = new Element(Element.Name.FIRE);

        /// <summary>
        /// Weak To: plant, electric |
        /// Resists: water, ground
        /// </summary>
        public static Element WATER { get; private set; } = new Element(Element.Name.WATER);

        /// <summary>
        /// Weak To: fire |
        /// Resists: water
        /// </summary>
        public static Element PLANT { get; private set; } = new Element(Element.Name.PLANT);

        /// <summary>
        /// Weak To: water, plant |
        /// Resists: normal, fire |
        /// Ignores: electric
        /// </summary>
        public static Element GROUND { get; private set; } = new Element(Element.Name.GROUND);

        /// <summary>
        /// Weak To: electric |
        /// Resists: air |
        /// Ignores: normal, ground
        /// </summary>
        public static Element AIR { get; private set; } = new Element(Element.Name.AIR);

        /// <summary>
        /// Weak To: ground |
        /// Resist: air, electric
        /// </summary>
        public static Element ELECTRIC { get; private set; } = new Element(Element.Name.ELECTRIC);
        #endregion

        //Static constructor to initialize the strengths and weaknesses of all above elements
        static ElementManager()
        {
            /// <summary>
            /// Weak To: nothing |
            /// Resists: nothing
            /// </summary>
            OMNI._Initialize
                (
                effectiveAgainst: new Element[] { },
                ineffectiveAgainst: new Element[] { },
                uselessAgainst: new Element[] { }
                );

            /// <summary>
            /// Weak To: nothing |
            /// Resists: nothing
            /// </summary>
            NORMAL._Initialize
                (
                effectiveAgainst: new Element[] { },
                ineffectiveAgainst: new Element[] { GROUND },
                uselessAgainst: new Element[] { AIR }
                );

            /// <summary>
            /// Weak To: water, ground, air |
            /// Resists: fire, plant, electric
            /// </summary>
            FIRE._Initialize
                (
                effectiveAgainst: new Element[] { PLANT },
                ineffectiveAgainst: new Element[] { FIRE, WATER, GROUND },
                uselessAgainst: new Element[] { }
                );

            /// <summary>
            /// Weak To: plant, electric |
            /// Resists: water, ground
            /// </summary>
            WATER._Initialize
                (
                effectiveAgainst: new Element[] { FIRE, GROUND },
                ineffectiveAgainst: new Element[] { WATER, PLANT },
                uselessAgainst: new Element[] { }
                );

            /// <summary>
            /// Weak To: fire |
            /// Resists: water
            /// </summary>
            PLANT._Initialize
                (
                effectiveAgainst: new Element[] { GROUND, WATER },
                ineffectiveAgainst: new Element[] { FIRE },
                uselessAgainst: new Element[] { }
                );

            /// <summary>
            /// Weak To: water, plant |
            /// Resists: normal, fire |
            /// Ignores: electric
            /// </summary>
            GROUND._Initialize
                (
                effectiveAgainst: new Element[] { FIRE, ELECTRIC },
                ineffectiveAgainst: new Element[] { WATER },
                uselessAgainst: new Element[] { AIR }
                );

            /// <summary>
            /// Weak To: electric |
            /// Resists: air |
            /// Ignores: normal, ground
            /// </summary>
            AIR._Initialize
                (
                effectiveAgainst: new Element[] { FIRE },
                ineffectiveAgainst: new Element[] { AIR, ELECTRIC },
                uselessAgainst: new Element[] { }
                );

            /// <summary>
            /// Weak To: ground |
            /// Resist: air, electric
            /// </summary>
            ELECTRIC._Initialize
                (
                effectiveAgainst: new Element[] { WATER, AIR },
                ineffectiveAgainst: new Element[] { ELECTRIC, FIRE },
                uselessAgainst: new Element[] { GROUND }
                );
        }

        public const float BONUS_MULT = 1.75f;
        public const float REDUCED_MULT = 0.5f;
        public const float USELESS_MULT = 0f;

        public static float CalculateAttackMultiplier(Element attackElement, Actor target)
        {
            float mult = 1;

            if (target.Element.IsUnfazedBy(attackElement))//For special elements only
            {
                mult = USELESS_MULT;
            }
            else if (attackElement.IsEffectiveAgainst(target.Element))
            {
                mult = BONUS_MULT;
            }
            else if (attackElement.IsIneffectiveAgainst(target.Element))
            {
                mult = REDUCED_MULT;
            }
            else if (attackElement.IsUselessAgainst(target.Element))
            {
                mult = USELESS_MULT;
            }

            return mult;
        }
    }

    public class Element
    {
        public enum Name
        {
            OMNI, NORMAL, FIRE, WATER, PLANT, GROUND, AIR, ELECTRIC
        }
        public Name NameFromEnum { get; private set; }


        private Element[] effectiveAgainst = null;
        private Element[] ineffectiveAgainst = null;
        private Element[] uselessAgainst = null;

        private Element[] unfazedBy = Array.Empty<Element>();

        /// <summary>
        /// Constructs a new element with strengths and weaknesses
        /// </summary>
        /// <param name="inName"></param>
        /// <param name="effectiveAgainst">Elements that I do bonus damage to</param>
        /// <param name="ineffectiveAgainst">Elements that I do reduced damage to</param>
        /// <param name="uselessAgainst">Elements that I can't affect</param>
        /// <param name="unfazedBy">Ensures I can't be affected by the basic elements. Only used for temporary elements</param>
        public Element(Name inName, Element[]? effectiveAgainst = null, Element[]? ineffectiveAgainst = null, 
            Element[]? uselessAgainst = null, Element[]? unfazedBy = null) 
        {
            NameFromEnum = inName;
            this.effectiveAgainst = effectiveAgainst;
            this.ineffectiveAgainst = ineffectiveAgainst;
            this.uselessAgainst = uselessAgainst;

            if (unfazedBy != null)
                this.unfazedBy = unfazedBy;
        }

        /// <summary>
        /// Fills in strengths and weaknesses for the static elements ONLY.
        /// </summary>
        /// <param name="effectiveAgainst">Elements that I do bonus damage to</param>
        /// <param name="ineffectiveAgainst">Elements that I do reduced damage to</param>
        /// <param name="uselessAgainst">Elements that I can't affect</param>
        public void _Initialize(Element[] effectiveAgainst, Element[] ineffectiveAgainst,
            Element[] uselessAgainst)
        {
            this.effectiveAgainst ??= effectiveAgainst;
            this.ineffectiveAgainst ??= ineffectiveAgainst;
            this.uselessAgainst ??= uselessAgainst;
        }

        public bool IsEffectiveAgainst(Element defender)
        {
            return effectiveAgainst.Contains(defender);
        }
        public bool IsIneffectiveAgainst(Element defender)
        {
            return ineffectiveAgainst.Contains(defender);
        }
        public bool IsUselessAgainst(Element defender)
        {
            return uselessAgainst.Contains(defender);
        }
        /// <summary>
        /// Only used for special elements
        /// </summary>
        public bool IsUnfazedBy(Element attacker)
        {
            return unfazedBy.Contains(attacker);
        }

    }
}
