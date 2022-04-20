using Alpha;
using Alpha.ID;
using Alpha.IO;
using System.IO;

namespace Zeus.Engine
{
    public class PlrFileTracker
    {
        public FileSystemWatcher FileTracker;

        public PlrFileTracker()
        {
            FileTracker = new(Path.Combine(MiscTerrariaMethods.GetSavePath(), "Players"));
            FileTracker.Filter = "*.plr";
            FileTracker.IncludeSubdirectories = false;
            FileTracker.EnableRaisingEvents = true;
            FileTracker.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            FileTracker.Changed += FileTracker_Changed;
        }

        public void FileTracker_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
                UpdateFromFile(e.Name, e.FullPath);
        }

        public void UpdateFromFile(string file, string path)
        {
            if (!File.Exists(path))
                return;
            
            string[] parts = file.Split('.');
            if (parts[0].ToLower() == EngineManager.CurrentlyTrackedPlayerName.ToLower() && parts[1] == "plr")
            {
                PlayerFileData fileData = PlayerFileData.Load(path);

                for (int i = 0; i < fileData.CreativeSacrifices.Length; i++)
                {
                    if (EngineManager.ResearchedItemAmounts[i].Current == fileData.CreativeSacrifices[i])
                        continue;

                    EngineManager.SubmitNewResearchStatus(
                        new ResearchedItemStatus(ItemID.Search.GetName(i), i, fileData.CreativeSacrifices[i]));
                }
            }
        }
    }
}
