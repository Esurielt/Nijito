using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SongData.Serialization
{
    [System.Serializable]
    public class SerializableSongMetadata
    {
        public SerializableKeyValuePair[] SongMetadataItems;
        public SerializableSongMetadata(SongMetadata songMetadata)
        {
            SongMetadataItems =
                songMetadata.GetAllValuesCopy().Select(
                    kvp => new SerializableKeyValuePair()
                    {
                        Key = kvp.Key, Value = kvp.Value
                    }).ToArray();
        }
        public SongMetadata GetAsSongMetadata()
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in SongMetadataItems)
            {
                dict.Add(item.Key, item.Value);
            }

            return new SongMetadata(dict);
        }
    }
}