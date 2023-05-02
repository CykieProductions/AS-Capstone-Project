using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Contains all <see cref="Hero"/> objects and related information
    /// </summary>
    internal static class HeroManager
    {
        private const string SAVE_KEY = "CURRENT_PARTY";
        public const string PLAYER_SAVE_KEY = "PLAYER_CHARACTER";

        public static Hero Player { get; private set; } = InitPlayer();//Air + Electric
        public static Hero Rezy { get; private set; } = InitRezy();//Plant + Water
        public static Hero Foo { get; private set; } = InitFoo();//Fire + Electric
        public static Hero Tessa { get; private set; } = InitTessa();//Water + Ground

        public static List<Hero> Party { get; private set; } = new List<Hero> { Player, Rezy };

        static HeroManager()
        {
            GameManager.BeginSave += Save;
            GameManager.BeginLoad += Load;
        }

        private static void Save(int slot)
        {
            List<string> heroesInParty = new();
            heroesInParty.Add("PLAYER");

            for (int i = 1; i < Party.Count; i++)
            {
                heroesInParty.Add(Party[i].Name);
            }

            SaveLoad.Save(heroesInParty, slot, SAVE_KEY);
        }
        private static void Load(int slot)
        {
            List<string> heroesInParty = SaveLoad.Load<List<string>>(slot, SAVE_KEY);
            if (heroesInParty == null)
                return;

            Party.Clear();
            Party.Add(Player);

            if (heroesInParty.Contains(Rezy.Name))
                Party.AddIfUnique(Rezy);

            if (heroesInParty.Contains(Foo.Name))
                Party.AddIfUnique(Foo);

            if (heroesInParty.Contains(Tessa.Name))
                Party.AddIfUnique(Tessa);
        }

        static Hero InitPlayer()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 40, hp: 9999, maxSp: 16, sp: 9999,
                attack: 5, defense: 0, special: 3, speed: 5, exp: 0);
            var growthInfo = new StatGrowth(
                _hpInfo: (StatGrowth.GrowthCurve.Log, 1.5f),
                _spInfo: (StatGrowth.GrowthCurve.Square, 1.2f),
                _attackInfo: (StatGrowth.GrowthCurve.Square, .9f),
                _defenseInfo: (StatGrowth.GrowthCurve.Linear, .1f),
                _specialInfo: (StatGrowth.GrowthCurve.Square, 1),
                _speedInfo: (StatGrowth.GrowthCurve.Linear, 0),
                _expInfo: (StatGrowth.GrowthCurve.Linear, 0.6f)
                );

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [2] = SkillManager.Healing_Powder,
                [4] = SkillManager.L1H1_Wind_Slash,
                [5] = SkillManager.Curing_Powder,
                [8] = SkillManager.Poison_Powder,

                [9] = SkillManager.L1Ha_Slash_Storm,
                [10] = SkillManager.Super_Healing_Powder,
                [11] = SkillManager.Phoenix_Powder,
                [12] = SkillManager.L1H1_Charge_Bolt,

                [13] = SkillManager.Healing_Cloud,
                [14] = SkillManager.L2H1_Air_Cannon,
                [15] = SkillManager.L2Ha_Sky_Crusher,
                [16] = SkillManager.L2H1_Taser_Grip,

                [18] = SkillManager.Curing_Cloud,
                [20] = SkillManager.Poison_Cloud,
                [22] = SkillManager.Ultra_Healing_Powder,
                [25] = SkillManager.L3H1_Sonic_Boom,
                [28] = SkillManager.L3Ha_Hurricane,
                [32] = SkillManager.Ultra_Healing_Cloud
            };

            var hero = new Hero("Cyclone", stats, growthInfo, skillDict, saveKey: PLAYER_SAVE_KEY);

            return hero;
        }

        static Hero InitRezy()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 36, maxSp: 24,
                attack: 2, defense: 0, special: 4, speed: 6, exp: 0);
            var growthInfo = new StatGrowth(
                _hpInfo: (StatGrowth.GrowthCurve.Log, 1.1f),
                _spInfo: (StatGrowth.GrowthCurve.Square, 1.5f),
                _attackInfo: (StatGrowth.GrowthCurve.Square, .8f),
                _defenseInfo: (StatGrowth.GrowthCurve.Linear, 0.2f),
                _specialInfo: (StatGrowth.GrowthCurve.Log, 1f),
                _speedInfo: (StatGrowth.GrowthCurve.Linear, 0.2f),
                _expInfo: (StatGrowth.GrowthCurve.Linear, 0.6f)
                );

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [1] = SkillManager.L1H1_Vine_Whip,
                [3] = SkillManager.Healing_Powder,
                [5] = SkillManager.L1Ha_Leaf_Storm,
                [6] = SkillManager.Poison_Powder,
                [8] = SkillManager.L1H1_Water_Pulse,
                [9] = SkillManager.Poison_Cloud,
                [10] = SkillManager.L2H1_Leaf_Cutter,
                [12] = SkillManager.L2H1_Water_Jet,
                [14] = SkillManager.Healing_Cloud,
                [15] = SkillManager.Super_Healing_Powder,
                [16] = SkillManager.L2Ha_Root_Wave,
                [24] = SkillManager.L3H1_Needle_Tomb,
                [32] = SkillManager.L3Ha_Thorn_Canopy,
            };

            var hero = new Hero("Rezy", stats, growthInfo, skillDict, ElementManager.PLANT);
            
            return hero;
        }
        
        static Hero InitFoo()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 32, maxSp: 24,
                attack: 4, defense: 0, special: 4, speed: 1, exp: 120);
            var growthInfo = new StatGrowth(
                _hpInfo: (StatGrowth.GrowthCurve.Log, 1.3f),
                _spInfo: (StatGrowth.GrowthCurve.Log, 1.2f),
                _attackInfo: (StatGrowth.GrowthCurve.Square, .8f),
                _defenseInfo: (StatGrowth.GrowthCurve.Linear, 0.3f),
                _specialInfo: (StatGrowth.GrowthCurve.Square, .8f),
                _speedInfo: (StatGrowth.GrowthCurve.Linear, 0.1f),
                _expInfo: (StatGrowth.GrowthCurve.Linear, 0.55f)
                );

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [2] = SkillManager.L1H1_Fireball,
                [4] = SkillManager.Healing_Powder,
                [6] = SkillManager.L1Ha_Flare_Fall,
                [10] = SkillManager.L1Ha_Electro_Wave,
                [12] = SkillManager.L2H1_Flame_Burst,
                [13] = SkillManager.L2Ha_Tesla_Cannon,
                [14] = SkillManager.Super_Healing_Powder,
                [15] = SkillManager.L2Ha_Blazing_Vortex,
                [16] = SkillManager.Healing_Cloud,
                [28] = SkillManager.L3H1_Eruption,
                [32] = SkillManager.L3Ha_Supernova,
            };

            var hero = new Hero("Foo", stats, growthInfo, skillDict, ElementManager.NORMAL);

            return hero;
        }

        static Hero InitTessa()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 36, maxSp: 24,
                attack: 2, defense: 0, special: 5, speed: 3, exp: 600);
            var growthInfo = new StatGrowth(
                _hpInfo: (StatGrowth.GrowthCurve.Log, 1.5f),
                _spInfo: (StatGrowth.GrowthCurve.Square, .9f),
                _attackInfo: (StatGrowth.GrowthCurve.Square, 1.1f),
                _defenseInfo: (StatGrowth.GrowthCurve.Linear, 0.2f),
                _specialInfo: (StatGrowth.GrowthCurve.Square, .8f),
                _speedInfo: (StatGrowth.GrowthCurve.Linear, 0.1f),
                _expInfo: (StatGrowth.GrowthCurve.Linear, 0.5f)
                );

            var skillDict = new Dictionary<int, SkillBase>()
            {
                [1] = SkillManager.L1H1_Water_Pulse,
                [3] = SkillManager.Healing_Powder,
                [8] = SkillManager.L1Ha_Waterfall,
                [10] = SkillManager.L2H1_Geo_Shift,
                [11] = SkillManager.L1Ha_Rock_Slide,
                [13] = SkillManager.L1H1_Water_Pulse,
                [16] = SkillManager.Super_Healing_Powder,
                [18] = SkillManager.L2H1_Water_Jet,
                [20] = SkillManager.L2Ha_Spire_Wall,
                [22] = SkillManager.Phoenix_Powder,
                [25] = SkillManager.L2Ha_Deluge,
                [32] = SkillManager.L3H1_Fissure,
                [38] = SkillManager.L3H1_Hydo_Cannon,
                [42] = SkillManager.L3Ha_Tsunami,
            };

            var hero = new Hero("Tessa", stats, growthInfo, skillDict, ElementManager.NORMAL);

            return hero;
        }
    }
}
