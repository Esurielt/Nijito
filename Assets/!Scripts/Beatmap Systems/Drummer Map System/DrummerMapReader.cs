namespace Beatmap.Drummer
{
    public class DrummerMapReader : BeatmapReader
    {
        //ctor
        public DrummerMapReader(DrummerMap beatmap) : base(beatmap) { }

        public override BeatmapType TypeInstance => BeatmapTypeInstances.Drummer;
    }
}
