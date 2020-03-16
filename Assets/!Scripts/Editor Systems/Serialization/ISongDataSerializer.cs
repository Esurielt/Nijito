using System.Collections.Generic;
using UnityEngine;

namespace SongData.Serialization
{
    public interface ISongDataSerializer
    {
        bool TryReadBeatmap(string songName, BeatmapType typeInstance, BeatmapDifficulty difficulty, out Beatmap newBeatmap);
        bool TryWriteBeatmap(string songName, BeatmapDifficulty difficulty, Beatmap beatmap);
        bool TryGetAudio(string songName, out AudioClip audio);
        bool TryGetSongMetadata(string songName, out SongMetadata metadata);
        bool TryWriteSongMetadata(string songName, SongMetadata metadata);
        bool TryGetAudioMetadata(string songName, out AudioMetadata metadata);
        bool TryWriteAudioMetadata(string songName, AudioMetadata metadata);

        //images
    }
}
