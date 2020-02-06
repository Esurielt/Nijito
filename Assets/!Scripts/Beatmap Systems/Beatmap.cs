using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beatmap.Interfaces;

namespace Beatmap
{
    public abstract class Beatmap : IBeatmap
    {
        protected List<IDataPoint> _dataPoints = new List<IDataPoint>();

        public abstract BeatmapType TypeInstance { get; }
        public List<IDataPoint> DataPoints => _dataPoints;  //<-- specifically not a cloned list
        public float StartingBpm { get; set; }
        public AudioClip SongAudio { get; set; }
        public int StartingDelayInMiliseconds { get; set; }
    }
    [System.Serializable]
    public class BeatmapData
    {
        public readonly string TypeName;
        public readonly DataPoint[] DataPoints;
        public readonly float StartingBpm;
        public readonly int StartingDelayInMiliseconds;
        public BeatmapData(IBeatmap beatmap)
        {
            TypeName = beatmap.TypeInstance.Name;
            List<DataPoint> dataPointList = new List<DataPoint>();
            for (int i = 0; i < beatmap.DataPoints.Count; i++)
            {
                dataPointList.Add(new DataPoint(beatmap.DataPoints[i]));
            }
            DataPoints = dataPointList.ToArray();

            StartingBpm = beatmap.StartingBpm;
            StartingDelayInMiliseconds = beatmap.StartingDelayInMiliseconds;
        }
    }
}