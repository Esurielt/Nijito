using TMPro;

namespace Editors.BeatmapEditor
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