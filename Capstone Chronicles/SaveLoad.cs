using System.Text.Json;

namespace Capstone_Chronicles
{
    /// <summary>
    /// Used for Saving and Loading game data
    /// </summary>
    public static class SaveLoad
    {
        /// <summary>
        /// The program's persistent data path
        /// </summary>
        public static string PersistentDataPath { get; private set; } = 
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/{Program.Name}/";//Windows will correct "/" to "\" 
        /// <summary>
        /// The max number of slots you can save to
        /// </summary>
        public const int MAX_SAVE_SLOTS = 8;

        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
        };

        //Init
        static SaveLoad()
        {
            var d = Directory.CreateDirectory(PersistentDataPath);
            PersistentDataPath = d.FullName;//Corrects slashes
        }

        /// <summary>
        /// Checks if the save data exists
        /// </summary>
        /// <param name="slotNum">The slot number</param>
        /// <param name="key">The unique save key</param>
        /// <param name="extension">The file extension</param>
        /// <returns>A bool: true if successful</returns>
        public static bool SaveExists(int slotNum, string key, string extension = ".sav")
        {
            bool successful;

            if (extension != "")
                successful = File.Exists(PersistentDataPath + "Saves\\" + $"Slot {slotNum}\\" + key + extension)
                    || File.Exists(PersistentDataPath + "Saves/" + $"Slot {slotNum}/" + key + extension);
            else
                successful = Directory.Exists(PersistentDataPath + "Saves\\" + $"Slot {slotNum}\\" + key)
                    || Directory.Exists(PersistentDataPath + "Saves/" + $"Slot {slotNum}/" + key);

            return successful;
        }

        /// <summary>
        /// Gets the slot numbers currently in use
        /// </summary>
        /// <returns>A list of int: slot numbers</returns>
        public static List<int> GetUsedSaveSlots()
        {
            List<int> output = new();
            for (int i = 1; i <= MAX_SAVE_SLOTS; i++)
            {
                if (Directory.Exists(PersistentDataPath + "Saves/Slot " + i))
                {
                    output.Add(i);
                }
            }

            return output;
        }

        /// <summary>
        /// Saves the object into a file
        /// </summary>
        /// <param name="objToSave">The object to save</param>
        /// <param name="slotNum">The slot number</param>
        /// <param name="key">The unique save key</param>
        /// <param name="extension">The file extension</param>
        public static void Save<T>(T objToSave, int slotNum, string key, string extension = ".sav")
        {
            string path = PersistentDataPath + "Saves/" + $"Slot {slotNum}/";
            Directory.CreateDirectory(path);

            string data = JsonSerializer.Serialize(objToSave, options);

            using (StreamWriter writer = new StreamWriter(path + key + extension))
            {
                writer.WriteLine(data);
            }
            Program.print("Saved " + key + extension + " to " + path);
        }

        /// <summary>
        /// Loads an object from a file
        /// </summary>
        /// <param name="objToSave">The object to save</param>
        /// <param name="slotNum">The slot number</param>
        /// <param name="key">The unique save key</param>
        /// <param name="extension">The file extension</param>
        public static T Load<T>(int slotNum, string key, string extension = ".sav")
        {
            if (!SaveExists(slotNum, key, extension))
                return default(T);

            string path = PersistentDataPath + "Saves/" + $"Slot {slotNum}/";
            string data = "";

            using (StreamReader reader = new StreamReader(path + key + extension))
            {
                data = reader.ReadToEnd();
            }

            Program.print("Loaded " + key + extension + " from " + path);

            var result = JsonSerializer.Deserialize<T>(data, options);

            if (result != null)
                return result;
            else
                throw new NullReferenceException("The data failed to load!");
                //JsonConvert.DeserializeObject<T>(data);
        }
    }
}
