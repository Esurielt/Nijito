using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beatmap.Editor.Commands;
using Beatmap.IO;

namespace Beatmap.Editor
{
    public abstract class BeatmapEditor : MonoBehaviour
    {
        //Unity fields
        public RectTransform DataPointContainer;
        public EditorDataPointController DataPointControllerPrefab;
        public TMPro.TMP_InputField DirectoryNameField;

        //repaint event
        [HideInInspector] public UnityEvent onRepaintEditor;
        private bool _dirtyFlag = false;

        public EditorVisualsWriter Writer { get; private set; }
        public IBeatmapIOHelper FileIOHelper { get; private set; }

        //internal stuff
        private readonly EventHelper _basicEventHelper = new EventHelper("Basic Events");
        private readonly UndoableEventHelper<BeatmapWriterCommand> _undoableEventHelper = new UndoableEventHelper<BeatmapWriterCommand>("Undoable Events");

        //abstract object getters
        protected abstract EditorVisualsWriter GetNewVisualsWriter();
        protected abstract IBeatmapIOHelper GetNewFileIOHelper();
        public abstract BeatmapType TypeInstance { get; }

        private void Awake()
        {
            Initialize();
        }
        private void OnEnable()
        {
            _basicEventHelper.SubscribeToEvents();
            _undoableEventHelper.SubscribeToEvents();
        }
        private void OnDisable()
        {
            _basicEventHelper.UnsubscribeFromEvents();
            _undoableEventHelper.UnsubscribeFromEvents();
        }
        public void Initialize()
        {
            Writer = GetNewVisualsWriter();
            FileIOHelper = GetNewFileIOHelper();

            LateInitialize();

            RegisterEvents();

            //if Initialize is ever called after OnEnable, you'll need to manually call OnEnable right here (to subscribe events for the first time)
        }
        protected virtual void LateInitialize() { }
        public abstract void OnDataPointInstantiated(EditorDataPointController dataPointController);
        public abstract void OnDataPointDestroyed(EditorDataPointController dataPointController);

        /// <summary>
        /// Use this method to register all editor events, both basic (e.g. 'select a tool') and undoable (e.g. 'insert/remove beats').
        /// </summary>
        protected virtual void RegisterEvents()
        {
            Game.Log(Logging.Category.BEATMAP, "Registering editor events...", Logging.Level.LOG);
        }
        protected void RegisterEvent_Basic(UnityEvent unityEvent, UnityAction action)
        {
            _basicEventHelper.RegisterEvent(unityEvent, action);
        }
        protected void RegisterEvent_OnRepaint(UnityAction action)
        {
            _basicEventHelper.RegisterEvent(onRepaintEditor, action);
        }
        protected void RegisterEvent_Undoable(UnityEvent unityEvent, BeatmapWriterCommand command)
        {
            _undoableEventHelper.RegisterEvent(unityEvent, () => _undoableEventHelper.ExecuteCommand(command));
        }
        //"unregister event" methods for reuseable editor window?

        public void Undo()
        {
            Game.Log(Logging.Category.BEATMAP, "Editor Event: Undo", Logging.Level.LOG);
            if (!_undoableEventHelper.Undo())
            {
                Game.Log(Logging.Category.BEATMAP, "Nothing to undo!", Logging.Level.LOG);
            }
        }
        public void Redo()
        {
            Game.Log(Logging.Category.BEATMAP, "Editor Event: Redo", Logging.Level.LOG);
            if (!_undoableEventHelper.Redo())
            {
                Game.Log(Logging.Category.BEATMAP, "Nothing to redo!", Logging.Level.LOG);
            }
        }
        public void SetDirty() => _dirtyFlag = true;
        protected void RepaintEditor()
        {
            _dirtyFlag = false;
            onRepaintEditor.Invoke();
        }
        public void ForEachDataPointController(System.Action<EditorDataPointController> forEach)
        {
            Writer.ForEachDataPointController(forEach);
        }

        private void Update()
        {
            if (_dirtyFlag)
            {
                RepaintEditor();
            }
        }

