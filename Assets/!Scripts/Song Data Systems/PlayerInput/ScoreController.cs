using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongData.PlayerInput
{
    /// <summary>
    /// Create when game starts. Save the score and combo
    /// 
    /// When player have new click, create new PlayerInput and notify
    /// When player change beatmap, change this object's _beatmap
    /// 
    /// TODO: detailed grading rules
    /// 1. how difficulty influences 
    /// 2. how button size influences 
    /// 3. how combo influence
    ///     _combo + _currentScore
    /// 4. great, excellent, perfect score
    ///     1, 2, 3
    /// ...
    /// 
    /// detailed comparing rules:
    /// 1. how to judge player is clicking or long holding?
    ///     find the nearest hit or holdbegin (left or right ) in range(greatsize + excellentsize + perfectsize)
    /// 2. how to give score? 
    ///     Hitscore depends on how far the playerinput is from the nearest hit, 
    ///     Holdscore depends on how long. 
    /// 3. how to find the nearest hit or hold begin?
    ///     search in range(greatsize + excellentsize + perfectsize)
    /// 
    /// </summary>
    public class ScoreController
    {
        private Beatmap _beatmap;
        private PlayerInput _playerInput;

        private int _currentScore;
        private int _combo;

        public int CurrentScore => _currentScore;
        public int Combo => _combo;

        /* <Hit> <Begin Hold> <End Hold>
         * 
         *  [Empty, Empty, Empty, Empty, Hit, Empty, Empty, Empty, Empty]
         *  eg => [0,1,1,2,3,2,1,1,0]
         *  
         *  [Empty, Empty, Empty, Empty, Begin Hold, Empty, Empty, Empty ,End Hold, Empty, Empty, Empty, Empty]
         *  eg => [0,0,0,0,3,3,3,3,3,0,0,0,0]
         *  eg: if StartIndex = 3 and EndIndex = 6, score = (6-4)*holdScore, 4 is because holdbegin is here
         *  eg: if StartIndex = 5 and EndIndex = 6, score = (6-4)*holdScore, 4 is because holdbegin is here
         *  eg: if StartIndex = 4 and EndIndex = 10, score = (8-4)*holdScore, 8 is because holdend is here
         *  
         *  rule: two diffenert hits shoulds have at least one interval
         *  TODO: depend on difficulty, hit image size and so on
         *  
         *  greatScore = 1;
         *  excellentScore = 2;
         *  perfectScore = 3;
         *
         *  holdScore = 3
         *  
         *  greatSize = 2;
         *  excellentSize = 1;
         *  perfectSize = 1;
         */
        int missScore = 0;
        int greatScore = 1;
        int excellentScore = 2;
        int perfectScore = 3;

        int holdScore = 1;

        int greatSize = 2;
        int excellentSize = 1;
        int perfectSize = 0;

        public ScoreController(Beatmap beatmap)
        {
            _beatmap = beatmap;
            _playerInput = null;
            _currentScore = 0;
            _combo = 0;
        }

        /// <summary>
        /// when player click or long press the botton
        /// </summary>
        /// <param name="newInput"></param>
        /// <returns></returns>
        public int Notify(PlayerInput newInput)
        {
            try
            {
                _playerInput = newInput;

                int score = Compare();

                // TODO how combo and score calculate
                if (score > 0)
                {

                    _currentScore += score;
                    _combo += 1;
                    _currentScore += _combo;
                }
                else
                {
                    _combo = 0;
                }

                return score;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        /// <summary>
        /// change beatmap when player select change
        /// </summary>
        /// <param name="beatmap"></param>
        /// <returns></returns>
        public bool ChangeBeatmap(Beatmap beatmap)
        {
            _beatmap = beatmap;
            return true;
        }

        private int Compare()
        {
            List<Channel.Value> vList = _beatmap.Frames[_playerInput.ChannelIndex].GetValues();
            Channel.Value startValue = vList[_playerInput.StartIndex];

            if (startValue.Equals(ChannelValueInstances.Hit))
            {
                return perfectScore;
            }
            else if (startValue.Equals(ChannelValueInstances.HoldBegin))
            {
                for (int i = _playerInput.StartIndex + 1; i < vList.Count && i < _playerInput.EndIndex; ++i)
                {
                    if (vList[i].Equals(ChannelValueInstances.HoldEnd))
                    {
                        return holdScore * (i - _playerInput.StartIndex);
                    }
                }
                return holdScore * (_playerInput.EndIndex - _playerInput.StartIndex);
            }
            else if (startValue.Equals(ChannelValueInstances.Empty))
            {
                // search left and right data close to startindex, find <hit> or <hold>
                // search right data(search <Hit> or <Hold Start>)
                for (int i = 1; i < greatSize + excellentSize + perfectSize && i + _playerInput.StartIndex < vList.Count; i++)
                {
                    // judge hit(perfect, excellent, great)
                    if (vList[i + _playerInput.StartIndex].Equals(ChannelValueInstances.Hit))
                    {
                        if (i < perfectSize)
                        {
                            return perfectScore;
                        }
                        else if (i < perfectSize + excellentSize)
                        {
                            return excellentScore;
                        }
                        else if (i < perfectSize + excellentSize + greatSize)
                        {
                            return greatScore;
                        }
                    }
                    else if (vList[i + _playerInput.StartIndex].Equals(ChannelValueInstances.HoldBegin))
                    {
                        int startIndex = i + _playerInput.StartIndex;
                        // sum 
                        for (int k = startIndex + 1; k < vList.Count && k < _playerInput.EndIndex; ++k)
                        {
                            if (vList[k].Equals(ChannelValueInstances.HoldEnd))
                            {
                                return holdScore * (k - startIndex);
                            }
                        }
                        return holdScore * (_playerInput.EndIndex - startIndex);
                    }
                }
                // search left data(search <Hit> or <Hold Start>)
                for (int i = 1; i < greatSize + excellentSize + perfectSize && _playerInput.StartIndex - i >= 0; i++)
                {
                    // judge hit(perfect, excellent, great)
                    if (vList[_playerInput.StartIndex - i].Equals(ChannelValueInstances.Hit))
                    {
                        if (i < perfectSize)
                        {
                            return perfectScore;
                        }
                        else if (i < perfectSize + excellentSize)
                        {
                            return excellentScore;
                        }
                        else if (i < perfectSize + excellentSize + greatSize)
                        {
                            return greatScore;
                        }
                    }
                    else if (vList[_playerInput.StartIndex - i].Equals(ChannelValueInstances.HoldBegin))
                    {
                        int startIndex = _playerInput.StartIndex - i;
                        // sum 
                        for (int k = _playerInput.StartIndex - i + 1; k < vList.Count && k < _playerInput.EndIndex; ++k)
                        {
                            if (vList[k].Equals(ChannelValueInstances.HoldEnd))
                            {
                                return holdScore * (k - startIndex);
                            }
                        }
                        return holdScore * (_playerInput.EndIndex - startIndex);
                    }
                }
            }

            return missScore;
        }


    }
}
