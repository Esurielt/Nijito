using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    /// <summary>
    /// Abstract class for objects that read from or write to beatmaps.
    /// </summary>
    public abstract class BeatmapWrapper
    {
        /// <summary>
        /// The encapsulated beatmap to read/write.
        /// </summary>
        public IBeatmap Beatmap { get; }
        /// <summary>
        /// The number of frames in the beatmap.
        /// </summary>
        public int FrameCount => Beatmap.GetDataPoints().Count;
        /// <summary>
        /// The current index being read/written.
        /// </summary>
        public int FrameIndex { get; private set; }
        /// <summary>
        /// Redirect to the appropriate instance of beatmap type (i.e. BeatmapTypes.[Your Rhythm Game Type])
        /// </summary>
        public abstract BeatmapType TypeInstance { get; }

        public BeatmapWrapper(IBeatmap beatmap)
        {
            Beatmap = beatmap;
        }

        /// <summary>
        /// Get the data point at the current frame index in the reader/writer (the one being read/written to).
        /// </summary>
        /// <param name="incrementIndex">if true, move the current frame index forward by 1.</param>
        /// <returns>the current data point before optional increment</returns>
        public DataPoint GetCurrentDataPoint(bool incrementIndex)
        {
            var dataPoint = Beatmap.GetDataPoints()[FrameIndex];
            if (incrementIndex)
                SetFrameIndex(FrameIndex + 1);
            return dataPoint;
        }
        public void ResetFrameIndex()
        {
            SetFrameIndex(0);
        }
        public void SetFrameIndex(int frameIndex)
        {
            if (frameIndex >= 0 && frameIndex < Beatmap.GetDataPoints().Count)
            {
                FrameIndex = frameIndex;
            }
            else
            {
                Game.Log(Logging.Category.BEATMAP, "Unable to set frame index out of bounds.", Logging.Level.LOG_WARNING);
                SetFrameIndex(0);   //<- resursive call
            }
        }
    }
}
