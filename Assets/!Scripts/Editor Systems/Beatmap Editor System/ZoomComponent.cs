using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editors.BeatmapEditor
{
    public class ZoomComponent : EditorComponent<BeatmapEditor>
    {
        const float MIN = 0.1f;
        const float MAX = 2f;

        public ReleasableSlider ZoomSlider;
        public Button ResetZoomButton;

        public float CurrentMultiplier => ZoomSlider.value;

        protected override void InitializeInternal()
        {
            ZoomSlider.minValue = MIN;
            ZoomSlider.maxValue = MAX;

            ResetZoom();
        }
        protected override void SubscribeToEventsInternal()
        {
            ZoomSlider.onSliderReleased.AddListener(Editor.SetDirty);
            ResetZoomButton.onClick.AddListener(ResetZoom);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            ZoomSlider.onSliderReleased.RemoveAllListeners();
            ResetZoomButton.onClick.RemoveAllListeners();
        }
        public void ResetZoom()
        {
            ZoomSlider.value = 1f;
            Editor.SetDirty();
        }
    }
}