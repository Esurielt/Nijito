using SongData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Editors.BeatmapEditor
{
    public class FrameValueRotateEvent : UnityEvent<int> { }    //channel index
    public class EditorFrameController : FrameController
    {
        public Image BackgroundImage;

        public Button SelectFrameButton;

        [HideInInspector] public FrameValueRotateEvent OnRequestValueRotate;

        protected RectTransform _rt;
        protected LayoutElement _le;

        protected void Awake()
        {
            OnRequestValueRotate = new FrameValueRotateEvent();

            _rt = GetComponent<RectTransform>();
            _le = GetComponent<LayoutElement>();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            SelectFrameButton.onClick.RemoveAllListeners();
            OnRequestValueRotate.RemoveAllListeners();
        }
        protected override void SpawnValueController_Late(Channel.Value newValue, int channelIndex, ValueController controller)
        {
            var castController = (EditorValueController)controller;
            castController.OnRequestRotate.AddListener(() => OnRequestValueRotate?.Invoke(channelIndex));  // Removed in controller's OnDestroy().
        }
        public void SetVisuals(Color backgroundColor)
        {
            BackgroundImage.color = backgroundColor;
        }
        public void SetWidth(float newWidth)
        {
            _le.preferredWidth = newWidth;
        }
    }
}