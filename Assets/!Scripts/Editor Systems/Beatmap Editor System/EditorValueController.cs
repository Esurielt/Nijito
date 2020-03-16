using SongData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Editors.BeatmapEditor
{
    public class EditorValueController : ValueController
    {
        public Image IconImage;
        public Button RotateValueButton;

        public UnityEvent OnRequestRotate;

        private void Awake()
        {
            RotateValueButton.onClick.AddListener(() => OnRequestRotate?.Invoke());
        }
        private void OnDestroy()
        {
            RotateValueButton.onClick.RemoveAllListeners();
            OnRequestRotate.RemoveAllListeners();
        }
        public override void SetValue(Channel.Value value)
        {
            base.SetValue(value);
            var channelInfo = ChannelValueInstances.GetInfo(value);
            IconImage.sprite = channelInfo.EditorSprite;
            IconImage.enabled = channelInfo.EditorSprite != null;
        }
    }
}