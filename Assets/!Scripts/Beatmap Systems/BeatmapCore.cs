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
        /// <summary>
        /// List of reuseable instances for each channel of the beatmap.
        /// </summary>
        public List<Channel> ChannelFlyweights { get; }
        
        //ctor
        public BeatmapType()
        {
            ChannelFlyweights = GetNewChannelInstances();
        }
        /// <summary>
        /// Supply a new List of channels to populate the flyweights list. This is called only once in the constructor. (If you need these values, use the flyweights.)
        /// </summary>
        protected abstract List<Channel> GetNewChannelInstances();
        /// <summary>
        /// Get a float between 0 and 1 representing the relative accuracy of one data frame compared to another (e.g. "Is getting 3 out of 4 notes 0% or 75% correct?").
        /// </summary>
        /// <returns>float between 0 and 1. 0 indicates total failure, 1 indicates total success.</returns>
        public virtual float JudgeAccuracy(BeatmapFrame x, BeatmapFrame y)
        {
            //default version of the method requires absolute perfection.

            if (x.Values.Count != y.Values.Count)
                return 0;

            for (int i = 0; i < x.Values.Count; i++)
            {
                if (x.Values[i] != y.Values[i])
                    return 0;
            }
            return 1;
        }

    }
    /// <summary>
    /// Interface for a beatmap. The actual collection of data points and initialization data for a song's beat sequence.
    /// </summary>
    /// <typeparam name="TMap">the corresponding subclass of BeatmapType (i.e. "What rhythm game is this a map for?")</typeparam>
    public interface IBeatmap
    {
        /// <summary>
        /// Get a collection of all data points in the beatmap.
        /// </summary>
        List<DataPoint> GetDataPoints();

        /// <summary>
        /// The starting BPM of the beatmap.
        /// </summary>
        float GetStartingBpm();

        /// <summary>
        /// Get the actual audio clip stored in Unity for the song this is a beatmap of.
        /// </summary>
        AudioClip GetSongAudio();

        /// <summary>
        /// Get the number of miliseconds from the first sample in the audio clip until the first beat.
        /// </summary>
        int GetMilisecondsUntilFirstBeat();

        //later, code for song metadata (track title, difficulty, yada yada)
    }
    /// <summary>
    /// Interface for a beatmap. The actual collection of data points and initialization data for a song's beat sequence.
    /// </summary>
    /// <typeparam name="TMap">the corresponding subclass of BeatmapType (i.e. "What rhythm game is this a map for?")</typeparam>
    public interface IBeatmap<TMap> : IBeatmap { }
    /// <summary>
    /// A collection of all data at a given frame index in a beatmap (expected channel values and any beatmap reader modifications).
    /// </summary>
    public class DataPoint
    {
        /// <summary>
        /// Get the player input that is expected on this frame.
        /// </summary>
        public BeatmapFrame ExpectedFrame { get; }

        /// <summary>
        /// Get all modifications that are triggered on this frame.
        /// </summary>
        public List<ReaderModifier> Modifiers { get; }

        public DataPoint(BeatmapFrame expectedFrame, List<ReaderModifier> modifiers)
        {
            ExpectedFrame = expectedFrame;
            Modifiers = modifiers;
        }
        public DataPoint(DataPoint other)
        {
            ExpectedFrame = new BeatmapFrame(other.ExpectedFrame);
            Modifiers = new List<ReaderModifier>(other.Modifiers);
        }
    }
    /// <summary>
    /// One frame's worth of channel values in a beatmap (all channel values for 1/24th of a beat). Used for both expected values and reading current values.
    /// </summary>
    public class BeatmapFrame
    {
        /// <summary>
        /// All channel values for this frame.
        /// </summary>
        public List<Channel.Value> Values { get; private set; }

        public BeatmapFrame(BeatmapType beatmapTypeInstance)
        {
            Values = new List<Channel.Value>();
            for (int i = 0; i < beatmapTypeInstance.ChannelFlyweights.Count; i++)
            {
                Values.Add(beatmapTypeInstance.ChannelFlyweights[i].DefaultValueFlyweight);
            }
        }
        public BeatmapFrame(BeatmapFrame other)
        {
            Values = new List<Channel.Value>(other.Values);
        }
    }
    /// <summary>
    /// Abstract class for a beatmap channel (e.g. KICK_DRUM, or STRING_4).
    /// A channel represents one line in a catch-the-notes rhythm game (like 'Green' or 'Blue' in Guitar Hero), or the one channel 'Finger Position' in Osu!.
    /// </summary>
    public abstract class Channel
    {
        /// <summary>
        /// Player-facing name of the channel (e.g. "Kick Drum").
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of reuseable instances for each possible state of the channel. Do not include the default state in this list.
        /// </summary>
        public List<State> StateFlyweights { get; }
        /// <summary>
        /// List of reuseable instances for each possible value of the channel. Do not include the default value in this list.
        /// </summary>
        public List<Value> ValueFlyweights { get; }

        /// <summary>
        /// Redirect to the state flyweight considered the 'default' one (starts as the universal 'Normal' state instance in ChannelStateInstances).
        /// </summary>
        public virtual State DefaultStateFlyweight => ChannelStateInstances.Normal;
        /// <summary>
        /// Redirect to the value flyweight considered the 'default' one (starts as the universal 'Empty' value instance in ChannelValueInstances).
        /// </summary>
        public virtual Value DefaultValueFlyweight => ChannelValueInstances.Empty;

        //ctor
        public Channel(string name)
        {
            Name = name;
            StateFlyweights = GetNewChannelStateInstances();
            ValueFlyweights = GetNewChannelValueInstances();
        }
        /// <summary>
        /// Supply a new List of channel states to populate the flyweights list. This is called only once in the constructor. (If you need these values, use the flyweights.)
        /// </summary>
        protected abstract List<State> GetNewChannelStateInstances();
        /// <summary>
        /// Supply a new List of channel values to populate the flyweights list. This is called only once in the constructor. (If you need these values, use the flyweights.)
        /// </summary>
        protected abstract List<Value> GetNewChannelValueInstances();

        /// <summary>
        /// A possible state of the channel. Think: Guitar Hero's 'Broken String' state in battle mode (all notes on the channel fail until the player hits the string enough times to fix it).
        /// </summary>
        public class State
        {
            /// <summary>
            /// Player-facing name of the channel state (e.g. "Broken String").
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// Delegate method for modifying the player input value (e.g. player input was 'Hit', change it to 'Empty')
            /// </summary>
            public ValueModifierDelegate ModifierDelegate { get; }

            //ctor
            public State(string name, ValueModifierDelegate modifier)
            {
                Name = name;
                ModifierDelegate = modifier;
            }
        }
        /// <summary>
        /// Delegate for methods that modify channel values. Used by channel states to affect player inputs.
        /// </summary>
        /// <param name="value">input value</param>
        /// <returns>modified value</returns>
        public delegate Value ValueModifierDelegate(Value value);
        public class Value : System.IEquatable<Value>
        {
            public string Name { get; }
            public Value(string name)
            {
                Name = name;
            }

            public virtual bool Equals(Value other)
            {
                return object.ReferenceEquals(this, other);
            }
        }
    }
    /// <summary>
    /// Delegate for methods that modify the internal state of a beatmap reader.
    /// </summary>
    /// <param name="reader">the beatmap reader instance to modify</param>
    public delegate void ReaderModifierDelegate(BeatmapReader reader);
    /// <summary>
    /// A modifier to the current state of a beatmap reader (e.g. setting a new tempo, marking a new song section, etc...)
    /// </summary>
    public class ReaderModifier
    {
        /// <summary>
        /// Player-facing name of the reader modifier (e.g. "Set New BPM").
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Delegate method for modifying a beatmap reader instance (i.e. change current bpm, or change current section name).
        /// </summary>
        public ReaderModifierDelegate ModifierDelegate { get; }

        //ctor
        public ReaderModifier(string name, ReaderModifierDelegate modifier)
        {
            Name = name;
            ModifierDelegate = modifier;
        }
    }
}
