using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Contains info on how stats should grow with the actor's level
    /// </summary>
    public class StatGrowth
    {
        public enum GrowthCurve
        {
            Linear, Square, Log, SmoothExpo
        }

        private Actor self;

        (GrowthCurve curve, float scalar) hpInfo;
        (GrowthCurve curve, float scalar) spInfo;
        (GrowthCurve curve, float scalar) attackInfo;
        (GrowthCurve curve, float scalar) defenseInfo;
        (GrowthCurve curve, float scalar) specialInfo;
        (GrowthCurve curve, float scalar) speedInfo;
        (GrowthCurve curve, float scalar) expInfo;

        public StatGrowth(
            (GrowthCurve curve, float scalar) _hpInfo,
            (GrowthCurve curve, float scalar) _spInfo,
            (GrowthCurve curve, float scalar) _attackInfo,
            (GrowthCurve curve, float scalar) _defenseInfo,
            (GrowthCurve curve, float scalar) _specialInfo,
            (GrowthCurve curve, float scalar) _speedInfo,
            (GrowthCurve curve, float scalar) _expInfo)
        {
            // Assign the parameters to the corresponding fields
            hpInfo = _hpInfo;
            spInfo = _spInfo;
            attackInfo = _attackInfo;
            defenseInfo = _defenseInfo;
            specialInfo = _specialInfo;
            speedInfo = _speedInfo;
            expInfo = _expInfo;

            // Subscribe to the AfterInit event of the Hero class
            Hero.AfterInit += Init;
        }


        //Dependency injection
        private void Init(StatGrowth @this, Actor inActor)
        {
            self = inActor;
            Hero.AfterInit -= Init;
        }

        public int StatGain(Actor.StatType statType, bool showText = true)
        {
            string statName = statType.ToString().Replace('_', ' ');
            (int stat, GrowthCurve curve, float scalar) info = default;

            switch (statType)
            {
                case Actor.StatType.Max_HP:
                    info = (self.MaxHp, hpInfo.curve, hpInfo.scalar);
                    break;
                case Actor.StatType.Max_SP:
                    info = (self.MaxSp, spInfo.curve, spInfo.scalar);
                    break;
                case Actor.StatType.Attack:
                    info = (self.Attack, attackInfo.curve, attackInfo.scalar);
                    break;
                case Actor.StatType.Defense:
                    info = (self.Defense, defenseInfo.curve, defenseInfo.scalar);
                    break;
                case Actor.StatType.Special:
                    info = (self.Special, specialInfo.curve, specialInfo.scalar);
                    break;
                case Actor.StatType.Speed:
                    info = (self.Speed, speedInfo.curve, speedInfo.scalar);
                    break;
                case Actor.StatType.Exp:
                    int value;
                    if (self is Hero)
                        value = (self as Hero).NeededExp;
                    else
                        value = self.Exp;

                    info = (value, expInfo.curve, expInfo.scalar);
                    break;
            }

            //Calculate a base increase and then randomly skew it a bit
            float amount = CalculateBaseIncrease(info.curve, info.stat, info.scalar);
            float skew;//for debugging

            if (info.scalar != 0 && statType != Actor.StatType.Exp)
            {
                skew = RNG.RandomInt(-20, 20) / 10f;
                amount = MathF.Abs(MathF.Round(amount + skew));
            }

            info.stat += (int)amount;

            if (showText && amount > 0)
                Scene.print($"{self.Name}'s {statName} increased by {(int)amount}");

            return info.stat;
        }

        public float CalculateBaseIncrease(GrowthCurve growthType, int stat, float scalar)
        {
            float result = stat;

            switch (growthType)
            {
                case GrowthCurve.Linear:
                    result = stat * scalar;
                    break;
                case GrowthCurve.Square:
                    result = MathF.Sqrt(stat) * scalar;
                    break;
                case GrowthCurve.Log:
                    result = MathF.Log(stat) * scalar;
                    break;
                case GrowthCurve.SmoothExpo:
                    result = stat * scalar + MathF.Pow(stat, 2) * scalar * 0.01f + self.Level * 2;
                    break;
            }

            //Return the increase amount
            return (int) result;
        }

    }
}
