using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongData
{
    public class SongMetadata
    {
        private static readonly List<string> _categories = new List<string>()
        {
            "ORIGINAL_TITLE",
            "ORIGINAL_SUBTITLE",
            "ENGLISH_TITLE",
            "ENGLISH_SUBTITLE",
            "ARTISTS",
            "ALBUM",
            "YEAR",
            "COLLECTION",
            "COLLECTION_URL",
            "BEATMAP_SEQUENCERS",
            "COMMENTS",
        };
        public static List<string> GetAllCategoriesCopy() => new List<string>(_categories);


        private readonly Dictionary<string, string> _allValues = new Dictionary<string, string>();
        public Dictionary<string, string> GetAllValuesCopy() => new Dictionary<string, string>(_allValues);

        public delegate void SetValueEventDelegate(string category, string value);
        public event SetValueEventDelegate OnSetValue;
        public SongMetadata()
        {
            foreach (var category in _categories)
            {
                _allValues.Add(category, "");
            }
        }
        public SongMetadata(SongMetadata other)
        {
            _allValues = new Dictionary<string, string>(other._allValues);
        }
        public SongMetadata(Dictionary<string, string> dict)
        {
            _allValues = new Dictionary<string, string>(dict);
        }

        public string GetOrCreateValue(string category)
        {
            if (_allValues.ContainsKey(category))
                return _allValues[category];
            else
            {
                _allValues.Add(category, "");
                return "";
            }
        }
        public void SetValue(string category, string newValue)
        {
            var oldValue = GetOrCreateValue(category);
            
            if (!string.Equals(oldValue, newValue, System.StringComparison.InvariantCulture))
            {
                // Will add new values or update existing ones.
                _allValues[category] = newValue;
                OnSetValue?.Invoke(category, newValue);
            }
        }
    }
}