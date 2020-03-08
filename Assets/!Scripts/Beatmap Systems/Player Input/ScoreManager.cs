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
            if (beatmap.TypeInstance.Equals(BeatmapTypeInstances.Drummer))
            {
                scoreController = new DrumScoreController(beatmap);
            }
            
        }
        public void ChangeBeatmap(Beatmap beatmap)
        {
            if (beatmap.TypeInstance.Equals(BeatmapTypeInstances.Drummer))
            {
                scoreController = new DrumScoreController(beatmap);
            }
        }
        public String[] Compare(int currentIndex, Dictionary<int,int> playerinput)
        {
            return scoreController.Compare(currentIndex, playerinput,ref _score,ref _combo);
        }
        
    }
}
