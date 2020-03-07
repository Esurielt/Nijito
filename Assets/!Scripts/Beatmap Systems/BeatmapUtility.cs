﻿using Beatmap.Editor;
using Beatmap.Editor.Timeline;
using Beatmap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
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
            return beatmap.Frames.Count;   //herp derp, nice syntactic sugar
        }
        public static int CountBeats(IBeatmap beatmap)
        {
            return ConvertFramesToBeats(CountFrames(beatmap));
        }
        public static int GetFrameIndexWithinBeat(int frameIndex)
        {
            return frameIndex % FRAMES_PER_BEAT;
        }
        public static string GetBeatIndexString(int frameIndex)
        {
            string format = "{0}:{1}";
            return string.Format(format, ConvertFramesToBeats(frameIndex), GetFrameIndexWithinBeat(frameIndex));
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

        public static class Audio
        {
            public const int WAV_SAMPLES_PER_SECOND = 32768;
            public static float ConvertBpmToBeatDuration(float bpm)
            {
                if (bpm != 0)
                    return 1 / (bpm / 60);
                return 0;
            }
            public static float ConvertBpmToFrameDuration(float bpm)
            {
                return ConvertBpmToBeatDuration(bpm) / FRAMES_PER_BEAT;
            }
        }
    }
}
