using Beatmap.Interfaces;

namespace Beatmap
{
    public class BeatmapWriter : AbstractBeatmapWriter
    {
        public BeatmapWriter(Beatmap beatmap) : base(beatmap) { }

        protected override IDataPoint InstantiateDataPointInstance(IDataPoint initializeValuesFrom, int frameIndex)
        {
            return new DataPoint(initializeValuesFrom);
        }
        protected override void DestroyDataPointInstance(IDataPoint dataPoint)
        {
            // Okay! :D
        }
    }
}
