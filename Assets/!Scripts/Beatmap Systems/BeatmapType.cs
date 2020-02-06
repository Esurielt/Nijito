using Beatmap.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    /// <summary>
    /// Abstract class for a beatmap type (e.g. GUITARIST, VOCALIST, or DRUMMER). Synonymous with 'Rhythm Game Type'
    /// Contains flyweight references to all channels used by this particular beatmap type.
    /// </summary>
    public abstract class BeatmapType
    {
        public string Name { get; }
        /// <summary>
        /// List of reuseable instances for each channel of the beatmap.
        /// </summary>
        public List<Channel> ChannelFlyweights { get; }

        //ctor
        public BeatmapType()
        {
            ChannelFlyweights = GetNewChannelInstances();
            BeatmapTypeInstances.AllInstances.Add(this);
        }
        /// <summary>
        /// Supply a new List of channels to populate the flyweights list. This is called only once in the constructor. (If you need these values, use the flyweights.)
        /// </summary>
        protected abstract List<Channel> GetNewChannelInstances();

        public abstract Beatmap GetNewEmptyBeatmap();
        public Frame GetNewFrameWithDefaults()
        {
            var valuesList = new List<Channel.Value>();
            foreach (var channel in ChannelFlyweights)
            {
                valuesList.Add(channel.DefaultValueFlyweight);
            }

            return new Frame(valuesList);
        }
        public bool GetNewBeatmapFromData(BeatmapData beatmapData, out Beatmap beatmap)
        {
            beatmap = GetNewEmptyBeatmap();

            if (beatmapData.TypeName != Name)
                return false;

            beatmap.DataPoints.AddRange(beatmapData.DataPoints);
            beatmap.StartingBpm = beatmapData.StartingBpm;
            beatmap.StartingDelayInMiliseconds = beatmapData.StartingDelayInMiliseconds;
            return true;
        }

        /// <summary>
        /// Get a float between 0 and 1 representing the relative accuracy of one data frame compared to another (e.g. "Is getting 3 out of 4 notes 0% or 75% correct?").
        /// </summary>
        /// <returns>float between 0 and 1. 0 indicates total failure, 1 indicates total success.</returns>
        public virtual float JudgeAccuracy(Frame x, Frame y)    //<-- move this to a game class
        {
            //default version of the method requires absolute perfection.

            if (x.ValueCount != y.ValueCount)
                return 0;

            for (int i = 0; i < x.GetValues().Count; i++)
            {
                if (x.GetValue(i) != y.GetValue(i))
                    return 0;
            }
            return 1;
        }

    }
}
