using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tween_VN : MonoBehaviour
{

    [SerializeField]
    private Image Battery;
    [SerializeField]
    private Image autoplay_circle;
    [SerializeField]
    private Toggle autoplay;

    private Tween rotator;

    // Start is called before the first frame update
    void Start()
    {
        rotator = autoplay_circle.transform.DORotate(new Vector3(0, 0, 360), 2.5f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart)
            .Pause();
        // Tween the battery
        Battery.DOFillAmount(SystemInfo.batteryLevel, 0.5f).Play();

    }

    // Update is called once per frame
    //void Update()
    //{
    //    // nothing;
    //}

    public void ToggleAutoCircle()
    {
        if (autoplay.isOn)
        {
            rotator.Play();
        }
        else {
            rotator.Pause();
            autoplay_circle.transform.DORotate(new Vector3(0, 0, 0), 0.3f,RotateMode.Fast)
                .SetEase(Ease.InQuart).Play();
        }
    }



}
