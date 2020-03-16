using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SongData.Serialization;
using Editors.BeatmapEditor.Timeline;
using Editors.BeatmapEditor.Commands;
using SongData;

namespace Editors.BeatmapEditor
{
    public class TimelineComponent : EditorComponent<BeatmapEditor>
    {
        public BpmChangeTrack BpmChangeTrack;
        public SectionNameTrack SectionNameTrack;

        public TrackMarker StartingDelayMarker;

        public AddTrackMarkerBox AddTrackMarkerBoxPrefab;

        public AudioMetadata AudioMetadata { get; private set; }
        private readonly List<TimelineTrack> _tracks = new List<TimelineTrack>();

        protected override void InitializeInternal()
        {
            AudioMetadata = Editor.IOComponent.LoadData<AudioMetadata>(SongDataFileUtility.TryGetAudioMetadata, "audio metadata");
            if (AudioMetadata == null)
            {
                AudioMetadata = new AudioMetadata();
            }

            DoSetStartingDelay(AudioMetadata.StartingDelay);

            RegisterTrackAndInitialize(AudioMetadata.BpmChanges, BpmChangeTrack);
            RegisterTrackAndInitialize(AudioMetadata.SectionNames, SectionNameTrack);
        }
        private void RegisterTrackAndInitialize<TValue>(AudioMetadata.ValueHelper<TValue> valueHelper, TimelineTrack<TValue> track)
        {
            track.Initialize(Editor, valueHelper);
            _tracks.Add(track);
        }
        protected override void SubscribeToEventsInternal()
        {
            // Visual update events
            AudioMetadata.OnStartingDelaySet += DoSetStartingDelay;

            // Request-update events
            StartingDelayMarker.ValueField.OnRequestSetValue.AddListener(OnRequestStartingDelaySet);

            _tracks.ForEach(track => track.SubscribeToEvents());
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            // Visual update events
            AudioMetadata.OnStartingDelaySet -= DoSetStartingDelay;

            _tracks.ForEach(track => track.UnsubscribeFromEvents());
        }
        protected override void RepaintInternal()
        {
            float frameWidth = Editor.FrameManagerComponent.GetCurrentFrameWidth();
            int visibleSubdivisionsPerBeat = Editor.SubdivisionComponent.CurrentSubdivision.QuantityPerBeat;

            _tracks.ForEach(track => track.Repaint(frameWidth, visibleSubdivisionsPerBeat));
        }
        private void OnRequestStartingDelaySet(string input)
        {
            if (float.TryParse(input, out float newValue))
            {
                var command = new SetStartingDelayCommand(AudioMetadata, () => newValue);
                Editor.UndoStackComponent.ExecuteCommand(command);
            }
        }
        private void DoSetStartingDelay(float newValue)
        {
            StartingDelayMarker.ValueField.SetValueText(newValue.ToString());
            Editor.SetDirty();
        }
    }
}