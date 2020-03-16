using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongData
{
    /// <summary>
    /// Represents a type of beatmap (or a type of rhythm game, which is the same thing). Contains references to all channel information.
    /// </summary>
    public class BeatmapType
    {
        private static readonly List<BeatmapType> _allInstances = new List<BeatmapType>();  //static collection of all beatmap types
        public static List<BeatmapType> GetAll()
        {
            BeatmapTypeInstances.PopulateInstances();
            return new List<BeatmapType>(_allInstances);
        }

        public static BeatmapType Get(string name) => _allInstances.Find(bt => bt.Name == name);    //get a beatmap type by its name

        public string Name { get; }     //the name of the beatmap type (like, "Drummer", or "Vocalist")
        public List<Channel> ChannelFlyweights { get; }     //list of instances which can be passed by reference instead of making new channels

        //ctor
        public BeatmapType(string name, List<Channel> channels)
        {
            Name = name;
            ChannelFlyweights = channels;
            _allInstances.Add(this);    //add to the static collection
        }
        /// <summary>
        /// Get a new Frame for this beatmap type with default values on each channel.
        /// </summary>
        /// <returns></returns>
        public Frame GetNewFrameWithDefaults()
        {
            var valuesList = new List<Channel.Value>();
            foreach (var channel in ChannelFlyweights)
            {
                valuesList.Add(channel.DefaultValueFlyweight);
            }

            return new Frame(valuesList);
        }
    }
}
