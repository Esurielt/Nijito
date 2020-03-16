using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SongData.Serialization
{
    [System.Serializable]
    public class SerializableBeatmap        //the crucial data, which is written to disk in whatever form
    {
        public string TypeName;
        public SerializableFrame[] Frames;
        public SerializableBeatmap(Beatmap beatmap)
        {
            TypeName = beatmap.TypeInstance.Name;

            Frames = beatmap.Frames.Select(frame => new SerializableFrame(frame)).ToArray();
        }
        public Beatmap ToBeatmap()
        {
            var typeInstance = BeatmapType.Get(TypeName);
            if (typeInstance == null)
            {
                return null;
            }

            var beatmap = new Beatmap(typeInstance);
            beatmap.Frames.AddRange(Frames.Select(sFrame => sFrame.ToFrame(typeInstance)));

            return beatmap;
        }
    }
    [System.Serializable]
    public class SerializableFrame
    {
        public string[] ValueNames;
        public SerializableFrame(Frame frame)
        {
            ValueNames = frame.GetValues().Select(v => v.Name).ToArray();
        }
        public Frame ToFrame(BeatmapType typeInstance)
        {
            if (typeInstance.ChannelFlyweights.Count != ValueNames.Length)
            {
                Game.Log(Logging.Category.SONG_DATA, "Serializable frame value count and requested type instance channel count mismatch.", Logging.Level.LOG_WARNING);
                return null;
            }

            var values = new List<Channel.Value>();
            for (int i = 0; i < typeInstance.ChannelFlyweights.Count; i++)
            {
                var channel = typeInstance.ChannelFlyweights[i];    //associated channel

                //try to find a valid value with that name (case insensitive), otherwise return the default for the channel
                var value = channel.ValueFlyweights.Find(v => v.Name.ToUpperInvariant() == ValueNames[i].ToUpperInvariant()) ?? channel.DefaultValueFlyweight;
                
                values.Add(value);    //add it to the list
            }
            return new Frame(values);
        }
    }
}