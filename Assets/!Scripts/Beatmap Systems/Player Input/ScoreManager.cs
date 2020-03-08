using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatmap.PlayerInput
{
    class ScoreManager
    {
        private int _combo;
        private int _score;

        private ScoreController scoreController;

        public ScoreManager(Beatmap beatmap)
        {
            _combo = 0;
            _score = 0;
            // TODO: choose correct Controller
            scoreController = new DrumScoreController(beatmap);
        }
        public void ChangeBeatmap(Beatmap beatmap)
        {
            // TODO: choose correct Controller
            scoreController = new DrumScoreController(beatmap);
        }
        public int[] Compare(int currentIndex, Dictionary<int,int> playerinput)
        {
            return scoreController.Compare(currentIndex, playerinput,ref _score,ref _combo);
        }
        
    }
}
