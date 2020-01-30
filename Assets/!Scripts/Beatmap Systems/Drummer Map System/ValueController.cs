using UnityEngine;
namespace Beatmap.UI
{
    public class ValueController : MonoBehaviour
    {
        public int FrameIndex { get; protected set; }       //the frame index in the beatmap
        public int ChannelIndex { get; protected set; }     //the channel # in the channel flyweights
        public Channel.Value Value { get; set; }     //the currently-represented channel value
        
        public TMPro.TMP_Text Text;

        public void Initialize(int frameIndex, int channelIndex, Channel.Value initialValue)
        {
            FrameIndex = frameIndex;
            ChannelIndex = channelIndex;
            Value = initialValue;
        }
        public void UpdateValue(Channel.Value newValue, string text)
        {
            Value = newValue;
            Text.text = text;
        }
    }
}