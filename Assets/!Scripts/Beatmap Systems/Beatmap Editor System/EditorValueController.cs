using UnityEngine;
using UnityEngine.UI;

namespace Beatmap.Editor
{
    public class EditorValueController : ValueController
    {
        public Image IconImage;
        public override void SetValue(Channel.Value value)
        {
            base.SetValue(value);
            var channelInfo = ChannelValueInstances.GetInfo(value);
            IconImage.sprite = channelInfo.EditorSprite;
            IconImage.enabled = channelInfo.EditorSprite != null;
        }
    }
}