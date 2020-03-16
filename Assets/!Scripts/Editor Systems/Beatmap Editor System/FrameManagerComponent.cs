using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using Editors.BeatmapEditor.Commands;
using SongData;

namespace Editors.BeatmapEditor
{
    public class FrameManagerComponent : EditorComponent<BeatmapEditor>
    {
        const int DEFAULT_WIDTH = 100;

        public EditorFrameController FrameControllerPrefab;
        public Transform FrameControllersContainer;

        public TMPro.TMP_InputField BeatNumberInput;
        public Button AddBeatsButton;
        public Button RemoveBeatsButton;

        // Parallel list is difficult to avoid here. Be careful when managing. Generally, try to use the Writer.
        protected List<EditorFrameController> _frameControllers = new List<EditorFrameController>();

        protected override void InitializeInternal()
        {
            var writer = Editor.BeatmapWriter;
            for (int frameIndex = 0; frameIndex < writer.FrameCount; frameIndex++)
            {
                DoAddController(writer.GetFrame(frameIndex), frameIndex);
            }
        }
        protected override void SubscribeToEventsInternal()
        {
            var writer = Editor.BeatmapWriter;            
            writer.OnFrameCreated += DoAddController;
            writer.OnFrameDestroyed += DoRemoveController;

            AddBeatsButton.onClick.AddListener(OnRequestAddBeats);
            RemoveBeatsButton.onClick.AddListener(OnRequestRemoveBeats);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            var writer = Editor.BeatmapWriter;
            writer.OnFrameCreated -= DoAddController;
            writer.OnFrameDestroyed -= DoRemoveController;

            AddBeatsButton.onClick.RemoveListener(OnRequestAddBeats);
            RemoveBeatsButton.onClick.RemoveListener(OnRequestRemoveBeats);
        }
        protected override void RepaintInternal()
        {
            var currentSubdivision = Editor.SubdivisionComponent.CurrentSubdivision;
            for (int i = 0; i < _frameControllers.Count; i++)
            {
                var thisController = _frameControllers[i];
                PerFrameRepaint(thisController, i, currentSubdivision);
            }
        }
        protected void PerFrameRepaint(EditorFrameController controller, int frameIndex, Subdivision currentSubdivision)
        {
            if (currentSubdivision.IncludesIndex(frameIndex))
            {
                controller.gameObject.SetActive(true);
                controller.SetVisuals(Subdivision.GetHighestSubdivisionOfFrame(frameIndex).Color);
                controller.SetWidth(GetCurrentFrameWidth());
            }
            else
            {
                controller.gameObject.SetActive(false);
            }
        }
        private void OnRequestAddBeats()
        {
            Editor.UndoStackComponent.ExecuteCommand(new InsertBlanksBeatsAtEndCommand(Editor.BeatmapWriter, () => GetBeatQuantity()));
            Editor.SetDirty();
        }
        private void OnRequestRemoveBeats()
        {
            Editor.UndoStackComponent.ExecuteCommand(new RemoveBeatsFromEndCommand(Editor.BeatmapWriter, () => GetBeatQuantity()));
            Editor.SetDirty();
        }
        private void OnRequestFrameValueSet(int frameIndex, int channelIndex, Channel.Value value)
        {
            var writer = Editor.BeatmapWriter;
            var channel = writer.TypeInstance.ChannelFlyweights[channelIndex];
            if (channel.ValueFlyweights.Contains(value))
            {
                var command = new SetChannelValueCommand(writer,
                    () => new SetChannelValueCommand.Args(frameIndex, channelIndex, value));
                
                Editor.UndoStackComponent.ExecuteCommand(command);
                Editor.SetDirty();
            }
        }
        private void OnRequestFrameValueRotate(int frameIndex, int channelIndex)
        {
            var writer = Editor.BeatmapWriter;
            var channel = writer.TypeInstance.ChannelFlyweights[channelIndex];
            var currentValue = writer.GetValue(frameIndex, channelIndex);
            var currentValueIndex = channel.ValueFlyweights.FindIndex(val => val == currentValue);
            var nextValueIndex = (int)Mathf.Repeat(currentValueIndex + 1, channel.ValueFlyweights.Count);
            var nextValue = channel.ValueFlyweights[nextValueIndex];

            var command = new SetChannelValueCommand(writer,
                    () => new SetChannelValueCommand.Args(frameIndex, channelIndex, nextValue));

            Editor.UndoStackComponent.ExecuteCommand(command);
            Editor.SetDirty();
        }
        protected void DoAddController(Frame frame, int frameIndex)
        {
            var controller = Instantiate(FrameControllerPrefab, FrameControllersContainer);
            controller.transform.SetSiblingIndex(frameIndex);
            controller.Initialize(frame);

            controller.SelectFrameButton.onClick.AddListener(() => OnSelectFrameController(controller));
            controller.OnRequestValueSet.AddListener((channelIndex, value) => OnRequestFrameValueSet(frameIndex, channelIndex, value));
            controller.OnRequestValueRotate.AddListener(channelIndex => OnRequestFrameValueRotate(frameIndex, channelIndex));

            _frameControllers.Add(controller);
            Editor.SetDirty();
        }
        protected void DoRemoveController(int frameIndex)
        {
            //maybe use object pooling for performance if we need it

            var controller = _frameControllers[frameIndex];
            _frameControllers.Remove(controller);

            controller.SelectFrameButton.onClick.RemoveAllListeners();

            Destroy(controller.gameObject);  //<-- have to destroy the object, not the controller!

            Editor.SetDirty();
        }
        public float GetCurrentFrameWidth()
        {
            return DEFAULT_WIDTH * Editor.ZoomComponent.CurrentMultiplier;
        }
        private void OnSelectFrameController(EditorFrameController frameController)
        {
            int frameIndex = _frameControllers.IndexOf(frameController);

            Debug.Log("selected controller " + frameIndex);

            if (frameIndex >= 0)    //-1 if not in list
            {
                Editor.SelectionComponent.SelectFrame(frameIndex);
            }
        }
        protected int GetBeatQuantity()
        {
            string strValue = BeatNumberInput.text;
            if (int.TryParse(strValue, out int intValue))
            {
                if (intValue > 1)
                    return intValue;
            }
            return 1;
        }
    }
}
