using Alpha;
using Alpha.ID;
using Alpha.IO;
using ImGuiNET;
using System;
using System.IO;

namespace Zeus.Engine
{
    public class PlrFileZeusEngine : IZeusEngine
    {
        private PlrFileTracker tracker;
        private ResearchedItemStatus[] researchedItemStatuses;

        private string currentPlayerName;

        public PlrFileZeusEngine()
        {
            researchedItemStatuses = new ResearchedItemStatus[ItemID.Count];
            for (int i = 0; i < researchedItemStatuses.Length; i++)
                researchedItemStatuses[i] = ResearchedItemStatus.CreateEmpty(i);

            currentPlayerName = "";

            tracker = new PlrFileTracker(
                status => researchedItemStatuses[status.Id] = status,
                () => currentPlayerName);
        }

        public ResearchedItemStatus[] GetResearchedItemStatuses() => researchedItemStatuses;

        public void PushSettingsUI()
        {
            ImGui.Text("Player to track: ");
            ImGui.SameLine();
            ImGui.InputText("", ref currentPlayerName, 32);

            if (ImGui.Button("Force update from file"))
                tracker.UpdateFromFile(currentPlayerName + ".plr",
                    Path.Combine(MiscTerrariaMethods.GetSavePath(), "Players", currentPlayerName + ".plr"));
        }

        public void Dispose()
        {
            tracker.Dispose();
        }

        private class PlrFileTracker : IDisposable
        {
            private readonly FileSystemWatcher FileTracker;
            private readonly Action<ResearchedItemStatus> ResearchedItemStatusConsumer;
            private readonly Func<string> GetCurrentlyTrackedPlayer;

            public PlrFileTracker(Action<ResearchedItemStatus> researchedItemStatusConsumer, Func<string> getCurrentlyTrackedPlayer)
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

                ResearchedItemStatusConsumer = researchedItemStatusConsumer;
                GetCurrentlyTrackedPlayer = getCurrentlyTrackedPlayer;
            }

            public void Dispose()
            {
                FileTracker.Dispose();
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
                if (parts[0].ToLower() == GetCurrentlyTrackedPlayer().ToLower() && parts[1] == "plr")
                {
                    PlayerFileData fileData = PlayerFileData.Load(path);

                    for (int i = 0; i < fileData.CreativeSacrifices.Length; i++)
                        ResearchedItemStatusConsumer(new(ItemID.Search.GetName(i), i, fileData.CreativeSacrifices[i]));
                }
            }
        }

    }
}
