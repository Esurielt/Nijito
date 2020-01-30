using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    public abstract class BeatmapWriter : BeatmapWrapper
    {
        //ctor
        public BeatmapWriter(IBeatmap beatmap) : base(beatmap) { }

        protected DataPoint GetBlankFrame()
        {
            return new DataPoint(new BeatmapFrame(TypeInstance), new List<ReaderModifier>());
            //NOTE (to self): consider cloning a blank frame reference rather than making new ones each time
        }
        protected List<DataPoint> GetBlankBeats(int count)
        {
            //new list
            List<DataPoint> blankBeats = new List<DataPoint>();

            //for each beat
            for (int i = 0; i < count; i++)
            {
                //for each frame in the beat
                for (int j = 0; j < BeatmapUtility.FRAMES_PER_BEAT; j++)
                {
                    //get a new blank frame
                    blankBeats.Add(GetBlankFrame());
                }
            }
            return blankBeats;
        }
        public void InsertBeats(int startingIndex, int beatCount)
        {
            int insertionFrame = BeatmapUtility.ConvertBeatsToFrames(startingIndex);
            var newBeats = GetBlankBeats(beatCount);

            Beatmap.GetDataPoints().InsertRange(insertionFrame, newBeats);
        }
        public void AddBeatsToEnd(int beatCount)
        {
            var newBeats = GetBlankBeats(beatCount);

            Beatmap.GetDataPoints().AddRange(newBeats);
        }
        public void AddBeatsToBeginning(int beatCount)
        {
            InsertBeats(0, beatCount);
        }
        public void ResetFrameToDefault(int frameIndex)
        {
            if (BeatmapUtility.FrameExists(Beatmap, frameIndex))
                Beatmap.GetDataPoints()[frameIndex] = GetBlankFrame();
        }
        public void ResetFrameRangeToDefault(int startingFrameIndex, int frameCount)
        {
            if (BeatmapUtility.FramesExist(Beatmap, startingFrameIndex, frameCount))
            {
                var data = Beatmap.GetDataPoints();
                for (int i = startingFrameIndex; i < startingFrameIndex + frameCount; i++)   //NOTE: iteration starts on starting index and runs for frame count
                {
                    data[i] = GetBlankFrame();
                }
            }
        }
    }
}
