using Capstone_Chronicles.GUI;

namespace Capstone_Chronicles
{
    static class HeroManager
    {
        public static Hero Player { get; private set; } = InitPlayer();
        public static Hero Foo { get; private set; } = InitFoo();

        public static List<Hero> Party { get; private set; } = new List<Hero> { Player, Foo };

        static Hero InitPlayer()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 40, hp: 9999, maxSp: 916, sp: 9999,
                attack: 5, defense: 0, spAttack: 3, spDefense: 0, speed: 5, exp: 0);
            var growthInfo = new Hero.StatGrowth();

            var hero = new Hero("Cyclone", stats, growthInfo);
            hero.skills = new()
            {
                SkillManager.Wind_Slash,
                SkillManager.Air_Cannon,
                SkillManager.Sonic_Boom,
                SkillManager.Slash_Storm,
                SkillManager.Sky_Crusher,
                SkillManager.Hurricane
            };

            return hero;
        }
        static Hero InitFoo()
        {
            var stats = new Actor.StatsStruct(level: 1, maxHp: 32, maxSp: 24,
                attack: 4, defense: 0, spAttack: 4, spDefense: 0, speed: 1);
            var growthInfo = new Hero.StatGrowth();

            return new Hero("Foo", stats, growthInfo);
        }
    }
}
