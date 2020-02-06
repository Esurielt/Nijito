using System.Collections.Generic;
using Beatmap.Interfaces;

namespace Beatmap.IO
{
    public interface IBeatmapIOHelper
    {
        bool TryReadBeatmapFromFile(BeatmapType typeInstance, out Beatmap newBeatmap);
        bool TryWriteBeatmapToFile(IBeatmap beatmap);
    }
    public enum BeatmapDataComponentType
    {
        AUDIO = 1,
        //metadata
        //images
    }
}
