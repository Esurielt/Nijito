using Beatmap.Editor;

namespace Beatmap
{
    /// <summary>
    /// Static class with references to type-instances of each beatmap type/rhythm game
    /// </summary>
    public static class BeatmapTypeInstances
    {
        public static Drummer.BeatmapType_Drummer Drummer { get; private set; } = new Drummer.BeatmapType_Drummer();
        //more to come

        
    }
    public static class ChannelStateInstances
    {
        public static Channel.State Normal = new Channel.State("Normal", frame => frame);   //used by all channels as default
    }
    public static class ChannelValueInstances
    {
        public static Channel.Value Empty = new Channel.Value("Empty");     //used by all channels as default empty state
        public static Channel.Value Hit = new Channel.Value("Hit");
    }
    public static class BeatmapEditorInstances
    {
        public static BeatmapEditor CurrentEditor = null;
    }
}
