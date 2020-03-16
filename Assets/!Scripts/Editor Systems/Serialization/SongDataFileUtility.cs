using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SongData.Serialization
{
    public static class SongDataFileUtility
    {
        /*
         *  [Application persistent data path]
         *      [..other systems' stuff...]
         *      [Song Data]
         *          *.wav (16-bit WAV)
         *          audio.metadata (json)
         *          song.metadata (json)
         *          [Beatmaps]
         *              [Drummer]
         *                  Drummer_Easy.nbm (json)
         *                  Drummer_Medium.nbm (json)
         *              [Vocalist]
         *                  Vocalist_Hard.nbm (json)
         */

        // TODO: Bundle up all the similar actions in this class.
        // Consider making the individual parts of song data more official.

        public const string SONG_DATA_DIRECTORY_NAME = "Song Data";
        public const string AUDIO_FILE_EXTENSION = "wav";
        public const string AUDIO_METADATA_FILE_NAME = "audio.metadata";
        public const string SONG_METADATA_FILE_NAME = "song.metadata";
        public const string BEATMAP_DIRECTORY_NAME = "Beatmaps";
        public const string BEATMAP_FILE_EXTENSION = "nbm";     //nijito beatmap

        public static string AllSongsDirectoryPath => string.Format("{0}/{1}", Application.persistentDataPath, SONG_DATA_DIRECTORY_NAME);
        public static string GetSongDirectoryPath(string songName)
        {
            return string.Format("{0}/{1}", AllSongsDirectoryPath, songName);
        }
        public static string GetBeatmapDirectoryPath(string songName)
        {
            return string.Format("{0}/{1}", GetSongDirectoryPath(songName), BEATMAP_DIRECTORY_NAME);
        }
        public static string GetBeatmapSubDirectoryPath(string songName, BeatmapType typeInstance)
        {
            return string.Format("{0}/{1}", GetBeatmapDirectoryPath(songName), typeInstance.Name);
        }
        public static string GetBeatmapFileName(BeatmapType typeInstance, BeatmapDifficulty difficulty)
        {
            return string.Format("{0}_{1}.{2}", typeInstance.Name, difficulty.Name, BEATMAP_FILE_EXTENSION);
        }
        public static string GetBeatmapFilePath(string songName, BeatmapType typeInstance, BeatmapDifficulty difficulty)
        {
            return string.Format("{0}/{1}", GetBeatmapSubDirectoryPath(songName, typeInstance), GetBeatmapFileName(typeInstance, difficulty));
        }
        public static string GetSongMetadataFilePath(string songName)
        {
            return string.Format("{0}/{1}", GetSongDirectoryPath(songName), SONG_METADATA_FILE_NAME);
        }
        public static string GetAudioMetadataFilePath(string songName)
        {
            return string.Format("{0}/{1}", GetSongDirectoryPath(songName), AUDIO_METADATA_FILE_NAME);
        }

        static SongDataFileUtility()
        {
            if (!SongDataDirectoryExists())
            {
                CreateSongDataDirectory();
            }
        }

        public static List<string> GetAllSongNames()
        {
            var directories = Directory.GetDirectories(AllSongsDirectoryPath);
            return directories.Select(dir => Path.GetFileName(dir)).ToList();
        }
        public static bool SongDataDirectoryExists()
        {
            return Directory.Exists(AllSongsDirectoryPath);
        }
        public static bool SongDirectoryExists(string songName)
        {
            if (string.IsNullOrEmpty(songName))
                return false;

            return Directory.Exists(AllSongsDirectoryPath + "/" + songName);
        }
        public static bool CreateSongDataDirectory()
        {
            try
            {
                var dirPath = AllSongsDirectoryPath;
                Directory.CreateDirectory(dirPath);
                return true;
            }
            catch (System.Exception e)
            {
                Game.LogFormat(Logging.Category.SONG_DATA, "Failed to create main song data directory: {0}", Logging.Level.LOG_ERROR, e);
                return false;
            }
        }
        public static bool CreateNewSong(string songName)
        {
            if (string.IsNullOrEmpty(songName))
                return false;

            try
            {
                Directory.CreateDirectory(GetSongDirectoryPath(songName));
                Directory.CreateDirectory(GetBeatmapDirectoryPath(songName));

                var fs = File.Create(GetAudioMetadataFilePath(songName));
                fs.Close();
                TryWriteAudioMetadata(songName, new AudioMetadata());

                fs = File.Create(GetSongMetadataFilePath(songName));
                fs.Close();
                TryWriteSongMetadata(songName, new SongMetadata());

                return true;
            }
            catch (System.Exception e)
            {
                Game.LogFormat(Logging.Category.SONG_DATA, "Failed to create song directory: {0}", Logging.Level.LOG_ERROR, e);
                return false;
            }
        }
        public static bool BeatmapFileExists(string songName, BeatmapType typeInstance, BeatmapDifficulty difficulty)
        {
            return File.Exists(GetBeatmapFilePath(songName, typeInstance, difficulty));
        }
        public static bool AudioFileExists(string songName)
        {
            var dirPath = GetSongDirectoryPath(songName);
            if (Directory.Exists(dirPath))
            {
                // Can only verify that a file named "*.wav" exists in the song directory. Further testing required on load.

                var filePaths = Directory.GetFiles(dirPath, $"*.{AUDIO_FILE_EXTENSION}");
                return filePaths.Length > 0;
            }
            return false;
        }
        public static bool AudioMetadataFileExists(string songName)
        {
            return File.Exists(GetAudioMetadataFilePath(songName));
        }
        public static bool SongMetadataFileExists(string songName)
        {
            return File.Exists(GetSongMetadataFilePath(songName));
        }

        public static bool TryReadBeatmap(string songName, BeatmapType typeInstance, BeatmapDifficulty difficulty, out Beatmap newBeatmap)
        {
            newBeatmap = null;
            try
            {
                if (Directory.Exists(GetSongDirectoryPath(songName)))
                {
                    string filePath = GetBeatmapFilePath(songName, typeInstance, difficulty);
                    if (File.Exists(filePath))
                    {
                        byte[] bytes = File.ReadAllBytes(filePath);
                        if (TryDeserializeBeatmap(bytes, out SerializableBeatmap beatmapData))
                        {
                            if (beatmapData.TypeName == typeInstance.Name)
                            {
                                newBeatmap = beatmapData.ToBeatmap();
                                if (newBeatmap != null)
                                {
                                    Game.Log(Logging.Category.SONG_DATA, "Read beatmap from file: " + filePath, Logging.Level.LOG);
                                    return true;
                                }
                                throw new IOException("Found beatmap is of unknown type.");
                            }
                            throw new IOException("Found beatmap is of incorrect type.");
                        }
                        throw new IOException("Beatmap data is corrupt.");
                    }
                    throw new FileNotFoundException(filePath);
                }
                throw new DirectoryNotFoundException();
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.SONG_DATA, "Unable to read beatmap from file: " + e, Logging.Level.LOG_WARNING);
            }
            return false;
        }

        public static bool TryWriteBeatmap(string songName, BeatmapDifficulty difficulty, Beatmap beatmap)
        {
            try
            {
                var beatmapData = new SerializableBeatmap(beatmap);
                string directory = GetBeatmapSubDirectoryPath(songName, beatmap.TypeInstance);
                string filePath = GetBeatmapFilePath(songName, beatmap.TypeInstance, difficulty);
                byte[] bytes = SerializeBeatmap(beatmapData);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }                    
                Game.Log(Logging.Category.SONG_DATA, "Wrote beatmap to file: " + directory, Logging.Level.LOG);
                return true;
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.SONG_DATA, "Unable to write beatmap to file: " + e, Logging.Level.LOG_WARNING);
            }
            return false;
        }
        private static bool TryDeserializeBeatmap(byte[] bytes, out SerializableBeatmap beatmapData)
        {
            string jsonStr = System.Text.Encoding.Unicode.GetString(bytes);
            beatmapData = JsonUtility.FromJson<SerializableBeatmap>(jsonStr);
            return beatmapData != null;
        }
        private static byte[] SerializeBeatmap(SerializableBeatmap beatmapData)
        {
            string jsonStr = JsonUtility.ToJson(beatmapData);
            return System.Text.Encoding.Unicode.GetBytes(jsonStr);
        }

        public static bool TryGetAudio(string songName, out AudioClip audio)
        {
            audio = null;
            try
            {
                string dirPath = GetSongDirectoryPath(songName);
                if (Directory.Exists(dirPath))
                {
                    string[] filePaths = Directory.GetFiles(dirPath, $"*.{AUDIO_FILE_EXTENSION}");
                    if (filePaths.Length > 0)
                    {
                        // Only read first found file path.
                        var filePath = filePaths[0];

                        byte[] fileBytes = File.ReadAllBytes(filePath);

                        //file byte count and validity checked by our friend eToile.
                        audio = Etoile.OpenWavParser.ByteArrayToAudioClip(fileBytes, songName + " WAV AUDIO");
                        if (audio != null)
                        {
                            Game.Log(Logging.Category.SONG_DATA, "Read audio from file: " + filePath, Logging.Level.LOG);
                            return true;
                        }
                        throw new IOException("WAV file data is incompatible.");
                    }
                    throw new FileNotFoundException(filePaths[0]);
                }
                throw new DirectoryNotFoundException(dirPath);
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.SONG_DATA, "Unable to read audio from file: " + e, Logging.Level.LOG_WARNING);
                return false;
            }
        }
        public static bool TryImportAudio(string songName, string sourceFilePath)
        {
            try
            {
                if (File.Exists(sourceFilePath))
                {
                    byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
                    if (fileBytes.Length > 0)
                    {
                        if (Etoile.OpenWavParser.IsCompatible(fileBytes))
                        {
                            if (Etoile.OpenWavParser.FormatIsValid(fileBytes))
                            {
                                ImportAudioInternal(songName, sourceFilePath);
                                Game.Log(Logging.Category.SONG_DATA, "Successfully imported audio.");
                                return true;
                            }
                            throw new IOException("WAV format is incompatible. Only 16-bit WAV files can be imported.");
                        }
                        throw new IOException("File data not recognized as a WAV format audio file.");
                    }
                }
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.SONG_DATA, $"Unable to import audio: {e}", Logging.Level.LOG_WARNING);
                // Fall out
            }

            return false;
        }
        private static void ImportAudioInternal(string songName, string sourceFilePath)
        {
            var sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePath);

            var destinationFilePath = string.Format("{0}/{1}.{2}",
                GetSongDirectoryPath(songName),
                sourceFileName,
                AUDIO_FILE_EXTENSION);

            File.Copy(sourceFilePath, destinationFilePath, true);
        }

        public static bool TryGetSongMetadata(string songName, out SongMetadata metadata)
        {
            metadata = null;
            try
            {
                string dirPath = GetSongDirectoryPath(songName);
                if (Directory.Exists(dirPath))
                {
                    string filePath = GetSongMetadataFilePath(songName);
                    if (File.Exists(filePath))
                    {
                        string jsonStr = File.ReadAllText(filePath, System.Text.Encoding.Unicode);
                        var serialized = JsonUtility.FromJson<SerializableSongMetadata>(jsonStr);
                        if (serialized != null)
                        {
                            metadata = serialized.GetAsSongMetadata();
                            Game.Log(Logging.Category.SONG_DATA, "Read song metadata from file: " + filePath, Logging.Level.LOG);
                            return true;
                        }
                        throw new IOException("Song metadata file is corrupt.");
                    }
                    throw new FileNotFoundException(filePath);

                }
                throw new DirectoryNotFoundException(dirPath);
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.SONG_DATA, "Unable to read song metadata from file: " + e, Logging.Level.LOG_WARNING);
                return false;
            }
        }

        public static bool TryWriteSongMetadata(string songName, SongMetadata metadata)
        {
            try
            {
                var serialized = new SerializableSongMetadata(metadata);
                string jsonStr = JsonUtility.ToJson(serialized);
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(jsonStr);

                string dirPath = GetSongDirectoryPath(songName);
                string filePath = GetSongMetadataFilePath(songName);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                Game.Log(Logging.Category.SONG_DATA, "Wrote song metadata to file: " + filePath, Logging.Level.LOG);
                return true;
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.SONG_DATA, "Unable to write song metadata to file: " + e, Logging.Level.LOG_WARNING);
            }
            return false;
        }

        public static bool TryGetAudioMetadata(string songName, out AudioMetadata metadata)
        {
            metadata = null;
            try
            {
                string dirPath = GetSongDirectoryPath(songName);
                if (Directory.Exists(dirPath))
                {
                    string filePath = GetAudioMetadataFilePath(songName);
                    if (File.Exists(filePath))
                    {
                        string jsonStr = File.ReadAllText(filePath, System.Text.Encoding.Unicode);
                        var serialized = JsonUtility.FromJson<SerializableAudioMetadata>(jsonStr);
                        if (serialized != null)
                        {
                            metadata = serialized.GetAsAudioMetadata();
                            Game.Log(Logging.Category.SONG_DATA, "Read audio metadata from file: " + filePath, Logging.Level.LOG);
                            return true;
                        }
                        throw new IOException("Audio metadata file is corrupt.");
                    }
                    throw new FileNotFoundException(filePath);
                    
                }
                throw new DirectoryNotFoundException(dirPath);
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.SONG_DATA, "Unable to read audio metadata from file: " + e, Logging.Level.LOG_WARNING);
                return false;
            }
        }

        public static bool TryWriteAudioMetadata(string songName, AudioMetadata metadata)
        {
            try
            {
                var serialized = new SerializableAudioMetadata(metadata);
                string jsonStr = JsonUtility.ToJson(serialized);
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(jsonStr);

                string dirPath = GetSongDirectoryPath(songName);
                string filePath = GetAudioMetadataFilePath(songName);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                Game.Log(Logging.Category.SONG_DATA, "Wrote audio metadata to file: " + filePath, Logging.Level.LOG);
                return true;
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.SONG_DATA, "Unable to write audio metadata to file: " + e, Logging.Level.LOG_WARNING);
            }
            return false;
        }
    }
}
