using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Beatmap.Editor.Timeline
{
    public class DeletableTrackMarker : TrackMarker
    {
        public Button DeleteButton;
        public int BeatIndex { get; set; }

        [HideInInspector] public UnityEvent OnRequestRemove;

        protected RectTransform _rt;

        protected override void LateAwake()
        {
            _rt = GetComponent<RectTransform>();

            DeleteButton.onClick.AddListener(() => OnRequestRemove?.Invoke());
        }
        protected override void LateOnDestroy()
        {
            DeleteButton.onClick.RemoveAllListeners();
            OnRequestRemove.RemoveAllListeners();
        }
        public void SetWidthAndPosition(float newWidth, float newPosition)
        {
            _rt.anchoredPosition = new Vector2(newPosition, _rt.anchoredPosition.y);
            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }
}