using Beatmap.Serialization;
using MessageBoxes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beatmap.Editor
{
    public class IOComponent : EditorComponent
    {
        public Button SaveButton;
        
        public SongMetadata SongMetadata { get; private set; }

        protected override void InitializeInternal()
        {
            SongMetadata = TryLoadData<SongMetadata>(BeatmapFileUtility.TryGetSongMetadata, "song metadata");
        }
        protected override void SubscribeToEventsInternal()
        {
            SaveButton.onClick.AddListener(Save);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            SaveButton.onClick.RemoveAllListeners();
        }
        public delegate bool DataLoader<T>(string songName, out T foundData);
        public T TryLoadData<T>(DataLoader<T> dataLoader, string logName)
        {
            if (dataLoader(Editor.SongName, out T foundData))
            {
                Game.LogFormat(Logging.Category.BEATMAP, "Loaded {0} successfully.", Logging.Level.LOG, logName);
            }
            else
            {
                Game.LogFormat(Logging.Category.BEATMAP, "Failed to load {0}.", Logging.Level.LOG_WARNING, logName);
            }
            return foundData;
        }
        public void Save()
        {
            string songName = Editor.SongName;
            BeatmapDifficulty difficulty = Editor.Difficulty;
            BeatmapType beatmapType = Editor.BeatmapWriter.TypeInstance;
            AudioMetadata audioMetadata = Editor.TimelineComponent.AudioMetadata;

            void writeToFile() => SaveInternal(songName, difficulty, audioMetadata);    //local function, how quaint

            if (BeatmapFileUtility.BeatmapFileExists(songName, beatmapType, difficulty))
            {
                var msg = MessageTemplate.GetConfirmationBox("Save Beatmap",
                    string.Format(
                    "Song: \"{0}\"\n" +
                    "Difficulty: \"{1}\"\n" +
                    "Beatmap Type: \"{2}\"\n" +
                    "A beatmap already exists for this song and difficulty. Are you sure you want to overwrite it?",
                    songName, difficulty.Name, beatmapType),
                    writeToFile //on yes
                    );

                Game.MessageBox(msg);
            }
            else
            {
                writeToFile();
                Game.MessageBox("Save Beatmap",
                    string.Format(
                    "Song: \"{0}\"\n" +
                    "Difficulty: \"{1}\"\n" +
                    "Beatmap Type: \"{2}\"\n" +
                    "Beatmap saved successfully.",
                    songName, difficulty.Name, beatmapType));
            }
        }
        public void SaveInternal(string songName, BeatmapDifficulty difficulty, AudioMetadata audioMetadata)
        {
            BeatmapFileUtility.TryWriteBeatmap(songName, difficulty, Editor.BeatmapWriter.Beatmap);
            BeatmapFileUtility.TryWriteAudioMetadata(songName, audioMetadata);
            // Song metadata too!
        }
    }
}