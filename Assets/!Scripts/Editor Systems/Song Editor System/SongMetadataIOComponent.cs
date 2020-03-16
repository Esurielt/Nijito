using SongData;
using SongData.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KeyCombos;

namespace Editors.SongMetadataEditor
{
    public class SongMetadataIOComponent : IOComponent
    {
        protected override SaveArgs PrepareSave()
        {
            SongMetadataEditor songEditor = (SongMetadataEditor)WeakEditor;

            string songName = songEditor.SongName;
            SongMetadata songMetadata = songEditor.SongMetadataComponent.SongMetadata;

            var saveDataIsValid =
                !string.IsNullOrEmpty(songName) &&
                songMetadata != null;

            var dataWillOverwrite =
                SongDataFileUtility.SongMetadataFileExists(songName);

            var willOverwriteMessage =
                string.Format(
                    "Song: \"{0}\"\n" +
                    "Are you sure you want to update this song metadata?",
                    songName);

            var successMessage =
                string.Format(
                    "Song: \"{0}\"\n" +
                    "Song metadata updated successfully.",
                    songName);

            void writeToFile() => WriteToFile(songName, songMetadata);    //local function, how quaint

            return new SaveArgs()
            {
                SaveDataIsValid = saveDataIsValid,
                DataWillOverwrite = dataWillOverwrite,
                WillOverwriteMessage = willOverwriteMessage,
                SuccessMessage = successMessage,
                WriteToFileDelegate = writeToFile,
            };
        }
        private void WriteToFile(string songName, SongMetadata songMetadata)
        {
            if (!SongDataFileUtility.TryWriteSongMetadata(songName, songMetadata))
            {
                throw new System.IO.IOException();
            }
        }
    }
}