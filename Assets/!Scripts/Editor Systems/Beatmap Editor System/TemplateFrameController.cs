using SongData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editors.BeatmapEditor
{
    public class TemplateFrameController : FrameController
    {
        public void SetChannels(BeatmapType typeInstance)
        {
            for (int i = 0; i < _expectedFrameValueControllers.Count; i++)
            {
                var templateFrameController = (TemplateFrameValueController)_expectedFrameValueControllers[i];
                templateFrameController.SetChannelNameText(typeInstance.ChannelFlyweights[i].Name);

                //rotate value button?
            }
        }
    }
}