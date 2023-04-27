using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    public static class AreaManager
    {
        public static Area SorecordForest { get; private set; }
            = new Area("Sorecord Forest", 10, encounters: new()
                {
                    new Encounter(new()
                    {
                        EnemyFactory.Growfa,
                    }, (1, 4)),
                    new Encounter(new()
                    {
                        EnemyFactory.Skoka,
                        EnemyFactory.Growfa,
                    }, (2, 3)),
                    new Encounter(new()
                    {
                        EnemyFactory.Caprid,
                        EnemyFactory.Growfa,
                    }, (1, 2)),
                }, 
                fixedEncounters: new()
                {
                    [10] = new Encounter(false, EnemyFactory.ElderSkoka, EnemyFactory.Skoka, EnemyFactory.Growfa)
                });
        public static Area SunstrokePlateau { get; private set; } 
            = new Area("Sunstroke Plateau", 12, encounters: new()
                {
                    new Encounter(new()
                    {
                        new(EnemyFactory.ElderSkoka),
                        new(EnemyFactory.Skoka, 5),
                    }, (2, 3)),
                    new Encounter(new()
                    {
                        new(EnemyFactory.Skoka, 6),
                        new(EnemyFactory.Growfa, 6),
                    }, (2, 5)),
                    new Encounter(new()
                    {
                        EnemyFactory.Flarix,
                        EnemyFactory.Plugry,
                    }, (1, 3)),
                    new Encounter(new()
                    {
                        new(EnemyFactory.Terradon, 3),
                        EnemyFactory.Plugry,
                    }, (1, 2)),
                }, 
                fixedEncounters: new()
                {
                    [12] = new Encounter(false, EnemyFactory.Boss_Leviac)
                });

        static AreaManager()
        {
            //Set up progression
            SorecordForest.NextArea = SunstrokePlateau;
            SunstrokePlateau.NextArea = SunstrokePlateau;
        }
    }

    public class Area
    {
        public static Area Current { get; private set; }
        public Area NextArea { get; set; }

        public string Name { get; private set; }
        public int Length { get; private set; }
        public int Progress { get; private set; }

        private List<Encounter> encounters;
        private Dictionary<int, Encounter> fixedEncounters;

        public Area(string name, int length, List<Encounter> encounters, Dictionary<int, Encounter>? fixedEncounters)
        {
            Name = name;
            Length = length;
            this.encounters = encounters;
            this.fixedEncounters = fixedEncounters ?? new();

            GameManager.SceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene cur, Scene prev)
        {
            if (cur is OverworldScene)
                Current = (cur as OverworldScene).AreaInfo;
        }

        public void ModifyProgress(int i, bool directSet = false)
        {
            Progress += i;
            Progress = Progress.Clamp(0, Length + 1);

            //Next area
            if (Progress > Length)
            {
                SceneFactory.Overworld.AreaInfo = NextArea;
            }
        }

        public Encounter GetRandomEncounter()
        {
            return encounters.GetRandomElement();
        }

        public bool TryGetFixedEncounter(out Encounter? encounter)
        {
            fixedEncounters.TryGetValue(Progress, out encounter);

            if (encounter == null)
                return false;

            return true;
        }

    }
}
