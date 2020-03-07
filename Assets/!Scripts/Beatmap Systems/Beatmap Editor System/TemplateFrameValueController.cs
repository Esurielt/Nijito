using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beatmap.Editor
{
    public class TemplateFrameValueController : EditorValueController
    {
        public TMP_Text ChannelNameText;

        public void SetChannelNameText(string text)
        {
            ChannelNameText.text = text;
        }
    }
}