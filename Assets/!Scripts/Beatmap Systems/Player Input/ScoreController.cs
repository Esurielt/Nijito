using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatmap.PlayerInput
{
    abstract class ScoreController
    {
        public abstract int[] Compare(int currentIndex,Dictionary<int,int> playerinput,ref int _score, ref int _combo);
    }

    class DrumScoreController : ScoreController
    {
        Beatmap beatmap;
        List<int[]> statemap;
        public class Relu
        {
            public class Size 
            { 
                
            }
            public class Score
            {

            }
        }
        public DrumScoreController(Beatmap map)
        {
            beatmap = map;
        }
        public void updatemap()
        {

        }

        public override int[] Compare(int currentIndex, Dictionary<int, int> playerinput, ref int _score, ref int _combo)
        {
            throw new NotImplementedException();
        }
    }
}
