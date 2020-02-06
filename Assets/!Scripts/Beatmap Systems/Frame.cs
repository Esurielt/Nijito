using System.Collections.Generic;
using System.Linq;
using Beatmap.Interfaces;

namespace Beatmap
{
    /// <summary>
    /// One frame's worth of channel values in a beatmap (all channel values for 1/24th of a beat). Used for both expected values and reading current values.
    /// </summary>
    public class Frame : IFrame
    {
        /// <summary>
        /// All channel values for this frame.
        /// </summary>
        private Channel.Value[] _valueArray;
        public int ValueCount => _valueArray.Length;

        public Frame()
        {
            _valueArray = new Channel.Value[0];
        }
        public Frame(List<Channel.Value> values)
        {
            _valueArray = values.ToArray();
        }
        public Frame(List<IValueWrapper> values)
        {
            _valueArray = values.Select(v => v.GetValue()).ToArray();
        }
        public Frame(IFrame other)  //clone
            : this(other.GetValues()) { }

        public IValueWrapper GetValue(int channelIndex)
        {
            return new ValueWrapper(_valueArray[channelIndex]);   //index out of range exception is a workable fail state here
        }

        public void SetValue(IValueWrapper value, int channelIndex)
        {
            _valueArray[channelIndex] = value.GetValue();  //index out of range exception is a workable fail state here
        }

        public List<IValueWrapper> GetValues()
        {
            return new List<IValueWrapper>(_valueArray.Select(v => new ValueWrapper(v)));
        }

        public void SetValues(List<IValueWrapper> values)
        {
            _valueArray = values.Select(v => v.GetValue()).ToArray();
        }
    }
}