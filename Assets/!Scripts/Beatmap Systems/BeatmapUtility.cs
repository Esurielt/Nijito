using Beatmap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    /* The subdivisions of a beat and how they line up with frame indexes:
            
    0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23     GUIDE

    0                                                                           1/4 notes
    0                                   12                                      1/8-notes
    0                       8                       16                          1/8-note triplets
    0                 6                 12                18                    1/16-notes
    0           4           8           12          16          20              1/16-note triplets
    0        3        6        9        12       15       18       21           1/32-notes
    0     2     4     6     8     10    12    14    16    18    20    22        1/32-note triplets
    0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23     1/64-note triplets (beatmap framerate) */

    public enum Subdivision     //enum assignments also work as beat divisors
    {
        /// <summary>
        /// One whole beat, or 1/4 note in 4/4 time.
        /// </summary>
        QUARTER = 1,
        /// <summary>
        /// Half of a beat, or 1/8th note in 4/4 time.
        /// </summary>
        EIGHTH = 2,
        /// <summary>
        /// A third of beat, or 1/8th note triplet in 4/4 time.
        /// </summary>
        EIGHTH_TRIPLET = 3,
        /// <summary>
        /// A quarter of a beat, or 1/16 note in 4/4 time.
        /// </summary>
        SIXTEENTH = 4,
        /// <summary>
        /// A sixth of a beat, or 1/16 note triplet in 4/4 time.
        /// </summary>
        SIXTEENTH_TRIPLET = 6,
        /// <summary>
        /// An eighth of a beat, or 1/32 note in 4/4 time.
        /// </summary>
        THIRTYSECOND = 8,
        /// <summary>
        /// A twelveth of a beat, or 1/32 note triplet in 4/4 time.
        /// </summary>
        THIRTYSECOND_TRIPLET = 12,
        /// <summary>
        /// A twenty-forth of a beat (the beatmap frame rate), or 1/64 note triplet in 4/4 time. This is far more precise than is reasonable for a beatmap, but exists to facilitate other subdivisions.
        /// </summary>
        FRAMERATE = 24,
    }
    public static class BeatmapUtility
    {
        public const int FRAMES_PER_BEAT = 24;  //don't change this, the world will explode

        public static int ConvertBeatsToFrames(int beats)
        {
            return beats * FRAMES_PER_BEAT;
        }
        public static int ConvertFramesToBeats(int frames)
        {
            return Mathf.FloorToInt(frames / FRAMES_PER_BEAT);
        }
        public static int CountFrames(IBeatmap beatmap)
        {
            return beatmap.DataPoints.Count;   //herp derp, nice syntactic sugar
        }
        public static int CountBeats(IBeatmap beatmap)
        {
            return ConvertFramesToBeats(CountFrames(beatmap));
        }
        public static bool SubdivisionIncludesIndex(Subdivision subdivision, int frameIndex)
        {
            return frameIndex % (FRAMES_PER_BEAT / (int)subdivision) == 0;
        }
        public static string GetBeatIndexString(int frameIndex)
        {
            string format = "{0}:{1}";
            return string.Format(format, ConvertFramesToBeats(frameIndex), frameIndex % FRAMES_PER_BEAT);
        }
        public static bool FrameExists(IBeatmap beatmap, int frameIndex)
        {
            return frameIndex >= 0 && frameIndex < CountFrames(beatmap);
        }
        public static bool FramesExist(IBeatmap beatmap, int startingFrameIndex, int frameCount)
        {
            return FrameExists(beatmap, startingFrameIndex) && startingFrameIndex + frameCount <= CountFrames(beatmap);
        }
        public static bool BeatsExist(IBeatmap beatmap, int startingBeatIndex, int beatCount)
        {
            return FramesExist(beatmap, ConvertBeatsToFrames(startingBeatIndex), ConvertBeatsToFrames(beatCount));
        }
    }
}
