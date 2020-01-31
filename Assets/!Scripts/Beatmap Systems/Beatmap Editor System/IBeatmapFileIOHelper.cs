using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Beatmap
{
    public interface IBeatmapFileIOHelper
    {
        /// <summary>
        /// Create a runtime beatmap from some file.
        /// </summary>
        /// <typeparam name="TBeatmap">the type of beatmap to make</typeparam>
        /// <returns>a runtime beatmap from file</returns>
        IBeatmap ReadFromFile<TBeatmap>() where TBeatmap : IBeatmap;

        /// <summary>
        /// Write a runtime beatmap to some file.
        /// </summary>
        /// <param name="beatmap">a runtime beatmap</param>
        void WriteToFile(IBeatmap beatmap);
    }
    public class BeatmapFileIOHelper_JSON : IBeatmapFileIOHelper
    {
        public string Directory { get; set; }
        public string FileName { get; set; } = "Untitled";
        public string Extension { get; set; } = "nbm";  //Nijito! Beatmap
        public string FilePath => Directory + FileName + "." + Extension;

        public BeatmapFileIOHelper_JSON()
        {
            Directory = Application.persistentDataPath;
        }
        public BeatmapFileIOHelper_JSON(string fileName)
            :this()
        {
            FileName = fileName;
        }
        public IBeatmap ReadFromFile<TBeatmap>() where TBeatmap : IBeatmap
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);

                    return JsonUtility.FromJson<IBeatmap>(json);
                }
            }
            catch (System.Exception)
            {
                Game.LogFormat(Logging.Category.BEATMAP, "Unable to read beatmap from file \"{0}\".", Logging.Level.LOG_WARNING, FilePath);
            }
            return null;
        }

        public void WriteToFile(IBeatmap beatmap)
        {
            try
            {
                var json = JsonUtility.ToJson(beatmap);
                File.WriteAllText(FilePath, json);
            }
            catch (System.Exception)
            {
                Game.LogFormat(Logging.Category.BEATMAP, "Unable to write beatmap to file \"{0}\".", Logging.Level.LOG_WARNING, FilePath);
            }
        }
    }
}
