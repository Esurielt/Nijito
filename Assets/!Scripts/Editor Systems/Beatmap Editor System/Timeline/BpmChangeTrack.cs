using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editors.BeatmapEditor.Timeline
{
    public class BpmChangeTrack : TimelineTrack<float>
    {
        public override bool TryParseValue(string input, out float value)
        {
            return float.TryParse(input, out value);
        }

        protected override void InitializeAddTrackMarkerBox()
        {
            _addTrackMarkerBox.Initialize("BPM Change", TMPro.TMP_InputField.CharacterValidation.Integer);
        }
    }
}