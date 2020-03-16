using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editors.BeatmapEditor.Timeline
{
    public class SectionNameTrack : TimelineTrack<string>
    {
        public override bool TryParseValue(string input, out string value)
        {
            value = string.Empty;
            
            if (string.IsNullOrEmpty(input))
                return false;
            
            // If we want to restrict section names later, we would add it here.

            value = input;
            return true;
        }

        protected override void InitializeAddTrackMarkerBox()
        {
            _addTrackMarkerBox.Initialize("Section Name", TMPro.TMP_InputField.CharacterValidation.Name);
        }
    }
}