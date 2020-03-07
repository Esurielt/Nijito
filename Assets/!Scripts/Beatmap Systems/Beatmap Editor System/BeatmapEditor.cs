using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beatmap.Editor.Commands;
using Beatmap.Serialization;
using System.Collections;
using MessageBoxes;
using System;
using Beatmap.Interfaces;
using UnityEngine.UI;

namespace Beatmap.Editor
{
    [RequireComponent(typeof(AudioComponent))]
    [RequireComponent(typeof(IOComponent))]
    [RequireComponent(typeof(FrameManagerComponent))]
    [RequireComponent(typeof(ZoomComponent))]
    [RequireComponent(typeof(UndoStackComponent))]
    [RequireComponent(typeof(SelectionComponent))]
    [RequireComponent(typeof(SubdivisionComponent))]
    [RequireComponent(typeof(TimelineComponent))]
    [RequireComponent(typeof(TemplateFrameComponent))]
    [RequireComponent(typeof(ScrollComponent))]
    [RequireComponent(typeof(ClipboardComponent))]
    [RequireComponent(typeof(FileBrowserComponent))]
    public class BeatmapEditor : MonoBehaviour
    {
        //Unity fields
        public TMPro.TMP_Text SongNameText;
        public TMPro.TMP_Text DifficultyText;
        public TMPro.TMP_Text BeatmapTypeText;

        public Transform SmallWindowContainer;

        //repaint event
        [HideInInspector] public UnityEvent OnRepaint;
        private bool _dirtyFlag = false;

        //internal stuff
        public bool Initialized { get; private set; }

        protected List<EditorComponent> _editorComponents = new List<EditorComponent>();
        public FrameManagerComponent FrameManagerComponent { get; private set; }
        public IOComponent IOComponent { get; private set; }
        public AudioComponent AudioComponent { get; private set; }
        public ZoomComponent ZoomComponent { get; private set; }
        public UndoStackComponent UndoStackComponent { get; private set; }
        public SelectionComponent SelectionComponent { get; private set; }
        public SubdivisionComponent SubdivisionComponent { get; private set; }
        public TimelineComponent TimelineComponent { get; private set; }
        public TemplateFrameComponent TemplateFrameComponent { get; private set; }
        public ScrollComponent ScrollComponent { get; private set; }
        public ClipboardComponent ClipboardComponent { get; private set; }
        public FileBrowserComponent FileBrowserComponent { get; private set; }

        public BeatmapWriter BeatmapWriter { get; private set; }
        //public Beatmap Beatmap { get; private set; }
        public string SongName { get; private set; }
        public BeatmapDifficulty Difficulty { get; private set; }

        public void Initialize(string songName, BeatmapDifficulty difficulty, Beatmap beatmap)
        {
            if (Initialized)
                return;

            SetDirty();

            Initialized = true;

            BeatmapWriter = new BeatmapWriter(beatmap);

            SongName = songName;
            SongNameText.text = songName;

            Difficulty = difficulty;
            DifficultyText.text = difficulty.Name;

            BeatmapTypeText.text = beatmap.TypeInstance.Name;

            IOComponent = RegisterEditorComponent<IOComponent>();
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
            FileBrowserComponent = RegisterEditorComponent<FileBrowserComponent>();

            _editorComponents.ForEach(com => com.Initialize(this));
            
            SubscribeToEvents();
        }
        private void SubscribeToEvents()
        {
            _editorComponents.ForEach(com => com.SubscribeToEvents());
        }
        private void UnsubscribeFromEvents()
        {
            _editorComponents.ForEach(com => com.UnsubscribeFromEvents());
        }
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        protected TComponent RegisterEditorComponent<TComponent>() where TComponent : EditorComponent
        {
            var component = GetComponent<TComponent>();
            _editorComponents.Add(component);
            return component;
        }
        public void SetDirty()
        {
            _dirtyFlag = true;
        }
        protected void Repaint()
        {
            _dirtyFlag = false;
            OnRepaint.Invoke();
        }
        private void Update()
        {
            if (_dirtyFlag)
                Repaint();
        }
    }
}
