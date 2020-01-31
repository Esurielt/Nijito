using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    public abstract class BeatmapWriter : BeatmapWrapper
    {
        //ctor
        public BeatmapWriter(IBeatmap beatmap) : base(beatmap)
        {
            BlankFrameFlyweight = GetBlankFrame();
        }

        public DataPoint BlankFrameFlyweight { get; protected set; }    //for making a quick copy of nothing
        protected DataPoint GetBlankFrame()
        {
            return new DataPoint(new BeatmapFrame(TypeInstance), new List<ReaderModifier>());
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
                    blankBeats.Add(new DataPoint(BlankFrameFlyweight));
                }
            }
            return blankBeats;
        }
        
        //insert blank frames? Do we need it?

        public void InsertBlankBeats(int startingBeatIndex, int beatCount)
        {
            int startingFrameIndex = BeatmapUtility.ConvertBeatsToFrames(startingBeatIndex);
            var newBeats = GetBlankBeats(beatCount);

            Beatmap.GetDataPoints().InsertRange(startingFrameIndex, newBeats);
        }
        public void AddBlankBeatsAtEnd(int beatCount)
        {
            var newBeats = GetBlankBeats(beatCount);

            Beatmap.GetDataPoints().AddRange(newBeats);
        }
        public void RemoveBeats(int startingBeatIndex, int beatCount)
        {
            int startingFrameIndex = BeatmapUtility.ConvertBeatsToFrames(startingBeatIndex);
            int frameCount = BeatmapUtility.ConvertBeatsToFrames(beatCount);

            Beatmap.GetDataPoints().RemoveRange(startingFrameIndex, frameCount);
        }
        public void RemoveBeatsFromEnd(int beatCount)
        {
            int frameCount = BeatmapUtility.ConvertBeatsToFrames(beatCount);
            var data = Beatmap.GetDataPoints();

            data.RemoveRange(data.Count - frameCount, frameCount);
        }
        public void ResetFramesToDefault(int startingFrameIndex, int frameCount)
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
