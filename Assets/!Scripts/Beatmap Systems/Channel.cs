using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
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
            StateFlyweights.Insert(0, DefaultStateFlyweight);

            ValueFlyweights = GetNewChannelValueInstances();
            ValueFlyweights.Insert(0, DefaultValueFlyweight);
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

            public bool Equals(Value other)
            {
                return object.ReferenceEquals(this, other);
            }
        }
    }
}