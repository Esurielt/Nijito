using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReleasableSlider : Slider
{
    public SliderEvent onSliderReleased;

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        onSliderReleased.Invoke(value);
    }
}
