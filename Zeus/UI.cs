using Alpha;
using Alpha.ID;
using ImGuiNET;
using System.Linq;
using System.Numerics;
using Veldrid.Sdl2;
using Zeus.Engine;

namespace Zeus
{
    public static class UI
    {
        private static bool HideCompleted = false;
        private static bool FilterMenuOpen = false;
        private static string SearchText = "";

        private static string Filter = "None";
        
        public static unsafe void Draw()
        {
            switch (Program.Config.Data.Theme)
            {
                case "Dark": ImGui.StyleColorsDark(); break;
                case "Light": ImGui.StyleColorsLight(); break;
                default: ImGui.StyleColorsDark(); break;
            }

            Draw_ItemList();
            Draw_Settings();
        }

        private static unsafe void Draw_ItemList()
        {
            ImGui.SetNextWindowPos(new(0, 0));
            ImGui.SetNextWindowSize(new(Program.Window.Width, 400));
            ImGui.Begin("Item List", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            if (EngineManager.Initialized)
            {
                ImGui.InputText("Item search", ref SearchText, 24);

                if (ImGui.Button("Filters"))
                    FilterMenuOpen = !FilterMenuOpen;

                if (FilterMenuOpen)
                {
                    ImGui.SetNextWindowPos(new(8, 70));
                    ImGui.SetNextWindowSize(new(125, 265));
                    ImGui.Begin("Filters", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);

                    void FilterButton(string filter)
                    {
                        if (ImGui.RadioButton(filter, Filter == filter))
                            Filter = filter;
                    }

                    FilterButton("None");
                    FilterButton("Weapons");
                    FilterButton("Armor");
                    FilterButton("Vanity");
                    FilterButton("Blocks");
                    FilterButton("Furniture");
                    FilterButton("Accessories");
                    FilterButton("Equipment");
                    FilterButton("Consumables");
                    FilterButton("Tools");
                    FilterButton("Materials");
                    FilterButton("Misc");

                    ImGui.End();
                }

                ImGui.SameLine();
                ImGui.Checkbox("Hide completed", ref HideCompleted);

                ImGuiStylePtr style = ImGui.GetStyle();
                Vector4 ogButtonCol = style.Colors[(int)ImGuiCol.Button];
                Vector4 ogButtonHoveredCol = style.Colors[(int)ImGuiCol.ButtonHovered];
                Vector4 ogButtonActiveCol = style.Colors[(int)ImGuiCol.ButtonActive];

                style.Colors[(int)ImGuiCol.ButtonActive] = style.Colors[(int)ImGuiCol.ButtonHovered];
                style.Colors[(int)ImGuiCol.ButtonHovered] = style.Colors[(int)ImGuiCol.Button];
                style.Colors[(int)ImGuiCol.Button] = style.Colors[(int)ImGuiCol.ChildBg];

                ImGui.BeginListBox("", new(400, 300));
                Vector4 researchedColour = new(0f, 1f, 0f, 1f);
                for (int i = 1; i < ItemID.Count; i++)
                {
                    ResearchedItemStatus status = EngineManager.GetItemStatuses()[i];
                    string name = MiscTerrariaMethods.GetItemName(status.Id);

                    if (i == ItemID.LesserRestorationPotion || i == ItemID.FirstFractal)
                        continue;

                    if (EngineManager.ItemResearchNeeded[i] == 0)
                        continue;

                    if ((HideCompleted && status.Researched) ||
                        (Filter != "None" && !JourneyHelper.GetJourneyCategoriesForItem(status.Id)[Filter]) ||
                        (!string.IsNullOrEmpty(SearchText) && !name.ToLower().Contains(SearchText.ToLower())))
                        continue;

                    string text = $"{name}: {status.Current}/{status.Needed}";
                    if (!status.Researched)
                    {
                        if (!ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                        {
                            ImGui.Text(text);
                            continue;
                        }
                        
                        if (ImGui.SmallButton(text))
                            Utils.OpenWikiPage(name);
                    }
                    else
                    {
                        if (!ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                        {
                            ImGui.TextColored(researchedColour, text);
                            continue;
                        }

                        Vector4 ogTextCol = style.Colors[(int)ImGuiCol.Text];
                        style.Colors[(int)ImGuiCol.Text] = researchedColour;

                        if (ImGui.SmallButton(text))
                            Utils.OpenWikiPage(name);

                        style.Colors[(int)ImGuiCol.Text] = ogTextCol;
                    }
                }
                ImGui.EndListBox();

                style.Colors[(int)ImGuiCol.Button] = ogButtonCol;
                style.Colors[(int)ImGuiCol.ButtonHovered] = ogButtonHoveredCol;
                style.Colors[(int)ImGuiCol.ButtonActive] = ogButtonActiveCol;

                float amountCompleted = EngineManager.GetItemStatuses().Count(x => x.Needed != 0 && x.Researched);
                float amountNeeded = ItemID.Count - EngineManager.NonResearchableItems;
                ImGui.Text(amountCompleted + "/" + amountNeeded);
                ImGui.SameLine();
                ImGui.Text($@"({((amountCompleted / amountNeeded) * 100f):00.00}%%)");
            }
            else
                ImGui.Text("Engine not initialized.");

            ImGui.End();
        }

        private static unsafe void Draw_Settings()
        {
            ImGui.SetNextWindowPos(new(0, 400));
            ImGui.SetNextWindowSize(new(Program.Window.Width, 100));
            ImGui.Begin("Settings", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            if (!EngineManager.Initialized)
            {
                if (ImGui.Button("Initialize file reading mode."))
                    EngineManager.InitializeModeA();

                ImGui.SameLine();
                if (ImGui.Button("Initialize injected tracker mode (NOT COMPLETED)."))
                    EngineManager.InitializeModeB();
            }
            else
                EngineManager.PushSettingsUI();

            ImGui.Text("Theme:");
            ImGui.SameLine();
            if (ImGui.RadioButton("Dark", Program.Config.Data.Theme == "Dark")) Program.Config.Data.Theme = "Dark";
            ImGui.SameLine();
            if (ImGui.RadioButton("Light", Program.Config.Data.Theme == "Light")) Program.Config.Data.Theme = "Light";

            ImGui.End();
        }
    }
}
