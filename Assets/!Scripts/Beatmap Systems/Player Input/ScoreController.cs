using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Beatmap.PlayerInput.PlayerInputSetting;

namespace Beatmap.PlayerInput
{
    abstract class ScoreController
    {
        public abstract String[] Compare(int currentIndex,Dictionary<int,int> playerinput,ref int _score, ref int _combo);
    }

    class DrumScoreController : ScoreController
    {
        Beatmap beatmap;
        List<int[]> statemap;

        public DrumScoreController(Beatmap map)
        {
            beatmap = map;
        }
        public void updatemap()
        {
            /*
             * [Empty, Empty, Empty, Hit, Empty, Empty, Empty] =>
             * [0    , 1    , 2    , -3 , -2   , -1   , 0    ]
             * 
             * [Empty, Empty, Empty, HoldBegin, Empty, Empty, Empty, Empty, Empty, Empty, HoldEnd, Empty, Empty, Empty] =>
             * [0    , 100  , 200  , 300      , 10000, 10000, 10000, 10000, 10000, 10000, -300   , -200 , -100 , 0    ]
             * 
             */
            int FrameCount = beatmap.Frames.Count;
            int ChannelCount = beatmap.Frames[0].ValueCount;
            int FrameIndex = 0;
            int ChannelIndex = 0;
            for (FrameIndex = 0; FrameIndex < FrameCount; FrameIndex++)
            {
                statemap.Add(new int[ChannelCount]);
            }
            for (FrameIndex = 0; FrameIndex < FrameCount; FrameIndex++)
            {
                statemap.Add(new int[ChannelCount]);
                for (ChannelIndex = 0; ChannelIndex < ChannelCount; ChannelIndex++)
                {
                    if(beatmap.Frames[FrameIndex].GetValue(ChannelIndex).Equals(ChannelValueInstances.Hit))
                    {
                        int start = FrameIndex - Rule.Boo.Size;
                        int end = FrameIndex + Rule.Boo.Size;
                        for (int i = 1; i <= FrameIndex - start; i++)
                        {
                            statemap[i + start][ChannelIndex] = i;
                            statemap[end - i][ChannelIndex] = -i;
                        }
                    }else if (beatmap.Frames[FrameIndex].GetValue(ChannelIndex).Equals(ChannelValueInstances.HoldBegin))
                    {
                        int holdbegin = FrameIndex;
                        int holdend = FrameIndex;
                        for(int i = 0; i < FrameCount; i++)
                        {
                            if (beatmap.Frames[i].GetValue(ChannelIndex).Equals(ChannelValueInstances.HoldEnd))
                                holdend = i;
                        }
                        for(int i=0;i< Rule.Boo.Size; i++)
                        {
                            statemap[holdbegin - i][ChannelIndex] = (Rule.Boo.Size - i)*100;
                            statemap[holdend + i][ChannelIndex] = (-Rule.Boo.Size + i)*100;
                        }
                        statemap[holdend][ChannelIndex] = -Rule.Boo.Size;
                        for(int i = holdbegin + 1; i < holdend; i++)
                        {
                            statemap[i][ChannelIndex] = 10000;
                        }
                    }
                }
            }
        }

        public override String[] Compare(int currentIndex, Dictionary<int, int> playerinput, ref int _score, ref int _combo)
        {
            int ChannelCount = beatmap.Frames[0].ValueCount;
            String[] result = new string[ChannelCount];
            for (int channelIndex = 0; channelIndex < ChannelCount; ++channelIndex)
            {
                if (playerinput.ContainsKey(channelIndex))
                {
                    if (playerinput[channelIndex] == TouchPrasetoInt[TouchPhase.Began])
                    {
                        if (statemap[currentIndex][channelIndex] / 100 != 0)// Hold or Hold Begin or Hold End
                        {
                            _score += Rule.Great.Score;
                            _combo += 1;
                            result[channelIndex] = Rule.Great.Name;
                            // set 0 to all states before hold begin and after hold end 
                            for (int i = currentIndex + 1; i < beatmap.Frames.Count; ++i)
                            {
                                if (statemap[i][channelIndex] / 100 <= 0)
                                {
                                    statemap[i][channelIndex] = 0;
                                    break;
                                }
                                else if (statemap[i][channelIndex] / 10000 == 0 && statemap[i][channelIndex] / 100 != 0)
                                {
                                    statemap[i][channelIndex] = 0;
                                }
                            }
                        }
                        else if (statemap[currentIndex][channelIndex] != 0)// Hit
                        {
                            int distance = Rule.Boo.Size - statemap[currentIndex][channelIndex];
                            if (distance < Rule.Marvelous.Size)
                            {
                                _score += Rule.Marvelous.Score;
                                _combo += 1;
                                result[channelIndex] = Rule.Marvelous.Name;
                            }
                            else if (distance < Rule.Perfect.Size)
                            {
                                _score += Rule.Perfect.Score;
                                _combo += 1;
                                result[channelIndex] = Rule.Perfect.Name;
                            }
                            else if (distance < Rule.Great.Size)
                            {
                                _score += Rule.Great.Score;
                                _combo += 1;
                                result[channelIndex] = Rule.Great.Name;
                            }
                            else if (distance < Rule.Good.Size)
                            {
                                _score += Rule.Good.Score;
                                _combo += 1;
                                result[channelIndex] = Rule.Good.Name;
                            }
                            else if (distance < Rule.Boo.Size)
                            {
                                _score += Rule.Boo.Score;
                                _combo += 1;
                                result[channelIndex] = Rule.Boo.Name;
                            }
                        }
                    }
                    else if (playerinput[channelIndex] == TouchPrasetoInt[TouchPhase.Stationary])
                    {
                        if (statemap[currentIndex][channelIndex] / 10000 > 0)// Hold
                        {
                            _score += Rule.Great.Score;
                            _combo += 1;
                            result[channelIndex] = Rule.Great.Name;
                        }
                    }
                    else if (playerinput[channelIndex] == TouchPrasetoInt[TouchPhase.Ended])
                    {
                        if (statemap[currentIndex][channelIndex] / 10000 > 0)// Hold
                        {
                            _score += Rule.Great.Score;
                            _combo += 1;
                            result[channelIndex] = Rule.Great.Name;
                        }
                        else if (statemap[currentIndex][channelIndex] / 100 < 0)// Hold End
                        {
                            _score += Rule.Great.Score;
                            _combo += 1;
                            result[channelIndex] = Rule.Great.Name;
                        }
                    }
                }
                else if (statemap[currentIndex][channelIndex] == -1 || statemap[currentIndex][channelIndex] == -100)// Miss 
                {
                    _combo = 0;
                    result[channelIndex] = Rule.Miss.Name;
                }
            }
            return result;
        }
    }
}
