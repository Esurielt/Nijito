using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SongData.SongDataUtility;
using static SongData.SongDataUtility.Audio;

namespace SongData
{
    public class AudioTimetable
    {
        private readonly List<AudioBeatInfo> _beatInfos = new List<AudioBeatInfo>();
        public AudioTimetable(Beatmap beatmap, AudioMetadata audioMetadata)
        {
            float currentTimestamp = audioMetadata.StartingDelay;

            for (int beat = 0; beat < CountBeats(beatmap); beat++)
            {
                float currentBpm = audioMetadata.BpmChanges.GetCurrentValue(beat);  //accounts for starting values
                float currentDuration = ConvertBpmToBeatDuration(currentBpm);
                _beatInfos.Add(new AudioBeatInfo(currentTimestamp, currentDuration));
                currentTimestamp += currentDuration;
            }
        }
        public AudioBeatInfo GetBeatInfo(int beatIndex)
        {
            return _beatInfos[beatIndex];
        }
        public AudioBeatInfo GetBeatInfoFromTimestamp(float timestamp, out int beatIndex)
        {
            beatIndex = 0;
            AudioBeatInfo examinedBeatInfo;

            // An actual 'do while' loop. My first in probably 13 years.
            // Ensures that the loop is completed at least once ('for' loop can theoretically never execute, according to the compiler).
            do
            {
                examinedBeatInfo = _beatInfos[beatIndex];
                if (examinedBeatInfo.EndTimestamp > timestamp)
                    break;

                // Increment iterator.
                beatIndex++;
            }
            while (beatIndex < _beatInfos.Count);

            return examinedBeatInfo;
        }
        public AudioFrameInfo GetFrameInfo(int frameIndex)
        {
            return GetBeatInfo(ConvertFramesToBeats(frameIndex)).GetFrameInfo(GetFrameIndexWithinBeat(frameIndex));
        }
        public AudioFrameInfo GetFrameInfoFromTimestamp(float timestamp, out int frameIndex)
        {
            var beatInfo = GetBeatInfoFromTimestamp(timestamp, out int beatIndex);
            var frameInfo = GetFrameInfoFromTimestampInternal(timestamp, beatInfo, beatIndex, out int frameIndexWithinBeat);
            frameIndex = ConvertBeatsToFrames(beatIndex) + frameIndexWithinBeat;

            return frameInfo;
        }
        private AudioFrameInfo GetFrameInfoFromTimestampInternal(float timestamp, AudioBeatInfo beatInfo, int beatIndex, out int frameIndexWithinBeat)
        {
            frameIndexWithinBeat = 0;
            AudioFrameInfo examinedFrameInfo;

            // Another 'do while' loop.
            // Ensures that the loop is completed at least once ('for' loop can theoretically never execute, according to the compiler).
            do
            {
                examinedFrameInfo = beatInfo.GetFrameInfo(frameIndexWithinBeat);
                if (examinedFrameInfo.EndTimestamp > timestamp)
                    break;

                // Increment iterator.
                frameIndexWithinBeat++;
            }
            while (frameIndexWithinBeat < FRAMES_PER_BEAT);

            return examinedFrameInfo;
        }
        public IndexesAndProgress GetIndexesAndProgress(float atTimestamp)
        {
            var beatInfo = GetBeatInfoFromTimestamp(atTimestamp, out int beatIndex);
            var beatProgress = Mathf.InverseLerp(beatInfo.StartTimestamp, beatInfo.EndTimestamp, atTimestamp);

            var frameInfo = GetFrameInfoFromTimestampInternal(atTimestamp, beatInfo, beatIndex, out int frameIndexWithinBeat);
            var frameProgress = Mathf.InverseLerp(frameInfo.StartTimestamp, frameInfo.EndTimestamp, atTimestamp);

            int totalFrameIndex = ConvertBeatsToFrames(beatIndex) + frameIndexWithinBeat;

            return new IndexesAndProgress(beatIndex, totalFrameIndex, beatProgress, frameProgress);
        }

        public struct IndexesAndProgress
        {
            public int BeatIndex { get; }
            public int FrameIndex { get; }
            public float BeatProgress { get; }
            public float FrameProgress { get; }
            public float PreciseBeatIndex { get; }
            public float PreciseFrameIndex { get; }
            public IndexesAndProgress(int beatIndex, int frameIndex, float beatProgress, float frameProgress)
            {
                BeatIndex = beatIndex;
                FrameIndex = frameIndex;
                BeatProgress = beatProgress;
                FrameProgress = frameProgress;
                PreciseBeatIndex = beatIndex + beatProgress;
                PreciseFrameIndex = frameIndex + frameProgress;
            }
        }
    }
    public struct AudioBeatInfo
    {
        public float StartTimestamp { get; }
        public float EndTimestamp { get; }
        public float Duration { get; }
        private readonly AudioFrameInfo[] _frameInfos;
        public AudioBeatInfo(float startTimestamp, float duration)
        {
            StartTimestamp = startTimestamp;
            Duration = duration;
            EndTimestamp = startTimestamp + duration;

            _frameInfos = new AudioFrameInfo[FRAMES_PER_BEAT];

            var frameDuration = Duration / FRAMES_PER_BEAT;
            for (int i = 0; i < FRAMES_PER_BEAT; i++)
            {
                _frameInfos[i] = new AudioFrameInfo(StartTimestamp + (frameDuration * i), frameDuration);
            }
        }
        public AudioFrameInfo GetFrameInfo(int indexWithinBeat)
        {
            return _frameInfos[indexWithinBeat];
        }
    }
    public struct AudioFrameInfo
    {
        public float StartTimestamp { get; }
        public float EndTimestamp { get; }
        public float Duration { get; }

        public AudioFrameInfo(float startTimestamp, float duration)
        {
            StartTimestamp = startTimestamp;
            Duration = duration;
            EndTimestamp = startTimestamp + duration;
        }
    }
}