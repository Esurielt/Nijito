using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongData
{
    public class BeatmapDifficulty
    {
        public readonly string Name;
        public readonly int StarCount;  //lol
        public BeatmapDifficulty(string name, int starCount)
        {
            Name = name;
            StarCount = starCount;
            
            _allDifficulties.Add(this);
        }

        private static readonly List<BeatmapDifficulty> _allDifficulties = new List<BeatmapDifficulty>();
        public static BeatmapDifficulty Easy = new BeatmapDifficulty("Easy", 1);
        public static BeatmapDifficulty Medium = new BeatmapDifficulty("Medium", 3);
        public static BeatmapDifficulty Hard = new BeatmapDifficulty("Hard", 5);

        public static BeatmapDifficulty Get(string name) => _allDifficulties.Find(d => d.Name == name);
        public static List<BeatmapDifficulty> GetAll() => new List<BeatmapDifficulty>(_allDifficulties);
    }
}