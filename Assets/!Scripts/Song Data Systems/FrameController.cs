using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace SongData
{
    public class FrameControllerValueSetEvent : UnityEngine.Events.UnityEvent<int, Channel.Value> { }
    public class FrameController : MonoBehaviour
    {
        //Unity fields
        public ValueController ValueControllerPrefab;
        public RectTransform ValueControllerContainer;

        public Frame Frame { get; protected set; }

        [HideInInspector] public FrameControllerValueSetEvent OnRequestValueSet = new FrameControllerValueSetEvent();

        //internal
        protected List<ValueController> _expectedFrameValueControllers = new List<ValueController>();

        public virtual void Initialize(Frame frame)
        {
            Frame = frame;
            var values = frame.GetValues();

            for (int i = 0; i < values.Count; i++)
            {
                SpawnValueController(values[i], i);
            }
            
            frame.OnChannelValueSet += SetValue;
        }
        protected virtual void OnDestroy()
        {
            OnRequestValueSet.RemoveAllListeners();

            if (Frame != null)
                Frame.OnChannelValueSet -= SetValue;
        }
        protected void SpawnValueController(Channel.Value newValue, int channelIndex)
        {
            var controller = Instantiate(ValueControllerPrefab, ValueControllerContainer);
            controller.SetValue(newValue);
            controller.OnRequestSet.AddListener(value => OnRequestValueSetInternal(channelIndex, value));

            _expectedFrameValueControllers.Add(controller);

            SpawnValueController_Late(newValue, channelIndex, controller);
        }
        protected virtual void SpawnValueController_Late(Channel.Value newValue, int channelIndex, ValueController controller) { /* Nothing. */ }
        protected void OnRequestValueSetInternal(int channelIndex, Channel.Value value)
        {
            OnRequestValueSet?.Invoke(channelIndex, value);
        }
        protected void SetValue(int channelIndex, Channel.Value value)
        {
            _expectedFrameValueControllers[channelIndex].SetValue(value);
        }
    }
}