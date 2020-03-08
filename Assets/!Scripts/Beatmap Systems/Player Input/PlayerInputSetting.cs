using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Beatmap.PlayerInput
{
    class PlayerInputSetting
    {
        public static Dictionary<TouchPhase, int> TouchPrasetoInt = new Dictionary<TouchPhase, int>
                {
                    { TouchPhase.Began, 0 } ,
                    { TouchPhase.Canceled, 1 },
                    { TouchPhase.Ended, 2 },
                    { TouchPhase.Moved, 3 },
                    { TouchPhase.Stationary, 4 }
                };

        public class Rule
        {
            public class Detail
            {
                public int Size;
                public int Score;
                public String Name;
                public Detail(int size, int score, String name)
                {
                    Size = size;
                    Score = score;
                    Name = name;
                }
            }
            /*
             * Marvelous: +/- 16.7 ms (+/- One Frame)
             *
             * Perfect: +/- 33 ms (+/- Two Frames)
             *
             * Great: +/- 92 ms (+/- 5.5 Frames)
             *
             * Good: +/- 142 ms (+/- 8.5 Frames)
             *
             * Boo: +/- 225 ms (+/- 13.5 Frames)
             */
            public static Detail Marvelous = new Detail(1, 5, "Marvelous");
            public static Detail Perfect = new Detail(2, 4, "Perfect");
            public static Detail Great = new Detail(6, 3, "Great");
            public static Detail Good = new Detail(9, 2, "Good");
            public static Detail Boo = new Detail(14, 5, "Boo");
            public static Detail Miss = new Detail(0, 0, "Miss");

        }
    }

}
