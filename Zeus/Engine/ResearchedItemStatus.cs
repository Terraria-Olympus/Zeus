using Alpha.ID;

namespace Zeus.Engine
{
    public record ResearchedItemStatus(string Name, int Id, int Current)
    {       
        public bool Researched => Current >= EngineManager.ItemResearchNeeded[Id];
        public int Needed => EngineManager.ItemResearchNeeded[Id];

        public static ResearchedItemStatus CreateEmpty(int id) => new(ItemID.Search.GetName(id), id, 0);
    }
}
