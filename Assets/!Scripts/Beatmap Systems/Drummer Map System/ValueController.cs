using UnityEngine;
namespace Beatmap.Editor
{
    public class ValueController : MonoBehaviour
    {
        protected DataPointController _parent;
        public int FrameIndex => _parent.FrameIndex;
        public int ChannelIndex { get; protected set; }     //the channel # in the channel flyweights
        public Channel.Value Value { get; private set; }     //the currently-represented channel value
        
        public TMPro.TMP_Text Text;

        public void Initialize(DataPointController parent, Channel.Value initialValue, int channelIndex)
        {
            _parent = parent;
            UpdateValue(initialValue);
            ChannelIndex = channelIndex;
        }
        public virtual void UpdateValue(Channel.Value newValue)
        {
            Value = newValue;
            Text.text = newValue.Name;
        }
    }
}