using System.Collections.Generic;

namespace Beatmap
{
    /// <summary>
    /// Abstract class for readers of beatmap data.
    /// </summary>
    public abstract class BeatmapReader : BeatmapWrapper
    {
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
        public BeatmapReader(IBeatmap beatmap)
            :base(beatmap)
        {
            _channelStates = new List<ChannelStatePair>();
            foreach(var channel in TypeInstance.ChannelFlyweights)
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
        public BeatmapFrame GetModifiedInputFrame(BeatmapFrame playerInputFrame)
        {
            //make a frame clone of the input
            var newFrame = new BeatmapFrame(playerInputFrame);

            //for each channel in the beatmap, modify each value based on the corresponding current channel state
            for (int i = 0; i < TypeInstance.ChannelFlyweights.Count; i++)
            {
                newFrame.Values[i] = _channelStates[i].State.ModifierDelegate(newFrame.Values[i]);
            }

            return newFrame;
        }

        /// <summary>
        /// Accept and apply a beatmap modification (e.g. tempo change (scroll speed), or whatever you like...)
        /// </summary>
        public void AcceptModifier(ReaderModifier modifier)
        {
            modifier.ModifierDelegate?.Invoke(this);
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
}
