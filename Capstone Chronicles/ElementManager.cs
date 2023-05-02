using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Stores information on all <see cref="Element"/>s
    /// </summary>
    public static class ElementManager
    {
        #region All Elements
        /// <summary>
        /// Weak To: nothing |
        /// Resists: nothing
        /// </summary>
        public static Element OMNI { get; private set; } = new Element(Element.Type.OMNI);

        /// <summary>
        /// Weak To: nothing |
        /// Resists: nothing
        /// </summary>
        public static Element NORMAL { get; private set; } = new Element(Element.Type.NORMAL);

        /// <summary>
        /// Weak To: water, ground, air |
        /// Resists: fire, plant, electric
        /// </summary>
        public static Element FIRE { get; private set; } = new Element(Element.Type.FIRE);

        /// <summary>
        /// Weak To: plant, electric |
        /// Resists: water, fire
        /// </summary>
        public static Element WATER { get; private set; } = new Element(Element.Type.WATER);

        /// <summary>
        /// Weak To: fire |
        /// Resists: water, ground
        /// </summary>
        public static Element PLANT { get; private set; } = new Element(Element.Type.PLANT);

        /// <summary>
        /// Weak To: water, plant |
        /// Resists: normal, fire |
        /// Ignores: electric
        /// </summary>
        public static Element GROUND { get; private set; } = new Element(Element.Type.GROUND);

        /// <summary>
        /// Weak To: electric |
        /// Resists: air, ground |
        /// Ignores: normal
        /// </summary>
        public static Element AIR { get; private set; } = new Element(Element.Type.AIR);

        /// <summary>
        /// Weak To: ground |
        /// Resist: air, electric
        /// </summary>
        public static Element ELECTRIC { get; private set; } = new Element(Element.Type.ELECTRIC);
        #endregion

        //Static constructor to initialize the strengths and weaknesses of all above elements
        static ElementManager()
        {
            OMNI._Initialize
                (
                effectiveAgainst: new Element[] { },
                ineffectiveAgainst: new Element[] { },
                uselessAgainst: new Element[] { }
                );

            NORMAL._Initialize
                (
                effectiveAgainst: new Element[] { },
                ineffectiveAgainst: new Element[] { GROUND },
                uselessAgainst: new Element[] { AIR }
                );

            FIRE._Initialize
                (
                effectiveAgainst: new Element[] { PLANT },
                ineffectiveAgainst: new Element[] { FIRE, WATER, GROUND },
                uselessAgainst: new Element[] { }
                );

            WATER._Initialize
                (
                effectiveAgainst: new Element[] { FIRE, GROUND },
                ineffectiveAgainst: new Element[] { WATER, PLANT },
                uselessAgainst: new Element[] { }
                );

            PLANT._Initialize
                (
                effectiveAgainst: new Element[] { GROUND, WATER },
                ineffectiveAgainst: new Element[] { FIRE },
                uselessAgainst: new Element[] { }
                );

            GROUND._Initialize
                (
                effectiveAgainst: new Element[] { FIRE, ELECTRIC },
                ineffectiveAgainst: new Element[] { PLANT, AIR },
                uselessAgainst: new Element[] {  }
                );

            AIR._Initialize
                (
                effectiveAgainst: new Element[] { FIRE },
                ineffectiveAgainst: new Element[] { AIR, ELECTRIC },
                uselessAgainst: new Element[] { }
                );

            ELECTRIC._Initialize
                (
                effectiveAgainst: new Element[] { WATER, AIR },
                ineffectiveAgainst: new Element[] { ELECTRIC, FIRE },
                uselessAgainst: new Element[] { GROUND }
                );
        }

        /// <summary>
        /// Bonus damage scalar
        /// </summary>
        public const float BONUS_MULT = 1.75f;
        /// <summary>
        /// Damage reduction scalar
        /// </summary>
        public const float REDUCED_MULT = 0.5f;
        /// <summary>
        /// Extreme damage reduction scalar
        /// </summary>
        public const float USELESS_MULT = 0f;

        /// <summary>
        /// Calculates the attack damage multiplier
        /// </summary>
        /// <param name="attackElement">The element of the attack</param>
        /// <param name="target">The target</param>
        /// <returns>A float: damage multiplier</returns>
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

    /// <summary>
    /// Data on the strengths and weaknesses of special attacks and Actors
    /// </summary>
    public class Element
    {
        public enum Type
        {
            OMNI, NORMAL, FIRE, WATER, PLANT, GROUND, AIR, ELECTRIC
        }
        /// <summary>
        /// The name of the Element
        /// </summary>
        public string Name { get; private set; }


        private Element[] effectiveAgainst = null;
        private Element[] ineffectiveAgainst = null;
        private Element[] uselessAgainst = null;

        private Element[] unfazedBy = Array.Empty<Element>();

        /// <summary>
        /// Constructs a new element with strengths and weaknesses
        /// </summary>
        /// <param name="inName">The name</param>
        /// <param name="effectiveAgainst">Elements that I do bonus damage to</param>
        /// <param name="ineffectiveAgainst">Elements that I do reduced damage to</param>
        /// <param name="uselessAgainst">Elements that I can't affect</param>
        /// <param name="unfazedBy">Ensures I can't be affected by some of the basic elements. Only used for temporary elements</param>
        public Element(Type inName, Element[]? effectiveAgainst = null, Element[]? ineffectiveAgainst = null, 
            Element[]? uselessAgainst = null, Element[]? unfazedBy = null) 
        {
            Name = inName.ToString();
            this.effectiveAgainst = effectiveAgainst;
            this.ineffectiveAgainst = ineffectiveAgainst;
            this.uselessAgainst = uselessAgainst;

            if (unfazedBy != null)
                this.unfazedBy = unfazedBy;
        }

        /// <summary>
        /// Constructs a new temporary element based on an existing one
        /// </summary>
        /// <param name="baseElement">The element to copy</param>
        /// <param name="inName">The name</param>
        /// <param name="unfazedBy">Ensures I can't be affected by specific elements</param>
        /// <param name="effectiveAgainst">Elements that I do bonus damage to</param>
        /// <param name="ineffectiveAgainst">Elements that I do reduced damage to</param>
        /// <param name="uselessAgainst">Elements that I can't affect</param>
        public Element(Element baseElement, string? inName = null, Element[]? unfazedBy = null, Element[]? effectiveAgainst = null, Element[]? ineffectiveAgainst = null,
            Element[]? uselessAgainst = null)
        {
            Name = inName ?? baseElement.Name;
            this.effectiveAgainst = effectiveAgainst ?? baseElement.effectiveAgainst;
            this.ineffectiveAgainst = ineffectiveAgainst ?? baseElement.ineffectiveAgainst;
            this.uselessAgainst = uselessAgainst ?? baseElement.uselessAgainst;

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

        /// <summary>
        /// Am I effective against a defending element?
        /// </summary>
        /// <param name="defender">The defending element</param>
        /// <returns>A bool: true if effective</returns>
        public bool IsEffectiveAgainst(Element defender)
        {
            return effectiveAgainst.Contains(defender);
        }
        /// <summary>
        /// Am I ineffective against a defending element?
        /// </summary>
        /// <param name="defender">The defending element</param>
        /// <returns>A bool: true if ineffective</returns>
        public bool IsIneffectiveAgainst(Element defender)
        {
            return ineffectiveAgainst.Contains(defender);
        }
        /// <summary>
        /// Am I useless against a defending element?
        /// </summary>
        /// <param name="defender">The defending element</param>
        /// <returns>A bool: true if useless</returns>
        public bool IsUselessAgainst(Element defender)
        {
            return uselessAgainst.Contains(defender);
        }

        /// /// <summary>
        /// Am I immune to an attacking element? Only used for special elements
        /// </summary>
        /// <param name="attacker">The attacking element</param>
        /// <returns>A bool: true if immune</returns>
        public bool IsUnfazedBy(Element attacker)
        {
            return unfazedBy.Contains(attacker);
        }

    }
}
