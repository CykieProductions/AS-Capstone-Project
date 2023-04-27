using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    static class HeroManager
    {
        public static Hero Player { get; private set; } = InitPlayer();//Air + Electric
        public static Hero Rezy { get; private set; } = InitRezy();//Plant + Water
        public static Hero Foo { get; private set; } = InitFoo();//Fire + Electric
        public static Hero Tessa { get; private set; } = InitTessa();//Water + Ground

        public static List<Hero> Party { get; private set; } = new List<Hero> { Player, Rezy, Foo, Tessa };

        static Hero InitPlayer()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 40, hp: 9999, maxSp: 9_16, sp: 9999,
                attack: 5, defense: 0, special: 3, speed: 5, exp: 1000000);
            var growthInfo = new StatGrowth(
                _hpInfo: (StatGrowth.GrowthCurve.Log, 1.5f),
                _spInfo: (StatGrowth.GrowthCurve.Square, 1f),
                _attackInfo: (StatGrowth.GrowthCurve.Square, 1f),
                _defenseInfo: (StatGrowth.GrowthCurve.Linear, .1f),
                _specialInfo: (StatGrowth.GrowthCurve.Square, 1),
                _speedInfo: (StatGrowth.GrowthCurve.Linear, 0),
                _expInfo: (StatGrowth.GrowthCurve.Linear, 0.6f)
                );

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [0] = SkillManager.Scan_Lash,
                [2] = SkillManager.Healing_Powder,
                [4] = SkillManager.Wind_Slash,
                [6] = SkillManager.Curing_Powder,
                [10] = SkillManager.Poison_Powder,

                [11] = SkillManager.Slash_Storm,
                [12] = SkillManager.Super_Healing_Powder,
                [15] = SkillManager.Curing_Cloud,

                [16] = SkillManager.L1H1_Charge_Bolt,
                [17] = SkillManager.Air_Cannon,
                [18] = SkillManager.Phoenix_Powder,
                [20] = SkillManager.Sky_Crusher,

                [22] = SkillManager.Healing_Cloud,
                [25] = SkillManager.Poison_Cloud,
                [30] = SkillManager.Ultra_Healing_Powder,
                [32] = SkillManager.Sonic_Boom,
                [38] = SkillManager.Hurricane,
                [40] = SkillManager.Ultra_Healing_Cloud
            };

            var hero = new Hero("Cyclone", stats, growthInfo, skillDict);

#if DEBUG
            hero.skills = new()
            {
                SkillManager.Super_Healing_Powder,
                SkillManager.Healing_Cloud,
                SkillManager.Curing_Powder,
                SkillManager.Curing_Cloud,
                SkillManager.Phoenix_Powder,
                SkillManager.Air_Cannon,
                SkillManager.Sonic_Boom,
                SkillManager.Slash_Storm,
                SkillManager.Sky_Crusher,
                SkillManager.Hurricane,
                SkillManager.L2H1_Taser_Grip
            };
