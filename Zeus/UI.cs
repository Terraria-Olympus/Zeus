using Alpha;
using Alpha.ID;
using ImGuiNET;
using System.IO;
using System.Linq;
using Zeus.Engine;

namespace Zeus
{
    public static class UI
    {
        private static bool HideCompleted = true;
        
        public static unsafe void Draw()
        {
            switch (Program.Config.Data.Theme)
            {
                case "Dark": ImGui.StyleColorsDark(); break;
                case "Light": ImGui.StyleColorsLight(); break;
                default: ImGui.StyleColorsDark(); break;
            }

            ImGui.SetNextWindowPos(new(0, 0));
            ImGui.SetNextWindowSize(new(Program.Window.Width, 400));
            ImGui.Begin("Item Checklist", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            if (EngineManager.Initialized)
            {
                string searchText = "";
                ImGui.InputText("Item search", ref searchText, 24);
                ImGui.Checkbox("Hide completed", ref HideCompleted);

                ImGui.BeginListBox("", new(400, 300));
                for (int i = 0; i < ItemID.Count; i++)
                {
                    ResearchedItemStatus status = EngineManager.ResearchedItemAmounts[i];
                    string name = MiscTerrariaMethods.GetItemName(status.Id);

                    if ((HideCompleted && status.Researched) ||
                        (!string.IsNullOrEmpty(searchText) && !name.ToLower().Contains(searchText.ToLower())))
                        continue;

                    ImGui.Text($"{name}: {status.Current}/{status.Needed}");
                }
                ImGui.EndListBox();

                float amountCompleted = EngineManager.ResearchedItemAmounts.Count(x => x.Needed != 0 && x.Researched);
                float amountNeeded = ItemID.Count - EngineManager.NonResearchableItems;
                ImGui.Text(amountCompleted + "/" + amountNeeded);
                ImGui.SameLine();
                ImGui.Text($@"({(amountCompleted / amountNeeded):00.00}%%)");
            }
            else
                ImGui.Text("Engine not initialized.");

            ImGui.End();

            ImGui.SetNextWindowPos(new(0, 400));
            ImGui.SetNextWindowSize(new(Program.Window.Width, 100));
            ImGui.Begin("Settings", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            if (EngineManager.PlrFileTracker != null)
            {
                ImGui.Text("Player to track: ");
                ImGui.SameLine();
                ImGui.InputText("", ref EngineManager.CurrentlyTrackedPlayerName, 32);

                if (ImGui.Button("Force update from file"))
                    EngineManager.PlrFileTracker.UpdateFromFile(EngineManager.CurrentlyTrackedPlayerName + ".plr",
                        Path.Combine(MiscTerrariaMethods.GetSavePath(), "Players", EngineManager.CurrentlyTrackedPlayerName + ".plr"));
            }

            ImGui.Text("Theme:");
            ImGui.SameLine();
            if (ImGui.RadioButton("Dark", Program.Config.Data.Theme == "Dark")) Program.Config.Data.Theme = "Dark";
            ImGui.SameLine();
            if (ImGui.RadioButton("Light", Program.Config.Data.Theme == "Light")) Program.Config.Data.Theme = "Light";

            if (!EngineManager.Initialized)
            {
                if (ImGui.Button("Initialize Mode A."))
                    EngineManager.InitializeModeA();

                ImGui.SameLine();
                if (ImGui.Button("Initialize Mode B (WIP)."))
                    EngineManager.InitializeModeB();
            }

            ImGui.End();
        }

    }
}
