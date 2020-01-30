using System.Collections.Generic;
using UnityEngine;
namespace Beatmap.UI
{
    public class DataPointController : MonoBehaviour
    {
        public ValueController ValueControllerPrefab;
        public TMPro.TMP_Text FrameIndexText;
        protected List<ValueController> _expectedFrameValueControllers = new List<ValueController>();
        
        //reminder to make modifiers too

        protected BeatmapType _typeInstance;
        public void Initialize(int frameIndex, BeatmapType typeInstance, DataPoint dataPoint)
        {
            _typeInstance = typeInstance;
            for (int i = 0; i < dataPoint.ExpectedFrame.Values.Count; i++)
            {
                var newController = Instantiate(ValueControllerPrefab, transform);
                newController.Initialize(frameIndex, i, dataPoint.ExpectedFrame.Values[i]);
                newController.Text.text = dataPoint.ExpectedFrame.Values[i].Name;
                _expectedFrameValueControllers.Add(newController);
            }

            FrameIndexText.text = frameIndex.ToString();
        }
        public DataPoint GetDataPoint()
        {
            var newFrame = new BeatmapFrame(_typeInstance);
            for (int i = 0; i < _typeInstance.ChannelFlyweights.Count; i++)
            {
                newFrame.Values[i] = _expectedFrameValueControllers[i].Value;
            }
            
            //reminder to modifiers

            return new DataPoint(newFrame, new List<ReaderModifier>());
        }
        public Channel.Value GetValueAtChannelIndex(int channelIndex)
        {
            if (channelIndex >= 0 && channelIndex < _expectedFrameValueControllers.Count)
                return _expectedFrameValueControllers[channelIndex].Value;
            return null;
        }
        public void SetChannelValueAtIndex(int channelIndex, Channel.Value value)
        {
            if (channelIndex >= 0 && channelIndex < _expectedFrameValueControllers.Count)
            {
                _expectedFrameValueControllers[channelIndex].UpdateValue(value, value.Name);
            }
        }
    }
}