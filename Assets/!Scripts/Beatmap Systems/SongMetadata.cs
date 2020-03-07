using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    //requires serializable version
    public class SongMetadata
    {
        private readonly Dictionary<Category, string> _allData = new Dictionary<Category, string>();

        public SongMetadata()
        {
            foreach (var category in System.Enum.GetValues(typeof(Category)))
            {
                _allData.Add((Category)category, "");
            }
        }
        public SongMetadata(SongMetadata other)
        {
            _allData = new Dictionary<Category, string>(other._allData);
        }
        public string GetValue(Category category)
        {
            if (_allData.ContainsKey(category))
                return _allData[category];
            else
            {
                _allData.Add(category, "");
                return "";
            }
        }
        public void SetValue(Category category, string newValue)
        {
            if (_allData.ContainsKey(category))
                _allData[category] = newValue;
            else
                _allData.Add(category, newValue);
        }

        public enum Category
        {
            ORIGINAL_TITLE = 0,
            ENGLISH_TITLE = 1,
            ARTISTS = 2,
            YEAR = 3,
            BEATMAP_SEQUENCERS = 4,
        }
    }
}