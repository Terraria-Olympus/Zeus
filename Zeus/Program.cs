using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zeus.DataStructures;
using Zeus.Id;

namespace Zeus
{
    public static class Program
    {
        public static void Main()
        {
            Dictionary<int, int> sacrificeCountsNeededById = TerrariaThings.LoadSacrificeCountsNeededByItemIdFromFile("Sacrifices.tsv");

            string playerSavePath = Path.Combine(TerrariaThings.GetSavePath(), "Players");
            FileSystemWatcher saveFileWatcher = new(playerSavePath);
            saveFileWatcher.Filter = "*.plr";
            saveFileWatcher.IncludeSubdirectories = false;
            saveFileWatcher.EnableRaisingEvents = true;
            saveFileWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            saveFileWatcher.Changed += (system, args) => {
                if (args.ChangeType == WatcherChangeTypes.Changed)
                {
                    Console.WriteLine("Changed detected to player file " + args.Name + ". Generating new json...");
                    UpdateJsonFile(sacrificeCountsNeededById, args.FullPath);
                }
            };

            // Update files whenever the programs started as well
            foreach (string file in Directory.EnumerateFiles(playerSavePath))
            {
                if (file.Split('.').Last() != "plr")
                    continue;

                Console.WriteLine("Pre-generating json file for player @ " + file);
                UpdateJsonFile(sacrificeCountsNeededById, file);
            }

            Console.WriteLine();
            Console.WriteLine("Save file tracking is initialized; you can now safely minimize this window. Press Escape to close Zeus.");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }

        public static void UpdateJsonFile(Dictionary<int, int> sacrificeCountsNeededById, string playerFilePath)
        {
            PlayerFileData file = PlayerFileData.Load(playerFilePath);

            List<JourneyResearchJsonObj> objects = new();
            foreach (var info in file.CreativeSacs)
            {
                int itemId = ItemID.Search.GetId(info.Key);
                int needed = sacrificeCountsNeededById[itemId];
                JourneyResearchJsonObj obj = new(info.Key, itemId, info.Amount, needed, info.Amount >= needed);

                objects.Add(obj);
            }

            string jsonFileDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlayerSacrifices");
            if (!Directory.Exists(jsonFileDirectory))
                Directory.CreateDirectory(jsonFileDirectory);

            File.WriteAllText(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlayerSacrifices", file.Name + "_JourneySacrifices.json"),
                JsonConvert.SerializeObject(objects, Formatting.Indented));
        }
    }
}