#endif

            return hero;
        }

        static Hero InitRezy()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 36, maxSp: 9_24,
                attack: 2, defense: 0, special: 4, speed: 6);
            var growthInfo = new StatGrowth(
                _hpInfo: (StatGrowth.GrowthCurve.Log, 1.2f),
                _spInfo: (StatGrowth.GrowthCurve.Square, .8f),
                _attackInfo: (StatGrowth.GrowthCurve.Square, .8f),
                _defenseInfo: (StatGrowth.GrowthCurve.Linear, 0.5f),
                _specialInfo: (StatGrowth.GrowthCurve.Log, 1f),
                _speedInfo: (StatGrowth.GrowthCurve.Linear, 0f),
                _expInfo: (StatGrowth.GrowthCurve.SmoothExpo, 0.6f)
                );

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [1] = SkillManager.Vine_Whip,
                [3] = SkillManager.Healing_Powder,
                [8] = SkillManager.Leaf_Storm,
                [10] = SkillManager.Poison_Powder,
                [13] = SkillManager.Water_Pulse,
                [15] = SkillManager.Poison_Cloud,
                [16] = SkillManager.Leaf_Cutter,
                [20] = SkillManager.Water_Jet,
                [22] = SkillManager.Healing_Cloud,
                [25] = SkillManager.Root_Wave,
                [34] = SkillManager.Needle_Tomb,
                [42] = SkillManager.Thorn_Canopy,
            };

            var hero = new Hero("Rezy", stats, growthInfo, skillDict, ElementManager.PLANT);

            //TODO remove
            /*hero.skills = new()
            {
                SkillManager.Damage_Allies_Test,
                SkillManager.Ignite_Allies_Test,
                SkillManager.Poison_Allies_Test,
                SkillManager.Restore_SP_Allies_Test,
                SkillManager.Fireball,
                SkillManager.Flame_Burst,
                SkillManager.Eruption,
                SkillManager.Flare_Fall,
                SkillManager.Blazing_Vortex,
                SkillManager.Supernova
            };//*/
            
            return hero;
        }
        
        static Hero InitFoo()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 32, maxSp: 9_24,
                attack: 4, defense: 0, special: 4, speed: 1);
            var growthInfo = new StatGrowth(
                _hpInfo: (StatGrowth.GrowthCurve.Log, 1.1f),
                _spInfo: (StatGrowth.GrowthCurve.Log, 1),
                _attackInfo: (StatGrowth.GrowthCurve.Square, 1),
                _defenseInfo: (StatGrowth.GrowthCurve.Linear, 0.5f),
                _specialInfo: (StatGrowth.GrowthCurve.Square, .8f),
                _speedInfo: (StatGrowth.GrowthCurve.Linear, 0),
                _expInfo: (StatGrowth.GrowthCurve.SmoothExpo, 0.6f)
                );

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [3] = SkillManager.L1H1_Fireball,
                [7] = SkillManager.Healing_Powder,
                [12] = SkillManager.L1Ha_Flare_Fall,
                [16] = SkillManager.L2H1_Flame_Burst,
                [25] = SkillManager.L2Ha_Blazing_Vortex,
                [34] = SkillManager.L3H1_Eruption,
                [42] = SkillManager.L3Ha_Supernova,
            };

            var hero = new Hero("Foo", stats, growthInfo, skillDict, ElementManager.FIRE);

            //TODO remove
            hero.skills = new()
            {
                SkillManager.Damage_Allies_Test,
                SkillManager.Ignite_Allies_Test,
                SkillManager.Poison_Allies_Test,
                SkillManager.Restore_SP_Allies_Test,
                SkillManager.L1H1_Fireball,
                SkillManager.L2H1_Flame_Burst,
                SkillManager.L3H1_Eruption,
                SkillManager.L1Ha_Flare_Fall,
                SkillManager.L2Ha_Blazing_Vortex,
                SkillManager.L3Ha_Supernova
            };//*/
            
            return hero;
        }

        static Hero InitTessa()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 36, maxSp: 9_24,
                attack: 2, defense: 0, special: 5, speed: 3);
            var growthInfo = new StatGrowth(
                _hpInfo: (StatGrowth.GrowthCurve.Log, 1),
                _spInfo: (StatGrowth.GrowthCurve.Square, 1),
                _attackInfo: (StatGrowth.GrowthCurve.Square, 1.2f),
                _defenseInfo: (StatGrowth.GrowthCurve.Linear, 0.5f),
                _specialInfo: (StatGrowth.GrowthCurve.Square, .8f),
                _speedInfo: (StatGrowth.GrowthCurve.Linear, 0),
                _expInfo: (StatGrowth.GrowthCurve.SmoothExpo, 0.55f)
                );

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [1] = SkillManager.Water_Pulse,
                [3] = SkillManager.Healing_Powder,
                [8] = SkillManager.Waterfall,
                [10] = SkillManager.Geo_Shift,
                [11] = SkillManager.Rock_Slide,
                [13] = SkillManager.Water_Pulse,
                [16] = SkillManager.Super_Healing_Powder,
                [18] = SkillManager.Water_Jet,
                [20] = SkillManager.Spire_Wall,
                [22] = SkillManager.Phoenix_Powder,
                [25] = SkillManager.Deluge,
                [32] = SkillManager.Fissure,
                [38] = SkillManager.Hydo_Cannon,
                [42] = SkillManager.Tsunami,
            };

            var hero = new Hero("Tessa", stats, growthInfo, skillDict, ElementManager.NORMAL);

            //TODO remove
            /*hero.skills = new()
            {
                SkillManager.Damage_Allies_Test,
                SkillManager.Ignite_Allies_Test,
                SkillManager.Poison_Allies_Test,
                SkillManager.Restore_SP_Allies_Test,
                SkillManager.Fireball,
                SkillManager.Flame_Burst,
                SkillManager.Eruption,
                SkillManager.Flare_Fall,
                SkillManager.Blazing_Vortex,
                SkillManager.Supernova
            };//*/

            return hero;
        }
    }
}
