using System.Collections.Generic;
using System.Linq;

namespace SongData
{
    /// <summary>
    /// One beatmap frame's worth of channel values in a beatmap (all channel values for 1/24th of a beat). Used for expected values, but can be used for player input values as well.
    /// </summary>
    public class Frame
    {
        private Channel.Value[] _valueArray;    //backing field for GetValues() and SetValues()
        public int ValueCount => _valueArray.Length;

        public event System.Action<int, Channel.Value> OnChannelValueSet;
        
        public Frame()
        {
            _valueArray = new Channel.Value[0];
        }
        public Frame(List<Channel.Value> values)
        {
            _valueArray = values.ToArray();
        }
        public Frame(Frame other)  //clone constructor
            : this(other.GetValues()) { }

        public Channel.Value GetValue(int channelIndex)
        {
            return _valueArray[channelIndex];   //index out of range exception is a workable fail state here
        }
        public void SetValue(int channelIndex, Channel.Value value)
        {
            _valueArray[channelIndex] = value;  //index out of range exception is a workable fail state here
            OnChannelValueSet?.Invoke(channelIndex, value);
        }
        public List<Channel.Value> GetValues()
        {
            return _valueArray.ToList();
        }
        public void SetValues(List<Channel.Value> values)
        {
            for (int i = 0; i < _valueArray.Length; i++)
            {
                SetValue(i, values[i]);
            }
        }
    }
}