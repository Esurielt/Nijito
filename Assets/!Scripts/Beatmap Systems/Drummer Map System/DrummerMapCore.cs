using System.Collections.Generic;

namespace Beatmap.Drummer
{
    /// <summary>
    /// Concrete class for the Drummer beatmap type. Use stored instance reference at BeatmapTypes.Drummer.
    /// </summary>
    public class BeatmapType_Drummer : BeatmapType
    {
        public override Beatmap GetNewEmptyBeatmap()
        {
            return new DrummerMap();
        }

        protected override List<Channel> GetNewChannelInstances()
        {
            return new List<Channel>
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
                new DrummerChannel("Outer-left Drum"),
                
                //inner-left drum (usually snares, toms, or hats)
                new DrummerChannel("Inner-left Drum"),

                //center drum (usually kicks)
                new DrummerChannel("Center Drum"),
                
                //inner-right drum (usually snares, toms, or rides)
                new DrummerChannel("Inner-right Drum"),

                //outer-right drum (usually cymbals, rides, or misc.)
                new DrummerChannel("Outer-right Drum"),
            };
        }
    }
    /// <summary>
    /// Concrete class for beatmaps of the Drummer rhythm game type.
    /// </summary>
    public class DrummerMap : Beatmap
    {
        public override BeatmapType TypeInstance => BeatmapTypeInstances.Drummer;
    }
    /// <summary>
    /// Concrete class for channels in the Drummer beatmap type.
    /// </summary>
    public class DrummerChannel : Channel
    {
        //ctor
        public DrummerChannel(string name) : base(name) { }

        protected override List<State> GetNewChannelStateInstances()
        {
            return new List<State>()
            {
                //empty
            };
        }
        protected override List<Value> GetNewChannelValueInstances()
        {
            return new List<Value>()
            {
                ChannelValueInstances.Hit,
                ChannelValueInstances.HoldBegin,
                ChannelValueInstances.HoldEnd,
            };
        }
    }
}
