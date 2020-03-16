using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongData.Serialization
{
    [System.Serializable]
    public class SerializableAudioMetadata
    {
        public float StartingDelay;
        public SerializableBpmChanges BpmChanges;
        public SerializableSectionNames SectionNames;

        public SerializableAudioMetadata(AudioMetadata audioMetadata)
        {
            StartingDelay = audioMetadata.StartingDelay;
            BpmChanges = new SerializableBpmChanges(audioMetadata.BpmChanges);
            SectionNames = new SerializableSectionNames(audioMetadata.SectionNames);
        }
        public AudioMetadata GetAsAudioMetadata()
        {
            var startingDelay = StartingDelay;
            var bpmChanges = BpmChanges.GetAsValueHelper();
            var sectionNames = SectionNames.GetAsValueHelper();
            return new AudioMetadata(startingDelay, bpmChanges, sectionNames);
        }
    }
    public abstract class AbstractSerializableValueHelper<TValue>
    {
        public string StartingValue;
        public SerializableKeyValuePair[] IndexesWithValues;

        public AbstractSerializableValueHelper(AudioMetadata.ValueHelper<TValue> valueHelper)
        {
            StartingValue = valueHelper.StartingValue.ToString();
            var otherKvps = new List<KeyValuePair<int, TValue>>(valueHelper.GetAllValuesCopy());
            IndexesWithValues = new SerializableKeyValuePair[otherKvps.Count];
            for (int i = 0; i < otherKvps.Count; i++)
            {
                IndexesWithValues[i] = ToSerializable(otherKvps[i]);
            }
        }
        public AudioMetadata.ValueHelper<TValue> GetAsValueHelper()
        {
            var dict = new Dictionary<int, TValue>();
            foreach (var skvp in IndexesWithValues)
            {
                int beatIndex = ParseInt(skvp.Key);
                TValue value = ParseValue(skvp.Value);

                if (!dict.ContainsKey(beatIndex))
                    dict.Add(beatIndex, value);
            }

            return new AudioMetadata.ValueHelper<TValue>(ParseValue(StartingValue), dict);
        }
        protected SerializableKeyValuePair ToSerializable(KeyValuePair<int, TValue> kvp)
        {
            return new SerializableKeyValuePair() { Key = kvp.Key.ToString(), Value = kvp.Value.ToString() };
        }
        protected KeyValuePair<int, TValue> ToKvp(SerializableKeyValuePair serializable)
        {
            return new KeyValuePair<int, TValue>(ParseInt(serializable.Key), ParseValue(serializable.Value));
        }
        protected int ParseInt(string str)
        {
            int.TryParse(str, out int f);
            return f;
        }
        protected abstract TValue ParseValue(string str);
    }
    [System.Serializable]
    public class SerializableBpmChanges : AbstractSerializableValueHelper<float>
    {
        public SerializableBpmChanges(AudioMetadata.ValueHelper<float> valueHelper) : base(valueHelper) { }
        protected override float ParseValue(string str) => float.Parse(str);
    }
    [System.Serializable]
    public class SerializableSectionNames : AbstractSerializableValueHelper<string>
    {
        public SerializableSectionNames(AudioMetadata.ValueHelper<string> valueHelper) : base(valueHelper) { }
        protected override string ParseValue(string str) => str;
    }
}
