using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beatmap.Editor.EditorCommands;

namespace Beatmap.Editor
{
    public abstract class BeatmapEditor : MonoBehaviour
    {
        public BeatmapWriter BeatmapWriter { get; private set; }
        public IBeatmapFileIOHelper FileIOHelper { get; private set; }

        //Unity fields
        public RectTransform DataPointsTransform;
        public DataPointController DataPointControllerPrefab;

        //internal stuff
        private readonly EventHelper _basicEventHelper = new EventHelper("Basic Events");
        private readonly UndoableEventHelper<EditorCommand> _undoableEventHelper = new UndoableEventHelper<EditorCommand>("Edit Events");
        protected List<DataPointController> _dataPointControllers = new List<DataPointController>();

        //abstract object getters
        public abstract BeatmapWriter GetNewBeatmapWriter();
        public abstract IBeatmapFileIOHelper GetNewFileIOHelper();

        private void Awake()
        {
            BeatmapEditorInstances.CurrentEditor = this;
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
            BeatmapEditorInstances.CurrentEditor = this;

            BeatmapWriter = GetNewBeatmapWriter();
            FileIOHelper = GetNewFileIOHelper();

            PopulateEditor();
            RegisterEvents();

            //if Initialize is ever called after OnEnable, you'll need to manually call OnEnable right here (to subscribe events for the first time)
        }
        public void PopulateEditor()
        {
            Game.Log(Logging.Category.BEATMAP, "Populating beatmap editor window.", Logging.Level.LOG);
            int count = BeatmapWriter.FrameCount;
            for (int i = 0; i < count; i++)
            {
                var newDataPointController = Instantiate(DataPointControllerPrefab, DataPointsTransform);
                newDataPointController.Initialize(i, BeatmapWriter.Beatmap.GetDataPoints()[i]);
                _dataPointControllers.Add(newDataPointController);
            }
        }
        /// <summary>
        /// Use this method to register all editor events, both basic (select a tool) and undoable (insert/remove beats).
        /// </summary>
        protected virtual void RegisterEvents()
        {
            Game.Log(Logging.Category.BEATMAP, "Registering editor events...", Logging.Level.LOG);
        }
        protected void RegisterEvent_Basic(UnityEvent unityEvent, UnityAction action)
        {
            _basicEventHelper.RegisterEvent(unityEvent, () => action());
        }
        protected void RegisterEvent_Undoable(UnityEvent unityEvent, EditorCommand command)
        {
            _undoableEventHelper.RegisterEvent(unityEvent, () => _undoableEventHelper.ExecuteCommand(command));
        }

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
        public void ClearEditor()
        {
            Game.Log(Logging.Category.BEATMAP, "Clearing beatmap editor window.", Logging.Level.LOG);
            for (int i = 0; i < DataPointsTransform.childCount; i++)
            {
                Destroy(DataPointsTransform.GetChild(i).gameObject);   //safe because Unity delays destruction until after stack unwinds (we can iterate)
            }
            _dataPointControllers.Clear();
        }

        protected DataPointController GetBlankFrame()
        {
            var newDataPointController = Instantiate(DataPointControllerPrefab, DataPointsTransform);
            newDataPointController.Initialize(0, new DataPoint(BeatmapWriter.BlankFrameFlyweight));
            return newDataPointController;
        }
        protected List<DataPointController> GetBlankBeat()
        {
            var newDataPointControllers = new List<DataPointController>();
            for (int i = 0; i < BeatmapUtility.FRAMES_PER_BEAT; i++)
            {
                newDataPointControllers.Add(GetBlankFrame());
            }
            return newDataPointControllers;
        }
        public void InsertBlankFramesAtIndex(int frameIndex, int frameCount)
        {
            Game.LogFormat(Logging.Category.BEATMAP, "Inserting {1} frame controllers into scene view, starting at index {0}.", Logging.Level.LOG, frameIndex, frameCount);

            for (int i = frameIndex; i < frameIndex + frameCount; i++) //NOTE: changed start and end to match desired sibling index
            {
                var controller = GetBlankFrame();
                controller.transform.SetSiblingIndex(i);
            }
        }
        public void InsertBlankBeatsAtIndex(int beatIndex, int beatCount)
        {
            int frameIndex = BeatmapUtility.ConvertBeatsToFrames(beatIndex);
            int frameCount = BeatmapUtility.ConvertBeatsToFrames(beatCount);

            InsertBlankFramesAtIndex(frameIndex, frameCount);
        }
        public void AddBlankBeatsAtEnd(int beatCount)
        {
            int frameCount = BeatmapUtility.ConvertBeatsToFrames(beatCount);

            InsertBlankFramesAtIndex(BeatmapWriter.FrameCount, frameCount);
        }

        public void RemoveFramesAtIndex(int frameIndex, int frameCount)
        {
            Game.LogFormat(Logging.Category.BEATMAP, "Removing {1} frame controllers from scene view, starting at index {0}.", Logging.Level.LOG, frameIndex, frameCount);

            var listOfChildrenToKill = new List<GameObject>();

            //get the gameobjects from their place in the sibling list
            for (int i = frameIndex; i < frameIndex + frameCount; i++)
            {
                listOfChildrenToKill.Add(DataPointsTransform.GetChild(i).gameObject);
            }

            //ruin christmas
            for (int i = 0; i < frameCount; i++)
            {
                Destroy(listOfChildrenToKill[i]);
            }
        }
        public void RemoveBeatsAtIndex(int beatIndex, int beatCount)
        {
            int frameIndex = BeatmapUtility.ConvertBeatsToFrames(beatIndex);
            int frameCount = BeatmapUtility.ConvertBeatsToFrames(beatCount);

            RemoveFramesAtIndex(frameIndex, frameCount);
        }
        public void RemoveBeatsFromEnd(int beatCount)
        {
            int frameCount = BeatmapUtility.ConvertBeatsToFrames(beatCount);

            RemoveFramesAtIndex(BeatmapWriter.FrameCount, frameCount);
        }

        #region Editor event helper classes
        private class EventHelper
        {
            //dictionary of events and the unity actions they trigger
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
        #endregion
    }
}
