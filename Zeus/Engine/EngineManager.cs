using Alpha;
using Alpha.ID;
using System.Linq;

namespace Zeus.Engine
{
    public static class EngineManager
    {
        public static int NonResearchableItems = 0;
        public static int[] ItemResearchNeeded;
        public static string CurrentlyTrackedPlayerName = "";
        public static bool Initialized;

        public static PlrFileTracker PlrFileTracker;

        public static ResearchedItemStatus[] ResearchedItemAmounts = new ResearchedItemStatus[ItemID.Count];

        static EngineManager()
        {
            ItemResearchNeeded = MiscTerrariaMethods.LoadSacrificeCountsNeededByItemIdFromFile();
            NonResearchableItems = ItemResearchNeeded.Count(x => x == 0);
            
            for (int i = 0; i < ItemID.Count; i++)
                ResearchedItemAmounts[i] = ResearchedItemStatus.CreateEmpty(i);
        }

        public static void InitializeModeA()
        {
            Initialized = true;
            PlrFileTracker = new();
        }
        
        public static void InitializeModeB()
        {
            Initialized = true;
        }

        public static void SubmitNewResearchStatus(ResearchedItemStatus status)
        {
            ResearchedItemAmounts[status.Id] = status;
        }
    }
}
