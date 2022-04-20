using System;

namespace Zeus.Engine
{
    public interface IZeusEngine : IDisposable
    {
        ResearchedItemStatus[] GetResearchedItemStatuses();

        void PushSettingsUI();
    }
}
