using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    public class ReleasableSlider : Slider
    {
        public UnityEvent onSliderReleased;

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onSliderReleased.Invoke();
        }
    }
}