        #region Editor helper classes
        private class EventHelper
        {
            //'dictionary' of events and the unity actions they trigger
            protected List<KeyValuePair<UnityEvent, UnityAction>> _registry = new List<KeyValuePair<UnityEvent, UnityAction>>();

            private bool _subscribed = false;

            public string Name { get; private set; }
            public EventHelper(string name)
            {
                Name = name;
            }

            public void RegisterEvent(UnityEvent unityEvent, UnityAction unityAction)
            {
                _registry.Add(new KeyValuePair<UnityEvent, UnityAction>(unityEvent, unityAction));
            }
            public void SubscribeToEvents()     //called on enable
            {
                if (_subscribed)
                    return;

                Game.LogFormat(Logging.Category.BEATMAP, "{0}: Subscribing to editor events...", Logging.Level.LOG, Name);
                foreach (var kvp in _registry)
                {
                    kvp.Key.AddListener(kvp.Value);
                }

                _subscribed = true;
            }
            public void UnsubscribeFromEvents()     //called on disable
            {
                if (!_subscribed)
                    return;

                Game.LogFormat(Logging.Category.BEATMAP, "{0}: Unsubscribing from editor events...", Logging.Level.LOG, Name);
                foreach (var kvp in _registry)
                {
                    kvp.Key.RemoveListener(kvp.Value);
                }

                _subscribed = false;
            }
        }
        private class UndoableEventHelper<TCommand> : EventHelper where TCommand : IUndoableCommand
        {
            private List<TCommand> _undoStack = new List<TCommand>();
            private int _undoStackPointer = 0;  //<-- points to the number after the last index (the stack count).
            //When pointer = undo stack count, then there are no undo actions to be made.

            public UndoableEventHelper(string name) : base(name) { }

            public void ExecuteCommand(TCommand command)
            {
                if (command == null)
                    return;

                //if undo stack pointer is before the last index of the list, there have been undo actions made
                if (_undoStackPointer < _undoStack.Count)
                {
                    //overwrite remaining undo stack
                    _undoStack = _undoStack.GetRange(0, _undoStackPointer); //<-- undo stack becomes the first N actions, where N is the pointer value (convenient!)
                }

                command.Execute();

                _undoStack.Add(command);
                _undoStackPointer++;
            }
            public bool Undo()  //return true if an action was undone
            {
                //if the undo stack pointer is at 0, then there are no more actions in the undo stack
                if (_undoStackPointer == 0)
                    return false;

                var command = _undoStack[_undoStackPointer - 1];    //<-- undo the last action
                
                command.Undo();
                _undoStackPointer--;

                return true;
            }
            public bool Redo()  //return true if an action was redone
            {
                //if undo stack pointer = the stack count, there are no undo actions to redo. do nothing.
                if (_undoStackPointer == _undoStack.Count)
                    return false;

                var command = _undoStack[_undoStackPointer];    //<-- do the 'next'(current) action

                command.Execute();
                _undoStackPointer++;

                return true;
            }
        }
        public class EditorIOHelper
        {
            protected BeatmapEditor _editor;
            protected BeatmapFileIOHelper _io;
            public EditorIOHelper(BeatmapEditor editor, BeatmapFileIOHelper io)
            {
                _editor = editor;
                _io = io;
                InitializeInputField();
            }
            protected void InitializeInputField()
            {
                _editor.DirectoryNameField.text = "Untitled Beatmap";
            }
            public bool TryGetBeatmapFromFile(out Beatmap beatmap)
            {
                _io.DirectoryName = _editor.DirectoryNameField.text;
                return _io.TryReadBeatmapFromFile(_editor.TypeInstance, out beatmap);
            }
            public bool TryWriteBeatmapToFile(Beatmap beatmap)
            {
                _io.DirectoryName = _editor.DirectoryNameField.text;
                return _io.TryWriteBeatmapToFile(beatmap);
            }
            //audio, metadata
        }
        #endregion
    }
}
