using Alpha;
using System.Linq;

namespace Zeus.Engine
{
    public static class EngineManager
    {
        public static int NonResearchableItems = 0;
        public static int[] ItemResearchNeeded;
        public static bool Initialized;

        private static IZeusEngine Engine;

        public static ResearchedItemStatus[] GetItemStatuses() => Engine.GetResearchedItemStatuses();
        public static ResearchedItemStatus GetItemStatus(int id) => Engine.GetResearchedItemStatuses()[id];
        public static void PushSettingsUI() => Engine.PushSettingsUI();
        public static void Dispose() => Engine.Dispose();

        private static void InitializeEither()
        {
            ItemResearchNeeded = JourneyHelper.LoadSacrificeCountsNeededByItemIdFromFile();
            NonResearchableItems = ItemResearchNeeded.Count(x => x == 0) + 2; // Hardcoded +2 for first fractal and lesser restoration potion
        }

        public static void InitializeModeA()
        {
            InitializeEither();
            Engine = new PlrFileZeusEngine();

            Initialized = true;
        }
        
        public static void InitializeModeB()
        {
            InitializeEither();

            Initialized = true;
        }
    }
}
