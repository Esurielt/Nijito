using Beatmap.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    /// <summary>
    /// Static class with references to type-instances of each beatmap type/rhythm game
    /// </summary>
    public static class BeatmapTypeInstances
    {
        public static readonly List<BeatmapType> AllInstances = new List<BeatmapType>();
        public static Drummer.BeatmapType_Drummer Drummer { get; private set; } = new Drummer.BeatmapType_Drummer();
        //more to come

        
    }
    public static class ChannelStateInstances
    {
        public static Channel.State Normal = new Channel.State("Normal", frame => frame);   //used by most channels as default
    }
    public static class ChannelValueInstances
    {
        //used by most channels as default empty state
        public static Channel.Value Empty = new Channel.Value("Empty");
        public static Channel.Value Hit = new Channel.Value("Hit");
        public static Channel.Value HoldBegin = new Channel.Value("Begin Hold");
        public static Channel.Value HoldEnd = new Channel.Value("End Hold");

        public static ChannelValueInfo GetInfo(Channel.Value value)
        {
            DatabaseManager.GetDB<ChannelValueInfo>().TryFind(value.Name, out ChannelValueInfo info, false);
            return info;
        }
    }
    public static class BeatmapEditorInstances
    {
        public static BeatmapEditor CurrentEditor = null;     //this belongs somewhere else...
    }
    public static class ReaderModifierInstances
    {
        //tempo change
        //song section
    }
}
