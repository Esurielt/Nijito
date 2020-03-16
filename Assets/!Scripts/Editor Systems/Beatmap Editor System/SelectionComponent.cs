using SongData;
using KeyCombos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editors.BeatmapEditor
{
    public class SelectionComponent : EditorComponent<BeatmapEditor>
    {
        public Image SelectionSquare;
        public TMPro.TMP_Text StartFrameIndexText;

        public GameObject SelectionEndBar;
        public TMPro.TMP_Text EndFrameIndexText;

        public Button SelectAllButton;
        public Button ClearSelectionButton;

        public bool HasSingleSelection => CurrentMode == Mode.SINGLE;
        public bool HasMultiSelection => CurrentMode == Mode.MULTI;
        public Mode CurrentMode { get; private set; }

        public int SelectionStart { get; private set; }
        public int SelectionLength { get; private set; }
        public int SelectionImpliedEnd => SelectionStart + SelectionLength;     // NOTE: The first non-selected frame index.

        public void SelectFrame(int frameIndex)
        {
            bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (shiftHeld)
            {
                // Muti-select mode on

                if (frameIndex > SelectionImpliedEnd - 1)
                {
                    // Past end: move end.
                    // Selected index becomes last index within selection (move implied end to the next visible frame)
                    MoveSelectionEnd(frameIndex);
                }
                else if (frameIndex < SelectionStart)
                {
                    // Before start: move start.
                    MoveSelectionStart(frameIndex);
                }
                else
                {
                    // Inside existing selection: adjust bounds based on distance from endpoint.
                    var normalizedPosition = Mathf.InverseLerp(SelectionStart, SelectionImpliedEnd - 1, frameIndex);
                    if (normalizedPosition < 0.5f)
                    {
                        MoveSelectionStart(frameIndex);
                    }
                    else
                    {
                        MoveSelectionEnd(frameIndex);
                    }
                }

                CurrentMode = Mode.MULTI;
                Editor.SetDirty();
                return;
            }

            // Single-select mode.

            SetSelection(frameIndex);
            
            CurrentMode = Mode.SINGLE;
            Editor.SetDirty();
        }
        private void MoveSelectionStart(int newStart)
        {
            var originalStart = SelectionStart;
            var deltaLength = originalStart - newStart;

            SetSelection(newStart, SelectionLength + deltaLength);
        }
        private void MoveSelectionEnd(int newFinalVisibleFrame)
        {
            var subdivisionsPerBeat = Editor.SubdivisionComponent.CurrentSubdivision.QuantityPerBeat;
            var newImpliedEnd = newFinalVisibleFrame + (SongDataUtility.FRAMES_PER_BEAT / subdivisionsPerBeat);
            var newLength = newImpliedEnd - SelectionStart;

            SetSelection(SelectionStart, newLength);
        }
        public bool IncludesIndex(int frameIndex, bool includeHiddenFrames)
        {
            if (CurrentMode == Mode.SINGLE)
            {
                return frameIndex == SelectionStart;
            }

            // Multi-mode
            // Check absolute range.
            if (frameIndex >= SelectionStart && frameIndex < SelectionImpliedEnd)
            {
                //return true if within range and all frames allowed, OR if frame index is visible with current subdivision
                return includeHiddenFrames || Editor.SubdivisionComponent.CurrentSubdivision.IncludesIndex(frameIndex);
            }
            
            return false;
        }
        public void SelectAll()
        {
            var frameCount = Editor.BeatmapWriter.FrameCount;
            if (frameCount > 0)
            {
                SetSelection(0, frameCount);
                CurrentMode = Mode.MULTI;
            }
        }        
        public void ClearSelection()
        {
            SetSelection(SelectionStart, 1);
            CurrentMode = Mode.SINGLE;
        }
        private void SetSelection(int singleSelection)
        {
            var subdivisionsPerBeat = Editor.SubdivisionComponent.CurrentSubdivision.QuantityPerBeat;
            SetSelection(singleSelection, (SongDataUtility.FRAMES_PER_BEAT / subdivisionsPerBeat));
        }
        private void SetSelection(int startIndex, int length)
        {
            SelectionStart = startIndex;
            SelectionLength = length;

            Editor.SetDirty();
        }
        protected override void RepaintInternal()
        {
            var unitWidth = Editor.FrameManagerComponent.GetCurrentFrameWidth() /
                (SongDataUtility.FRAMES_PER_BEAT / Editor.SubdivisionComponent.CurrentSubdivision.QuantityPerBeat);

            var squareRt = SelectionSquare.GetComponent<RectTransform>();
            
            squareRt.anchoredPosition = new Vector2(unitWidth * SelectionStart, squareRt.anchoredPosition.y);
            
            StartFrameIndexText.text = SelectionStart.ToString();

            if (CurrentMode == Mode.MULTI)
            {
                squareRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, unitWidth * SelectionLength);
                
                SelectionSquare.enabled = true;

                var selectionLastFrame = SelectionImpliedEnd - 1;
                EndFrameIndexText.text = selectionLastFrame.ToString();
                SelectionEndBar.SetActive(true);
            }
            else
            {
                SelectionSquare.enabled = false;
                SelectionEndBar.SetActive(false);
            }
        }

        protected override void InitializeInternal()
        {
            ClearSelection();
        }
        public override void RegisterHotkeys()
        {
            RegisterHotkey("Select All Frames", () => SelectAll(), new KeyCombo(KeyCode.A, ToggleKey.Ctrl));
        }
        protected override void SubscribeToEventsInternal()
        {
            SelectAllButton.onClick.AddListener(SelectAll);            
            ClearSelectionButton.onClick.AddListener(ClearSelection);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            SelectAllButton.onClick.RemoveListener(SelectAll);
            ClearSelectionButton.onClick.RemoveListener(ClearSelection);
        }

        public enum Mode
        {
            SINGLE = 0,
            MULTI = 1,
        }
    }
}