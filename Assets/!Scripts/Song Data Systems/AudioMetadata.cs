using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongData
{
    public delegate bool ValueParser<TValue>(string input, out TValue value);
    public class AudioMetadata
    {
        private float _startingDelay;
        public float StartingDelay
        {
            get => _startingDelay;
            set
            {
                if (_startingDelay != value)
                {
                    _startingDelay = value;
                    OnStartingDelaySet?.Invoke(value);
                }
            }
        }
        public ValueHelper<float> BpmChanges;
        public ValueHelper<string> SectionNames;

        public System.Action<float> OnStartingDelaySet;

        public AudioMetadata()
        {
            _startingDelay = 0;
            BpmChanges = new ValueHelper<float>(150, new Dictionary<int, float>());
            SectionNames = new ValueHelper<string>("Intro", new Dictionary<int, string>());
        }
        public AudioMetadata(AudioMetadata other)
        {
            _startingDelay = other.StartingDelay;
            BpmChanges = new ValueHelper<float>(other.BpmChanges);
            SectionNames = new ValueHelper<string>(other.SectionNames);
        }
        public AudioMetadata(float startingDelay, ValueHelper<float> bpmChanges, ValueHelper<string> sectionNames)
        {
            _startingDelay = startingDelay;
            BpmChanges = bpmChanges;
            SectionNames = sectionNames;
        }
        public class ValueHelper<TValue>
        {
            private TValue _startingValue;
            public TValue StartingValue
            {
                get => _startingValue;
                set
                {
                    if (!_startingValue.Equals(value))
                    {
                        _startingValue = value;
                        OnStartingValueSet?.Invoke(value);
                    }
                }
            }
            protected readonly Dictionary<int, TValue> _valuesDict = new Dictionary<int, TValue>(); //beat index to value
            public Dictionary<int, TValue> GetAllValuesCopy()
            {
                return new Dictionary<int, TValue>(_valuesDict);
            }

            protected readonly ValueParser<TValue> _parser;

            public event System.Action<TValue> OnStartingValueSet;
            public event System.Action<int, TValue> OnValueAdded;
            public event System.Action<int, TValue> OnValueSet;
            public event System.Action<int> OnValueRemoved;

            public ValueHelper(TValue startingValue, Dictionary<int, TValue> valuesDict)
            {
                _startingValue = startingValue;
                _valuesDict = new Dictionary<int, TValue>(valuesDict);
            }
            public ValueHelper(ValueHelper<TValue> other)
                : this(other.StartingValue, other._valuesDict) { }

            public TValue GetCurrentValue(int atBeatIndex)
            {
                KeyValuePair<int, TValue> lastValue = new KeyValuePair<int, TValue>(0, StartingValue);
                foreach (var value in _valuesDict)
                {
                    if (value.Key > atBeatIndex)
                        break;

                    lastValue = value;
                }
                return lastValue.Value;
            }
            public bool TryAddValue(int atBeatIndex, TValue newValue)
            {
                if (!_valuesDict.ContainsKey(atBeatIndex))
                {
                    _valuesDict.Add(atBeatIndex, newValue);
                    OnValueAdded?.Invoke(atBeatIndex, newValue);
                    return true;
                }
                else
                {
                    Game.LogFormat(Logging.Category.SONG_DATA, "Value helper already has a value at index {0}", Logging.Level.LOG_WARNING, atBeatIndex);
                    return false;
                }
            }
            public bool TrySetValue(int atBeatIndex, TValue newValue)
            {
                if (_valuesDict.ContainsKey(atBeatIndex))
                {
                    _valuesDict[atBeatIndex] = newValue;
                    OnValueSet?.Invoke(atBeatIndex, newValue);
                    return true;
                }
                else
                {
                    Game.LogFormat(Logging.Category.SONG_DATA, "Value helper does not have a value at index {0}", Logging.Level.LOG_WARNING, atBeatIndex);
                    return false;
                }
            }
            public bool TryRemoveValue(int atBeatIndex)
            {
                if (_valuesDict.ContainsKey(atBeatIndex))
                {
                    _valuesDict.Remove(atBeatIndex);
                    OnValueRemoved?.Invoke(atBeatIndex);
                    return true;
                }
                else
                {
                    Game.LogFormat(Logging.Category.SONG_DATA, "Value helper does not have a value at index {0}", Logging.Level.LOG_WARNING, atBeatIndex);
                    return false;
                }
            }
        }
    }
}