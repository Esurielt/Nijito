using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Beatmap.Drummer
{
    /// <summary>
    /// Concrete class for the Drummer beatmap type. Use stored instance reference at BeatmapTypes.Drummer.
    /// </summary>
    public class BeatmapType_Drummer : BeatmapType
    {
        protected override List<Channel> GetNewChannelInstances()
        {
            return new List<Channel>
            {
                //kick drum channel
                new DrummerChannel("Kick"),

                //snare drum channel
                new DrummerChannel("Snare"),
            };
        }
    }
    /// <summary>
    /// Concrete class for beatmaps of the Drummer rhythm game type.
    /// </summary>
    public class DrummerMap : IBeatmap<BeatmapType_Drummer>
    {
        #region Interface Compliance
        List<DataPoint> IBeatmap.GetDataPoints()
        {
            return DataPoints;
        }

        int IBeatmap.GetMilisecondsUntilFirstBeat()
        {
            return MilisecondDelay;
        }

        AudioClip IBeatmap.GetSongAudio()
        {
            return SongAudio;
        }

        float IBeatmap.GetStartingBpm()
        {
            return BeatsPerMinute;
        }
        #endregion

        public AudioClip SongAudio;
        public float BeatsPerMinute;
        public int MilisecondDelay;
        public List<DataPoint> DataPoints = new List<DataPoint>();
    }
    /// <summary>
    /// Concrete class for channels in the Drummer beatmap type.
    /// </summary>
    public class DrummerChannel : Channel
    {
        //ctor
        public DrummerChannel(string name) : base(name) { }

        protected override List<State> GetNewChannelStateInstances()
        {
            return new List<State>()
            {
                //empty
            };
        }
        protected override List<Value> GetNewChannelValueInstances()
        {
            return new List<Value>()
            {
                ChannelValueInstances.Hit,
            };
        }
    }
}
