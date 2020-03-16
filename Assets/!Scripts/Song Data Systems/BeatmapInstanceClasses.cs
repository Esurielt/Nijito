using System.Collections.Generic;

namespace SongData
{
    /// <summary>
    /// Static class with references to type-instances of each beatmap type/rhythm game
    /// </summary>
    public static class BeatmapTypeInstances
    {
        public static void PopulateInstances() { }  //ensures static class has been constructed.
        public static BeatmapType Drummer { get; private set; } = new BeatmapType("Drummer",
            new List<Channel>
            {
                /* | Left: Hats   |   Right: Rides |
                 *  
                 *   <OL>                    <OR>  | Outer: Cymbals, misc.
                 * 
                 *       <IL>           <IR>       | Inner: Snares, toms
                 *               <M>
                 *              Kicks
                 */
                
                //outer-left drum (usually cymbals, hats, or misc.)
                new Channel("Outer-left", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                }),
                
                //inner-left drum (usually snares, toms, or hats)
                new Channel("Inner-left", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                }),

                //center drum (usually kicks)
                new Channel("Center", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                }),
                
                //inner-right drum (usually snares, toms, or rides)
                new Channel("Inner-right", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                }),

                //outer-right drum (usually cymbals, rides, or misc.)
                new Channel("Outer-right", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                }),
            });
        public static BeatmapType Guitarist { get; private set; } = new BeatmapType("Guitarist",
            new List<Channel>
            {
                /*
                 * 
                 */
                
                new Channel("0", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("1", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("2", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("3", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("4", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("5", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("6", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("7", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("8", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("9", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("10", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
                new Channel("11", new List<Channel.State>(), new List<Channel.Value>()
                {
                    ChannelValueInstances.Hit,
                    ChannelValueInstances.HoldBegin,
                    ChannelValueInstances.HoldEnd,
                    ChannelValueInstances.Run,
                    ChannelValueInstances.ChordMember,
                }),
            });
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

        public static Channel.Value Run = new Channel.Value("Run");
        public static Channel.Value ChordMember = new Channel.Value("Chord Member");

        public static ChannelValueInfo GetInfo(Channel.Value value)
        {
            DatabaseManager.GetDB<ChannelValueInfo>().TryFind(value.Name, out ChannelValueInfo info, false);
            return info;
        }
    }
}
