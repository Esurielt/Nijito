using Beatmap.Editor;
using Beatmap.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap.Drummer
{
    public class DrummerEditor : PianoRollEditor
    {
        public override BeatmapType TypeInstance => BeatmapTypeInstances.Drummer;

        protected override IBeatmapIOHelper GetNewFileIOHelper()
        {
            return new BeatmapFileIOHelper_JSON();
        }

        protected override EditorVisualsWriter GetNewVisualsWriter()
        {
            return new PianoRollEditorVisualsWriter(this, new DrummerMap());
        }
    }
}