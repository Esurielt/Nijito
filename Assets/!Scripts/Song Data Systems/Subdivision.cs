using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongData
{
    public class Subdivision
    {
        /* The subdivisions of a beat and how they line up with frame indexes:
            
        0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23     GUIDE

        0                                                                           1/4 notes
        0                                   12                                      1/8-notes
        0                       8                       16                          1/8-note triplets
        0                 6                 12                18                    1/16-notes
        0           4           8           12          16          20              1/16-note triplets
        0        3        6        9        12       15       18       21           1/32-notes
        0     2     4     6     8     10    12    14    16    18    20    22        1/32-note triplets
        0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23     1/64-note triplets (beatmap framerate) */
        
        private static readonly List<Subdivision> _allSubdivisions = new List<Subdivision>();
        public static List<Subdivision> GetAll() => new List<Subdivision>(_allSubdivisions);
        public static Subdivision GetByIndex(int index) => _allSubdivisions[index];

        //static functions


        //static instances
        public static Subdivision Beats = new Subdivision("Beats", "1/4 notes", 1, new Color(.871f, .443f, .427f));                         //red
        public static Subdivision HalfBeats = new Subdivision("1/2 Beats", "1/8 notes", 2, new Color(.443f, .427f, .871f));                 //blue
        public static Subdivision ThirdBeats = new Subdivision("1/3 Beats", "1/8 note triplets", 3, new Color(.427f, .971f, .443f));        //green
        public static Subdivision QuarterBeats = new Subdivision("1/4 Beats", "1/16 notes", 4, new Color(.851f, .871f, .427f));             //yellow
        public static Subdivision SixthBeats = new Subdivision("1/6 Beats", "1/16 note triplets", 6, new Color(.871f, .427f, .851f));       //magenta
        public static Subdivision EighthBeats = new Subdivision("1/8 Beats", "1/32 notes", 8, new Color(.871f, .667f, .427f));              //orange
        public static Subdivision TwelfthBeats = new Subdivision("1/12 Beats", "1/32 note triplets", 12, new Color(.624f, .624f, .624f));   //gray
        public static Subdivision Framerate = new Subdivision("Framerate", "1/64 note triplets", 24, new Color(1f, 1f, 1f));                //white

        //instance members
        public string Name { get; }
        public string Description { get; }
        public int QuantityPerBeat { get; }
        public Color Color { get; }
        private Subdivision(string name, string description, int quantityPerBeat, Color color)
        {
            Name = name;
            Description = description;
            QuantityPerBeat = quantityPerBeat;
            Color = color;

            _allSubdivisions.Add(this);
        }
        public bool IncludesIndex(int frameIndex)
        {
            return frameIndex % (SongDataUtility.FRAMES_PER_BEAT / QuantityPerBeat) == 0;
        }
        public int GetClosestVisibleFrame(int frameIndex)
        {
            // These are ints, so they will appropriately truncate after division.
            return frameIndex - (frameIndex % QuantityPerBeat);
            // Quantiy: 8
            // frameIndex: 781
            // 781 % 8 = 5 (97_5/8)
            // 781 - 5 = 776 (776 / 8 = 97)
        }
        public static Subdivision GetHighestSubdivisionOfFrame(int frameIndex)
        {
            //this is done a lot and switch lookup table is easier on cpu
            switch (frameIndex % SongDataUtility.FRAMES_PER_BEAT)
            {
                case 1:
                case 5:
                case 7:
                case 11:
                case 13:
                case 17:
                case 19:
                case 23:
                    return Framerate;
                case 2:
                case 10:
                case 14:
                case 22:
                    return TwelfthBeats;
                case 3:
                case 9:
                case 15:
                case 21:
                    return EighthBeats;
                case 4:
                case 20:
                    return SixthBeats;
                case 6:
                case 18:
                    return QuarterBeats;
                case 8:
                case 16:
                    return ThirdBeats;
                case 12:
                    return HalfBeats;
                case 0: default:
                    return Beats;
            }
        }
    }
}