using SongData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editors.BeatmapEditor
{
    public class ScrollComponent : EditorComponent<BeatmapEditor>
    {
        public RectTransform PointerDragHandle;

        private Coroutine _scrollCoroutine = null;
        public bool IsScrolling => _scrollCoroutine != null;

        public float ScrollPointerPreciseFrameIndex { get; private set; } = 0;

        public void SetScrollPointerToSelectionStart()
        {
            SetScrollPointerFrameIndex(Editor.SelectionComponent.SelectionStart);
        }
        public void SetScrollPointerFrameIndex(int frameIndex)
        {
            var frameWidth = Editor.FrameManagerComponent.GetCurrentFrameWidth();
            var subdivisionsPerBeat = Editor.SubdivisionComponent.CurrentSubdivision.QuantityPerBeat;
            SetScrollPointerPreciseFrameIndex((float)frameIndex, frameWidth, subdivisionsPerBeat);
        }
        private void SetScrollPointerPreciseFrameIndex(AudioComponent.PlaySession playSession, FrameManagerComponent frameManager, SubdivisionComponent subdivision)
        {
            SetScrollPointerPreciseFrameIndex(
                playSession.CurrentIndexesAndProgress.PreciseFrameIndex,
                frameManager.GetCurrentFrameWidth(),
                subdivision.CurrentSubdivision.QuantityPerBeat);
        }
        private void SetScrollPointerPreciseFrameIndex(float preciseFrameIndex, float frameWidth, int subdivisionsPerBeat)
        {
            ScrollPointerPreciseFrameIndex = preciseFrameIndex;
            var newXPos = GetScrollPositionForFrameIndex(preciseFrameIndex, frameWidth, subdivisionsPerBeat);
            SetScrollPointer(newXPos);
        }
        private void SetScrollPointer(float xPos)
        {
            PointerDragHandle.localPosition = new Vector2(xPos, PointerDragHandle.localPosition.y);
        }
        public void Scroll(AudioComponent.PlaySession playSession)
        {
            //Game.Log(Logging.Category.BEATMAP, "Beginning piano roll scroll.", Logging.Level.LOG);
            if (IsScrolling)
            {
                StopCoroutine(_scrollCoroutine);
            }
            _scrollCoroutine = StartCoroutine(ScrollCoroutine(playSession));
        }
        private IEnumerator ScrollCoroutine(AudioComponent.PlaySession playSession)
        {
            // Maybe a 'do while' would be cleaner

            var frameManager = Editor.FrameManagerComponent;
            var subdivisionComponent = Editor.SubdivisionComponent;

            while (!playSession.Complete)
            {
                SetScrollPointerPreciseFrameIndex(playSession, frameManager, subdivisionComponent);
                yield return null;  //come back next frame
            }

            SetScrollPointer(GetScrollPositionForFrameIndex(playSession.EndFrameIndex + 1, frameManager.GetCurrentFrameWidth(), subdivisionComponent.CurrentSubdivision.QuantityPerBeat));
        }
        private static float GetScrollPositionForFrameIndex(float preciseFrameIndex, float frameWidth, int subdivisionsPerBeat)
        {
            return preciseFrameIndex * frameWidth * ((float)subdivisionsPerBeat / (float)SongDataUtility.FRAMES_PER_BEAT);
        }
        public void StopScrollingAndReset()
        {
            if (IsScrolling)
                StopCoroutine(_scrollCoroutine);

            SetScrollPointerToSelectionStart();
        }

        protected override void CleanUpInternal()
        {
            if (IsScrolling)
                StopCoroutine(_scrollCoroutine);
        }
        protected override void SubscribeToEventsInternal()
        {
            Editor.AudioComponent.OnAudioPlay.AddListener(Scroll);
            Editor.AudioComponent.OnAudioStop.AddListener(StopScrollingAndReset);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            Editor.AudioComponent.OnAudioPlay.RemoveListener(Scroll);
            Editor.AudioComponent.OnAudioStop.RemoveListener(StopScrollingAndReset);
        }
        protected override void RepaintInternal()
        {
            SetScrollPointerToSelectionStart();
        }
    }
}