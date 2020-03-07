using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beatmap.Serialization;
using Beatmap.Editor.Timeline;
using System.Linq;
using Beatmap.Editor.Commands;

namespace Beatmap.Editor
{
    public class TimelineComponent : EditorComponent
    {
        public BpmChangeTrack BpmChangeTrack;
        public SectionNameTrack SectionNameTrack;

        public TrackMarker StartingDelayMarker;

        public AddTrackMarkerBox AddTrackMarkerBoxPrefab;

        public AudioMetadata AudioMetadata { get; private set; }
        private readonly List<TimelineTrack> _tracks = new List<TimelineTrack>();

        protected override void InitializeInternal()
        {
            AudioMetadata = Editor.IOComponent.TryLoadData<AudioMetadata>(BeatmapFileUtility.TryGetAudioMetadata, "audio metadata");

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
            StartingDelayMarker.OnRequestSet.AddListener(OnRequestStartingDelaySet);

            _tracks.ForEach(track => track.SubscribeToEvents());
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            // Visual update events
            AudioMetadata.OnStartingDelaySet -= DoSetStartingDelay;

            // Request-update events
            StartingDelayMarker.OnRequestSet.RemoveListener(OnRequestStartingDelaySet);

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
            StartingDelayMarker.StringValue = newValue.ToString();
            Editor.SetDirty();
        }
    }
}