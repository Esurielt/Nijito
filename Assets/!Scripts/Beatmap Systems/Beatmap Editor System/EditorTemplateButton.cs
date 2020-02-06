using UnityEngine.UI;
using Beatmap.Interfaces;
using UnityEditor;

namespace Beatmap.Editor
{
    public class EditorTemplateButton : Button, IValueWrapper
    {
        //Unity fields
        public TMPro.TMP_Text ValueText;
        public Image IconImage;
        public TMPro.TMP_Text ChannelNameText;

        protected Channel.Value _value;
        public Channel.Value GetValue() => _value;

        public void Initialize(Channel channel)
        {
            ChannelNameText.text = channel.Name;
        }
        public void SetValue(Channel.Value value)
        {
            _value = value;
            ValueText.text = value.Name;
            var channelInfo = ChannelValueInstances.GetInfo(value);
            IconImage.sprite = channelInfo.EditorSprite;
            IconImage.enabled = channelInfo.EditorSprite != null;
        }
        public bool Equals(IValueWrapper other)
        {
            return GetValue() == other.GetValue();
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(EditorTemplateButton))]
    public class Editor_EditorTemplateButton : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}