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
                attack: 5, defense: 0, spAttack: 3, spDefense: 0, speed: 5, exp: 0);
            var growthInfo = new Hero.StatGrowth();

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

                [16] = SkillManager.Charge_Bolt,
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

            //TODO remove
            hero.skills = new()
            {
                SkillManager.Wind_Slash,
                SkillManager.Healing_Powder,
                SkillManager.Curing_Powder,
                SkillManager.Healing_Cloud,
                SkillManager.Air_Cannon,
                SkillManager.Sonic_Boom,
                SkillManager.Slash_Storm,
                SkillManager.Sky_Crusher,
                SkillManager.Hurricane,
                SkillManager.Taser_Grip
            };

            return hero;
        }

        static Hero InitRezy()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 36, maxSp: 9_24,
                attack: 2, defense: 0, spAttack: 4, spDefense: 0, speed: 6);
            var growthInfo = new Hero.StatGrowth();

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
                attack: 4, defense: 0, spAttack: 4, spDefense: 0, speed: 1);
            var growthInfo = new Hero.StatGrowth();

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [3] = SkillManager.Fireball,
                [7] = SkillManager.Healing_Powder,
                [12] = SkillManager.Flare_Fall,
                [16] = SkillManager.Flame_Burst,
                [25] = SkillManager.Blazing_Vortex,
                [34] = SkillManager.Eruption,
                [42] = SkillManager.Supernova,
            };

            var hero = new Hero("Foo", stats, growthInfo, skillDict);

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

        static Hero InitTessa()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 36, maxSp: 9_24,
                attack: 2, defense: 0, spAttack: 5, spDefense: 0, speed: 3);
            var growthInfo = new Hero.StatGrowth();

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

            var hero = new Hero("Tessa", stats, growthInfo, skillDict, ElementManager.PLANT);

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
