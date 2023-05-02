using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Contains data on all Areas
    /// </summary>
    public static class AreaManager
    {
        /// <summary>
        /// A list of all <see cref="Area"/>s in the game
        /// </summary>
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

    /// <summary>
    /// Condensed <see cref="Area"/> data for saving and loading
    /// </summary>
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
        /// <summary>
        /// Constructs AreaData from an <see cref="Area"/> object
        /// </summary>
        /// <param name="area">The Area object</param>
        public AreaData(Area area)
        {
            progress = area.Progress;
            started = area.Started;
            completed = area.Completed;
        }
    }

    /// <summary>
    /// Contains data on an in-game location
    /// </summary>
    public class Area
    {
        /// <summary>
        /// The Area that you are currently in
        /// </summary>
        public static Area Current { get; private set; }
        /// <summary>
        /// The area to load when you finish this one
        /// </summary>
        public Area? NextArea { get; set; }

        /// <summary>
        /// The name of the area
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// How many encounters are in the area
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// How close are you to the end of the area
        /// </summary>
        public int Progress { get; private set; }

        private List<Encounter> encounters;
        private Dictionary<int, Encounter> fixedEncounters;

        /// <summary>
        /// Have you reached this area?
        /// </summary>
        public bool Started { get; private set; } = false;
        /// <summary>
        /// Have you completed this area?
        /// </summary>
        public bool Completed { get; private set; } = false;

        private event Action? FinishArea;

        /// <summary>
        /// Initializes a new instance of the <see cref="Area"/> class.
        /// </summary>
        /// <param name="name">The name of the area</param>
        /// <param name="length">The amount of encounters</param>
        /// <param name="encounters">The types of encounters</param>
        /// <param name="fixedEncounters">The fixed encounters</param>
        /// <param name="finishAction">The action to trigger when finishing the area</param>
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
                hero.FullRestore();
            }
        }

        private void OnSceneChanged(Scene cur, Scene prev)
        {
            if (cur is OverworldScene)
                Current = (cur as OverworldScene).Area;
        }

        /// <summary>
        /// Make or lose progress
        /// </summary>
        /// <param name="i">The amount to change by</param>
        /// <param name="directSet">If true, set progress to <paramref name="i"/></param>
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

        /// <summary>
        /// Randomly gets an encounter from the list
        /// </summary>
        /// <returns>An Encounter</returns>
        public Encounter GetRandomEncounter()
        {
            var valid = encounters.ToList();

            valid.RemoveAll(x => x.hasBeenBeat && !x.CanBeRepeated);

            return valid.GetRandomElement();
        }

        /// <summary>
        /// Tries the get a fixed encounter based on your progress
        /// </summary>
        /// <param name="encounter">The fixed encounter</param>
        /// <returns>A bool: true if successful</returns>
        public bool TryGetFixedEncounter(out Encounter? encounter)
        {
            fixedEncounters.TryGetValue(Progress, out encounter);

            if (encounter == null || (encounter.hasBeenBeat && !encounter.CanBeRepeated))
                return false;

            return true;
        }

        private void Save(int slot)
        {
            var data = new AreaData(this);
            SaveLoad.Save(data, slot, Name);
        }

        private void Load(int slot)
        {
            var data = SaveLoad.Load<AreaData>(slot, Name);

            Progress = data.progress;
            Started = data.started;
            Completed = data.completed;
        }
    }
}
