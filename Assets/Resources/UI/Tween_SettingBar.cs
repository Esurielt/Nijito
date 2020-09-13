using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tween_SettingBar : MonoBehaviour
{

    [SerializeField]
    private Image battery;
    [SerializeField]
    private Image setting;

    private Tween gear_rotator;

    // Start is called before the first frame update
    void Start()
    {
        //fetch the components
        battery = gameObject.transform.Find("Battery").GetComponent<Image>();
        setting = gameObject.transform.Find("SettingGear").GetComponent<Image>();
        //Tween the battery fill
        if (battery != null)
        {
            Debug.Log("LOG: Generating tweener for battery.");
            battery.DOFillAmount(SystemInfo.batteryLevel, 0.5f).Play();
        }
        else
        {
            Debug.LogError("ERROR: Autoplay Circle is NULL.");
        }

        // setup the setting gear tween
        gear_rotator = setting.transform.DORotate(new Vector3(0, 0, 180), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutBack).SetAutoKill(false)
                .Pause();

    }

    // Update is called once per frame
    public void rotateSettingGear() {
        gear_rotator.Restart();
    }
}
