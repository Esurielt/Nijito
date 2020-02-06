using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap.Interfaces
{
    /// <summary>
    /// Interface for things that behave like a beatmap. Used by beatmap instances as well as beatmap editor visuals.
    /// </summary>
    /// <typeparam name="TMap">the corresponding subclass of BeatmapType (i.e. "What rhythm game is this a map for?")</typeparam>
    public interface IBeatmap
    {
        /// <summary>
        /// Redirect to the correct instance of BeatmapType for data about required channels.
        /// </summary>
        /// <returns></returns>
        BeatmapType TypeInstance { get; }

        /// <summary>
        /// Get a collection of all data points in the beatmap.
        /// </summary>
        List<IDataPoint> DataPoints { get; }

        /// <summary>
        /// The starting BPM of the beatmap.
        /// </summary>
        float StartingBpm { get; }

        /// <summary>
        /// Get the actual audio clip stored in Unity for the song this is a beatmap of.
        /// </summary>
        AudioClip SongAudio { get; }

        /// <summary>
        /// Get the number of miliseconds from the first sample in the audio clip until the first beat.
        /// </summary>
        int StartingDelayInMiliseconds { get; }

        //later, code for song metadata (track title, difficulty, yada yada)
        //like, GetBeatmapMetadata() or something
    }
    public interface IDataPoint
    {
        IFrame GetExpectedFrame();
        void SetExpectedFrame(IFrame frame);
        
        //this should all be contained in a seperate component -_-
        List<ProcessorModifier> GetModifiers();
        void AddModifier(ProcessorModifier modifier);
        void AddModifiers(List<ProcessorModifier> modifiers);
        void RemoveModifier(ProcessorModifier modifier);
        void RemoveModifiers(List<ProcessorModifier> modifiers);
        void ClearModifiers();
        void ReplaceModifiers(List<ProcessorModifier> modifiers);
    }
    public interface IFrame
    {
        int ValueCount { get; }
        IValueWrapper GetValue(int channelIndex);
        List<IValueWrapper> GetValues();
        void SetValue(IValueWrapper value, int channelIndex);
        void SetValues(List<IValueWrapper> values);
    }
    public interface IValueWrapper : System.IEquatable<IValueWrapper>
    {
        Channel.Value GetValue();
        void SetValue(Channel.Value value);
    }
}