using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Editors.BeatmapEditor.Commands;
using SongData;

namespace Editors.BeatmapEditor.Timeline
{
    public abstract class TimelineTrack : MonoBehaviour
    {
        public Transform TrackMarkerContainer;
        public DeletableTrackMarker TrackMarkerPrefab;
        
        public TrackMarker StartingTrackMarker;
        
        public Button AddNewMarkerButton;

        protected BeatmapEditor _editor;
        protected List<DeletableTrackMarker> _trackMarkers = new List<DeletableTrackMarker>();
        protected AddTrackMarkerBox _addTrackMarkerBox;
        protected RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }
        public abstract void SubscribeToEvents();
        protected abstract void SubscribeToEvents_PerMarker(DeletableTrackMarker marker);
        public abstract void UnsubscribeFromEvents();
        public void Repaint(float frameWidth, int visibleSubdivisionsPerBeat)
        {
            foreach (var marker in _trackMarkers)
            {
                float newWidth = frameWidth * visibleSubdivisionsPerBeat;
                float newPosition = frameWidth * visibleSubdivisionsPerBeat * marker.BeatIndex;

                marker.SetWidthAndPosition(newWidth, newPosition);
            }
        }
        protected DeletableTrackMarker GetMarkerAtIndex(int beatIndex)
        {
            return _trackMarkers.Find(m => m.BeatIndex == beatIndex);
        }
        
        public void DoRemoveMarker(int atBeatIndex)
        {
            var marker = GetMarkerAtIndex(atBeatIndex);
            
            Destroy(marker.gameObject);
            
            _trackMarkers.Remove(marker);
            _editor.SetDirty();
        }
    }
    public abstract class TimelineTrack<TValue> : TimelineTrack
    {
        protected AudioMetadata.ValueHelper<TValue> _valueHelper;
        public void Initialize(BeatmapEditor editor, AudioMetadata.ValueHelper<TValue> valueHelper)
        {
            _editor = editor;
            _valueHelper = valueHelper;

            DoSetStartingMarker(valueHelper.StartingValue);            
            
            // Initialize track markers.
            foreach (var kvp in valueHelper.GetAllValuesCopy())
            {
                DoAddMarker(kvp.Key, kvp.Value);
            }

            _addTrackMarkerBox = Instantiate(editor.TimelineComponent.AddTrackMarkerBoxPrefab, editor.SmallWindowContainer);
            InitializeAddTrackMarkerBox();
            _addTrackMarkerBox.gameObject.SetActive(false);
        }
        public override void SubscribeToEvents()
        {
            // Visual update events.
            _valueHelper.OnStartingValueSet += DoSetStartingMarker;
            _valueHelper.OnValueAdded += DoAddMarker;
            _valueHelper.OnValueSet += DoSetMarker;
            _valueHelper.OnValueRemoved += DoRemoveMarker;

            // Update-request events.
            StartingTrackMarker.ValueField.OnRequestSetValue.AddListener(OnRequestStartingValueSet);

            AddNewMarkerButton.onClick.AddListener(OnClickAddNewMarkerButton);
            _addTrackMarkerBox.AddButton.onClick.AddListener(OnSubmitAddTrackMarkerBox);
        }
        protected override void SubscribeToEvents_PerMarker(DeletableTrackMarker marker)
        {
            int markerBeatIndex = marker.BeatIndex;

            marker.ValueField.OnRequestSetValue.AddListener(input => OnRequestValueSet(markerBeatIndex, input));
            marker.OnRequestRemove.AddListener(() => OnRequestValueRemove(markerBeatIndex));
        }
        public override void UnsubscribeFromEvents()
        {
            // Visual update events.
            _valueHelper.OnStartingValueSet -= DoSetStartingMarker;
            _valueHelper.OnValueAdded -= DoAddMarker;
            _valueHelper.OnValueSet -= DoSetMarker;
            _valueHelper.OnValueRemoved -= DoRemoveMarker;

            // Update-request events.
            StartingTrackMarker.ValueField.OnRequestSetValue.RemoveListener(OnRequestStartingValueSet);

            AddNewMarkerButton.onClick.RemoveListener(OnClickAddNewMarkerButton);
            _addTrackMarkerBox.AddButton.onClick.RemoveListener(OnSubmitAddTrackMarkerBox);
        }
        protected void OnClickAddNewMarkerButton()
        {
            _addTrackMarkerBox.gameObject.SetActive(true);
        }
        protected abstract void InitializeAddTrackMarkerBox();
        private void OnRequestStartingValueSet(string input)
        {
            if (TryParseValue(input, out TValue newValue))
            {
                var command = new TimelineSetStartingValueCommand<TValue>(_valueHelper, () => newValue);
                _editor.UndoStackComponent.ExecuteCommand(command);
            }
        }
        private void OnSubmitAddTrackMarkerBox()
        {
            if (_addTrackMarkerBox.TryGetBeatIndex(out int beatIndex))
            {
                if (TryParseValue(_addTrackMarkerBox.GetValueString(), out TValue newValue))
                {
                    var command = new TimelineAddTrackMarkerCommand<TValue>(_valueHelper, () => new TimelineAddTrackMarkerCommand<TValue>.Args(beatIndex, newValue));
                    _editor.UndoStackComponent.ExecuteCommand(command);

                    _addTrackMarkerBox.gameObject.SetActive(false);
                }
            }
        }
        private void OnRequestValueSet(int atBeatIndex, string input)
        {
            if (TryParseValue(input, out TValue newValue))
            {
                var command = new TimelineSetTrackMarkerCommand<TValue>(_valueHelper, () => new TimelineSetTrackMarkerCommand<TValue>.Args(atBeatIndex, newValue));
                _editor.UndoStackComponent.ExecuteCommand(command);
            }
        }
        private void OnRequestValueRemove(int atBeatIndex)
        {
            var command = new TimelineRemoveTrackMarkerCommand<TValue>(_valueHelper, () => atBeatIndex);
            _editor.UndoStackComponent.ExecuteCommand(command);
        }
        public abstract bool TryParseValue(string input, out TValue value);
        public void DoSetStartingMarker(TValue value)
        {
            StartingTrackMarker.ValueField.SetValueText(value.ToString());
            _editor.SetDirty();
        }
        public void DoAddMarker(int atBeatIndex, TValue value)
        {
            var controller = Instantiate(TrackMarkerPrefab, TrackMarkerContainer);
            
            controller.BeatIndex = atBeatIndex;            
            controller.ValueField.SetValueText(value.ToString());
            SubscribeToEvents_PerMarker(controller);

            _trackMarkers.Add(controller);
            _editor.SetDirty();
        }
        public void DoSetMarker(int atBeatIndex, TValue value)
        {
            var marker = GetMarkerAtIndex(atBeatIndex);
            marker.ValueField.SetValueText(value.ToString());
            _editor.SetDirty();
        }
    }
}