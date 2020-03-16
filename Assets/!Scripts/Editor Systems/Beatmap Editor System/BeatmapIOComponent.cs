using SongData;
using SongData.Serialization;
using MessageBoxes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editors.BeatmapEditor
{
    public class BeatmapIOComponent : IOComponent
    {
        protected override SaveArgs PrepareSave()
        {
            BeatmapEditor beatmapEditor = (BeatmapEditor)WeakEditor;

            string songName = WeakEditor.SongName;
            BeatmapDifficulty difficulty = beatmapEditor.Difficulty;
            BeatmapType beatmapType = beatmapEditor.BeatmapWriter.TypeInstance;

            var saveDataIsValid = !string.IsNullOrEmpty(songName) &&
                difficulty != null &&
                beatmapType != null;

            var dataWillOverwrite = SongDataFileUtility.BeatmapFileExists(songName, beatmapType, difficulty);

            var willOverwriteMessage =
                string.Format(
                    "Song: \"{0}\"\n" +
                    "Difficulty: \"{1}\"\n" +
                    "Beatmap Type: \"{2}\"\n" +
                    "A beatmap already exists for this song and difficulty. Are you sure you want to overwrite it?",
                    songName, difficulty.Name, beatmapType);

            var successMessage =
                string.Format(
                    "Song: \"{0}\"\n" +
                    "Difficulty: \"{1}\"\n" +
                    "Beatmap Type: \"{2}\"\n" +
                    "Beatmap saved successfully.",
                    songName, difficulty.Name, beatmapType);

            void writeToFile() => WriteToFile(songName, difficulty);    //local function

            return new SaveArgs()
            {
                SaveDataIsValid = saveDataIsValid,
                DataWillOverwrite = dataWillOverwrite,
                WillOverwriteMessage = willOverwriteMessage,
                SuccessMessage = successMessage,
                WriteToFileDelegate = writeToFile,
            };
        }
        private void WriteToFile(string songName, BeatmapDifficulty difficulty)
        {
            var beatmapEditor = (BeatmapEditor)WeakEditor;
            
            if (!SongDataFileUtility.TryWriteBeatmap(songName, difficulty, beatmapEditor.BeatmapWriter.Beatmap))
            {
                throw new System.IO.IOException();
            }
        }
    }
}