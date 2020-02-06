using UnityEngine;
using UnityEngine.UI;

namespace Beatmap.Editor
{
    public class EditorDataPointController : DataPointController
    {
        public Image BackgroundImage;
        public TMPro.TMP_Text FrameIndexText;
        public GameObject SelectedIndicator;

        protected RectTransform _rt;

        protected override void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }
        public void UpdateVisuals(int frameIndex, Color backgroundColor)
        {
            string format = "B={0}\nF={1}";
            FrameIndexText.text = string.Format(format, BeatmapUtility.GetBeatIndexString(frameIndex), frameIndex.ToString());

            BackgroundImage.color = backgroundColor;
        }
        public void UpdateSelected(bool isSelected)
        {
            SelectedIndicator.SetActive(isSelected);
        }
        public void UpdateWidth(float newWidth)
        {
            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }
}