using Beatmap.Interfaces;
using System.Collections.Generic;

namespace Beatmap
{
    /// <summary>
    /// Abstract class for readers of beatmap data.
    /// </summary>
    public abstract class BeatmapProcessor
    {
        protected IBeatmap _beatmap;

        /// <summary>
        /// Internal class for managing the state a channel is currently in.
        /// </summary>
        protected class ChannelStatePair
        {
            public Channel Channel { get; set; }
            public Channel.State State { get; set; }
        }
        /// <summary>
        /// Internal list of channel-state pairs. Similar to a Dictionary<TKey, TValue>, but with a strict numerical order to reduce lookup time.
        /// </summary>
        protected readonly List<ChannelStatePair> _channelStates;

        //ctor
        public BeatmapProcessor(IBeatmap beatmap)
        {
            _beatmap = beatmap;

            _channelStates = new List<ChannelStatePair>();
            foreach(var channel in _beatmap.TypeInstance.ChannelFlyweights)
            {
                _channelStates.Add(new ChannelStatePair()
                {
                    Channel = channel,

                    State = channel.DefaultStateFlyweight,
                } );
            }
        }

        /// <summary>
        /// Modify a frame by the current state of each of the channels in the reader.
        /// </summary>
        /// <param name="inputFrame">the input frame (usually the player input on this frame)</param>
        /// <returns>a new frame modified by the current state of the channels</returns>
        public Frame GetModifiedInputFrame(IFrame playerInputFrame)
        {
            var newFrameList = new List<IValueWrapper>(playerInputFrame.GetValues());

            //for each channel in the beatmap, modify each value based on the corresponding current channel state
            for (int i = 0; i < _beatmap.TypeInstance.ChannelFlyweights.Count; i++)
            {
                var modifier = _channelStates[i].State.ModifierDelegate;
                var newValue = modifier(newFrameList[i].GetValue());
                newFrameList[i] = new ValueWrapper(newValue);
            }
            return new Frame(newFrameList);
        }

        /// <summary>
        /// Accept and apply a beatmap modification (e.g. tempo change (scroll speed), or whatever you like...)
        /// </summary>
        public void AcceptModifier(ProcessorModifier modifier)
        {
            modifier.Modifier?.Invoke(this);
        }

        /// <summary>
        /// Get the current state of a given channel in the beatmap.
        /// </summary>
        public Channel.State GetChannelState(Channel beatmapChannel)
        {
            return _channelStates.Find(cs => cs.Channel == beatmapChannel)?.State;
        }
        /// <summary>
        /// Set the current state of a channel in the beatmap.
        /// </summary>
        public void SetChannelState(Channel beatmapChannel, Channel.State state)
        {
            var found = _channelStates.Find(cs => cs.Channel == beatmapChannel);
            if (found != null)
                found.State = state;
        }
    }
    //public class ChannelInstance
    //{
    //    private Channel _template;
    //    private Channel.State _currentState;
    //    private Channel.Value _lastNonEmptyInputValue;
    //    private int _lastNonEmptyInputFrameIndex;
    //    public ChannelInstance(Channel template)
    //    {
    //        _template = template;
    //    }
    //    public void RecordInput(Channel.Value value, int currentFrameIndex)
    //    {
    //        if (value != ChannelValueInstances.Empty)
    //        {
    //            _lastNonEmptyInputValue = value;
    //            _lastNonEmptyInputFrameIndex = currentFrameIndex;
    //        }
    //    }
    //    public float GetInputAccuracy(Channel.Value value, int currentFrameIndex)
    //    {
            
    //    }
    //}
}
