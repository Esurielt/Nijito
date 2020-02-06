using UnityEngine;
using System.IO;
using Beatmap.Interfaces;
using System.Collections.Generic;

namespace Beatmap.IO
{
    public abstract class BeatmapFileIOHelper : IBeatmapIOHelper
    {
        private static readonly string SONG_DATA_DIRECTORY;

        protected string _directoryName = "Untitled Song";
        public string DirectoryName
        {
            get => _directoryName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _directoryName = value;
                }
            }
        }
        public string DirectoryPath => SONG_DATA_DIRECTORY + DirectoryName;
        protected string GetFileName(BeatmapType typeInstance)
        {
            return typeInstance.Name.ToLowerInvariant();
        }
        protected string GetFileName(BeatmapDataComponentType componentType)
        {
            switch (componentType)
            {
                case BeatmapDataComponentType.AUDIO: return "audio";
                default: throw new System.ArgumentException("Unhandled beatmap component type: " + componentType.ToString());
            }
        }
        static BeatmapFileIOHelper()
        {
            SONG_DATA_DIRECTORY = Application.persistentDataPath + "/Song Data/";
        }
        public BeatmapFileIOHelper() { }
        public bool TryReadBeatmapFromFile(BeatmapType typeInstance, out Beatmap newBeatmap)
        {
            newBeatmap = null;
            try
            {
                if (Directory.Exists(DirectoryPath))
                {
                    string filePath = DirectoryPath + "/" + GetFileName(typeInstance);
                    if (File.Exists(filePath))
                    {
                        if (TryRead(filePath, out BeatmapData beatmapData))
                        {
                            return true;
                        }
                        throw new IOException("Beatmap data is corrupt.");
                    }
                    throw new FileNotFoundException(filePath);
                }
                throw new DirectoryNotFoundException(DirectoryPath);
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.BEATMAP, "Unable to read beatmap from file: " + e, Logging.Level.LOG_WARNING);
            }
            return false;
        }

        public bool TryWriteBeatmapToFile(IBeatmap beatmap)
        {
            try
            {
                var beatmapData = new BeatmapData(beatmap);
                string filePath = DirectoryPath + "/" + GetFileName(beatmap.TypeInstance);
                Write(filePath, beatmapData);
                return true;
            }
            catch (System.Exception e)
            {
                Game.Log(Logging.Category.BEATMAP, "Unable to read beatmap from file: " + e, Logging.Level.LOG_WARNING);
            }
            return false;
        }
        protected abstract bool TryRead(string safeFilePath, out BeatmapData beatmapData);
        protected abstract void Write(string safeFilePath, BeatmapData beatmapData);
    }
    public class BeatmapFileIOHelper_JSON : BeatmapFileIOHelper
    {
        public BeatmapFileIOHelper_JSON() : base() { }

        protected override bool TryRead(string safeFilePath, out BeatmapData beatmapData)
        {
            string jsonStr = File.ReadAllText(safeFilePath);
            beatmapData = JsonUtility.FromJson<BeatmapData>(jsonStr);
            return beatmapData != null;
        }
        protected override void Write(string safeFilePath, BeatmapData beatmapData)
        {
            string jsonStr = JsonUtility.ToJson(beatmapData);
            File.WriteAllText(safeFilePath, jsonStr);
        }
    }
}
