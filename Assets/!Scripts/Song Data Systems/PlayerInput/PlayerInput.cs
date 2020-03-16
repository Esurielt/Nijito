using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongData.PlayerInput
{
    /// <summary>
    /// Player's Input 
    /// </summary>
    public class PlayerInput
    {
        /// <summary>
        /// Start hit time index
        /// </summary>
        public int StartIndex { get; set; }
        /// <summary>
        /// End hit time index
        /// </summary>
        public int EndIndex { get; set; }
        /// <summary>
        /// ChannelIndex
        /// </summary>
        public int ChannelIndex { get; set; }
        /// <summary>
        /// TODO: get start second, end second and translate to indexes
        /// </summary>
        public PlayerInput(int StartSecond, int EndSecond)
        {
            // 
        }
    }
}
