using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap.Drummer
{
    public class DrummerMapWriter : BeatmapWriter
    {
        //ctor
        public DrummerMapWriter(DrummerMap beatmap) : base(beatmap) { }

        public override BeatmapType TypeInstance => BeatmapTypeInstances.Drummer;
    }
}
