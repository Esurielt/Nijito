using System.Collections.Generic;
using UnityEngine;
namespace Beatmap.Editor
{
    public class DataPointController : MonoBehaviour
    {
        public ValueController ValueControllerPrefab;
        public int FrameIndex { get; protected set; }
        
        protected List<ValueController> _expectedFrameValueControllers = new List<ValueController>();
        
        //reminder to make this handle modifiers too!

        public void Initialize(int newFrameIndex, DataPoint dataPoint)
        {
            for (int i = 0; i < dataPoint.ExpectedFrame.Values.Count; i++)
            {
                var controller = GetBlankValueController(i);
                controller.Initialize(this, dataPoint.ExpectedFrame.Values[i], i);
                _expectedFrameValueControllers.Add(controller);
            }

            UpdateFrameIndex(newFrameIndex);
        }
        protected ValueController GetBlankValueController(int channelIndex)
        {
            var newController = Instantiate(ValueControllerPrefab, transform);
            return newController;
        }
        public void UpdateFrameIndex(int newFrameIndex)
        {
            FrameIndex = newFrameIndex;
        }
        public void UpdateChannelValue(Channel.Value newValue, int channelIndex)
        {
            _expectedFrameValueControllers[channelIndex].UpdateValue(newValue);
        }
        public void UpdateChannelValues(DataPoint newDataPoint)
        {
            for (int i = 0; i < _expectedFrameValueControllers.Count; i++)
            {
                _expectedFrameValueControllers[i].UpdateValue(newDataPoint.ExpectedFrame.Values[i]);
            }
        }
        //don't need to remove value controllers, I think
    }
}