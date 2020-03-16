using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using SongData;

namespace Editors.BeatmapEditor
{
    [RequireComponent(typeof(AudioComponent))]
    [RequireComponent(typeof(BeatmapIOComponent))]
    [RequireComponent(typeof(FrameManagerComponent))]
    [RequireComponent(typeof(ZoomComponent))]
    [RequireComponent(typeof(UndoStackComponent))]
    [RequireComponent(typeof(SelectionComponent))]
    [RequireComponent(typeof(SubdivisionComponent))]
    [RequireComponent(typeof(TimelineComponent))]
    [RequireComponent(typeof(TemplateFrameComponent))]
    [RequireComponent(typeof(ScrollComponent))]
    [RequireComponent(typeof(ClipboardComponent))]
    public class BeatmapEditor : EditorBase
    {
        //Unity fields
        public TMPro.TMP_Text DifficultyText;
        public TMPro.TMP_Text BeatmapTypeText;

        public Transform SmallWindowContainer;

        public FrameManagerComponent FrameManagerComponent { get; private set; }
        public AudioComponent AudioComponent { get; private set; }
        public ZoomComponent ZoomComponent { get; private set; }
        public UndoStackComponent UndoStackComponent { get; private set; }
        public SelectionComponent SelectionComponent { get; private set; }
        public SubdivisionComponent SubdivisionComponent { get; private set; }
        public TimelineComponent TimelineComponent { get; private set; }
        public TemplateFrameComponent TemplateFrameComponent { get; private set; }
        public ScrollComponent ScrollComponent { get; private set; }
        public ClipboardComponent ClipboardComponent { get; private set; }

        public BeatmapWriter BeatmapWriter { get; private set; }
        public BeatmapDifficulty Difficulty { get; private set; }

        public void Initialize(string songName, BeatmapDifficulty difficulty, SongData.Beatmap beatmap)
        {
            InitializeFirst(songName);

            BeatmapWriter = new BeatmapWriter(beatmap);

            Difficulty = difficulty;
            DifficultyText.text = difficulty.Name;

            BeatmapTypeText.text = beatmap.TypeInstance.Name;

            InitializeLast();
        }
        protected override void RegisterComponentsInternal()
        {
            IOComponent = RegisterEditorComponent<BeatmapIOComponent>();
            AudioComponent = RegisterEditorComponent<AudioComponent>();
            ZoomComponent = RegisterEditorComponent<ZoomComponent>();
            FrameManagerComponent = RegisterEditorComponent<FrameManagerComponent>();
            UndoStackComponent = RegisterEditorComponent<UndoStackComponent>();
            SelectionComponent = RegisterEditorComponent<SelectionComponent>();
            SubdivisionComponent = RegisterEditorComponent<SubdivisionComponent>();
            TimelineComponent = RegisterEditorComponent<TimelineComponent>();
            TemplateFrameComponent = RegisterEditorComponent<TemplateFrameComponent>();
            ScrollComponent = RegisterEditorComponent<ScrollComponent>();
            ClipboardComponent = RegisterEditorComponent<ClipboardComponent>();
        }

    }
}
