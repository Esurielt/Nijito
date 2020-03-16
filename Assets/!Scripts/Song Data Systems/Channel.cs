using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongData
{
    /// <summary>
    /// All information about a channel in a beatmap (guitar hero channels: green, red, yellow, blue, orange) (DDR channels: left, up, down, right).
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Player-facing name of the channel (e.g. "Center Drum").
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
        /// Redirect to the state flyweight considered the 'default' one (starts as the 'Normal' state instance in ChannelStateInstances), but can be overridden.
        /// </summary>
        public State DefaultStateFlyweight => ChannelStateInstances.Normal;
        /// <summary>
        /// Redirect to the value flyweight considered the 'default' one (starts as the 'Empty' value instance in ChannelValueInstances), but can be overridden.
        /// </summary>
        public Value DefaultValueFlyweight => ChannelValueInstances.Empty;

        //ctor
        public Channel(string name, List<State> states, List<Value> values)
        {
            Name = name;

            StateFlyweights = states;
            StateFlyweights.Insert(0, DefaultStateFlyweight);

            ValueFlyweights = values;
            ValueFlyweights.Insert(0, DefaultValueFlyweight);
        }

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
        /// <summary>
        /// A possible value of the channel. Think: Stepmania's 'Step', 'Hold', or 'Bomb' values
        /// </summary>
        public class Value : System.IEquatable<Value>
        {
            /// <summary>
            /// Player-facing name of the channel value (e.g. "Begin Hold").
            /// </summary>
            public string Name { get; }

            public Value(string name)
            {
                Name = name;
            }

            /// <summary>
            /// Compare two channel values and determine if they are equal. For most values, this will be an object reference comparison (memory address), but later games may need to use unique values.
            /// </summary>
            /// <param name="other">the other channel value</param>
            /// <returns>true if both values are equal</returns>
            public bool Equals(Value other)
            {
                return object.ReferenceEquals(this, other);
            }
        }
    }
}