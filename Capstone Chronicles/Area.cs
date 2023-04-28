using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    public static class AreaManager
    {
        public static readonly List<Area> areaList = new();
        public static Area SorecordForest { get; private set; }
            = new Area("Sorecord Forest", 6, encounters: new()
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
                    [6] = new Encounter(false, false, "SoreFore Miniboss", EnemyFactory.ElderSkoka, EnemyFactory.Skoka, EnemyFactory.Growfa)
                }, () => HeroManager.Party.AddIfUnique(HeroManager.Foo));
        public static Area SunstrokePlateau { get; private set; } 
            = new Area("Sunstroke Plateau", 10, encounters: new()
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
                    [10] = new Encounter(false, false, "SunPlat Boss", EnemyFactory.Boss_Leviac)
                }, () => HeroManager.Party.AddIfUnique(HeroManager.Tessa));
        public static Area DiagryPeak { get; private set; } 
            = new Area("Diagry Peak", 10, encounters: new()
                {
                    new Encounter(new()
                    {
                        new(EnemyFactory.ElderSkoka, 8, "Elite Skoka"){Element = ElementManager.WATER},
                        new(EnemyFactory.Plugry, 8),
                    }, (2, 3)),
                    new Encounter(new()
                    {
                        new(EnemyFactory.Flarix, 8),
                        new(EnemyFactory.Caprid, 5),
                    }, (2, 5)),
                    new Encounter(new()
                    {
                        new(EnemyFactory.Flarix, 6),
                        EnemyFactory.Epho,
                    }, (1, 4)),
                    new Encounter(new()
                    {
                        new(EnemyFactory.Terradon),
                        new(EnemyFactory.Plugry, 8),
                    }, (1, 2)),
                }, 
                fixedEncounters: new()
                {
                    [10] = new Encounter(false, false, "DiaPeak Boss", EnemyFactory.Boss_Hology)
                });

        static AreaManager()
        {
            //Set up progression
            SorecordForest.NextArea = SunstrokePlateau;
            SunstrokePlateau.NextArea = DiagryPeak;
            DiagryPeak.NextArea = null;
        }
    }

    public struct AreaData
    {
        [JsonInclude]
        public int progress;
        [JsonInclude]
        public bool started;
        [JsonInclude]
        public bool completed;

        [JsonConstructor]
        public AreaData(int progress, bool started, bool completed)
        {
            this.progress = progress;
            this.started = started;
            this.completed = completed;
        }
        public AreaData(Area area)
        {
            progress = area.Progress;
            started = area.Started;
            completed = area.Completed;
        }
    }

    public class Area
    {
        public static Area Current { get; private set; }
        public Area? NextArea { get; set; }

        public string Name { get; private set; }
        public int Length { get; private set; }

        public int Progress { get; private set; }

        private List<Encounter> encounters;
        private Dictionary<int, Encounter> fixedEncounters;

        public bool Started { get; private set; } = false;
        public bool Completed { get; private set; } = false;

        private event Action? FinishArea;

        public Area(string name, int length, List<Encounter> encounters, Dictionary<int, Encounter>? fixedEncounters,
            Action? finishAction = null)
        {
            Name = name;
            Length = length;
            this.encounters = encounters;
            this.fixedEncounters = fixedEncounters ?? new();

            FinishArea = finishAction;

            GameManager.SceneChanged += OnSceneChanged;
            GameManager.BeginSave += Save;
            GameManager.BeginLoad += Load;

            AreaManager.areaList.Add(this);
            if (AreaManager.areaList[0] == this)
                Started = true;
        }

        private void HealParty()
        {
            foreach (var hero in HeroManager.Party)
            {
                hero.SetHealth(hero.MaxHp);
                hero.SetStamina(hero.MaxSp);
                hero.statusEffects.Clear();
            }
        }

        private void OnSceneChanged(Scene cur, Scene prev)
        {
            if (cur is OverworldScene)
                Current = (cur as OverworldScene).Area;
        }

        public void ModifyProgress(int i, bool directSet = false)
        {
            Started = true;

            if (directSet)
                Progress = i;
            else
                Progress += i;

            Progress = Progress.Clamp(0, Length + 1);

            if (Progress == 0 && directSet)
                HealParty();

            //Next area
            if (Progress > Length)
            {
                if (NextArea == null)
                {
                    GameManager.ChangeScene(SceneFactory.EndingScene);
                    return;
                }

                if (!Completed)
                {
                    FinishArea?.Invoke();
                    Completed = true;
                }

                Current = NextArea;
                SceneFactory.Overworld.Area = Current;
                Current.ModifyProgress(0, true);
            }
        }

        public Encounter GetRandomEncounter()
        {
            var valid = encounters.ToList();

            valid.RemoveAll(x => x.hasBeenBeat && !x.CanBeRepeated);

            return valid.GetRandomElement();
        }

        public bool TryGetFixedEncounter(out Encounter? encounter)
        {
            fixedEncounters.TryGetValue(Progress, out encounter);

            if (encounter == null || (encounter.hasBeenBeat && !encounter.CanBeRepeated))
                return false;

            return true;
        }

        void Save(int slot)
        {
            var data = new AreaData(this);
            SaveLoad.Save(data, slot, Name);
        }

        void Load(int slot)
        {
            var data = SaveLoad.Load<AreaData>(slot, Name);

            Progress = data.progress;
            Started = data.started;
            Completed = data.completed;
        }
    }
}
