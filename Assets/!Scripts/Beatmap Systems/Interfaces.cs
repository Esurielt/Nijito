using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap.Interfaces
{
    /// <summary>
    /// Interface for things that behave like a Beatmap. Used by beatmap instances as well as beatmap editor visuals.
    /// </summary>
    /// <typeparam name="TMap">the corresponding instance of BeatmapType (i.e. "What rhythm game is this a map for?")</typeparam>
    public interface IBeatmap
    {
        /// <summary>
        /// Redirect to the correct instance of BeatmapType which holds data about Channels.
        /// </summary>
        /// <returns>The type instance reference for this beatmap.</returns>
        BeatmapType TypeInstance { get; }

        /// <summary>
        /// Get a collection of all data points in the beatmap. This should always be a reference to the existing collection, not a new collection.
        /// </summary>
        List<Frame> Frames { get; }
    }
    /// <summary>
    /// Interface for things that behave like a Frame. Used by frame instances as well as datapoint controller objects in beatmap editors.
    /// </summary>
    public interface IFrame
    {
        /// <summary>
        /// Get the number of channel values in the frame (this should always be the same as the number of channels in the beatmap type).
        /// </summary>
        int ValueCount { get; }
        /// <summary>
        /// Get the channel value at a given index (getting the 3rd value will get you the value for the 3rd channel in the beatmap type). This comes as a wrapper around the value so that other objects may be used to represent channel values.
        /// </summary>
        /// <param name="channelIndex">The index of the channel whose value is desired</param>
        /// <returns></returns>
        Channel.Value GetValue(int channelIndex);
        /// <summary>
        /// Get a collection of all channel values in the frame. The count should always be the same as the number of channels in the beatmap type.
        /// </summary>
        /// <returns>A new collection of wrapped channel values</returns>
        List<Channel.Value> GetValues();
        /// <summary>
        /// Set the channel value at a given index.
        /// </summary>
        /// <param name="channelIndex">the index of the channel whose value will be changed on this frame</param>
        /// <param name="value">a wrapper around the new channel value instance reference</param>
        void SetValue(int channelIndex, Channel.Value value);
        /// <summary>
        /// Set all the values in this frame. This should always have a count equal to the number of channels in the beatmap type.
        /// </summary>
        /// <param name="values">a collection of wrappers around new channel value instance references</param>
        void SetValues(List<Channel.Value> values);
    }
}