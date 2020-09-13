using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Canvas))]
public class Tween_VN : MonoBehaviour
{
    /// <summary>
    /// References to Scene objects
    /// </summary>
    [SerializeField]
    private Image autoplay_circle;
    [SerializeField]
    private Toggle autoplay;
    [SerializeField]
    private Transform dialoguebox;
    //[SerializeField]
    private GameObject namebar;
    //[SerializeField]
    private GameObject autoskip;
    // tweener storage 
    private Tween rotator;

    // Start is called before the first frame update
    void Start()
    {
        //Setup Dialogue box
        namebar = GameObject.Find("namebar");
        autoskip = GameObject.Find("Auto&Skip");
        if (namebar == null || autoskip == null)
        {
            Debug.LogError("ERROR: Mo namebar or autoskip found on this canvas.");
            return;

        }
        OpenDialogueBox();

        //Setup autoplay circle
        if (autoplay_circle != null)
        {
            Debug.Log("LOG: Generating tweener for autocircle.");
            rotator = autoplay_circle.transform.DORotate(new Vector3(0, 0, 360), 2.5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart)
                .Pause();
        }
        else {
            Debug.LogError("ERROR: Autoplay Circle is NULL.");
        }

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
        else
        {
            rotator.Pause();
            autoplay_circle.transform.DORotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast)
                .SetEase(Ease.InQuart).Play();
        }
    }

    public void OpenDialogueBox() 
    {
        SetDialogueUtilBar(false);
        //check if dialoguebox is valid.
        if (dialoguebox != null && dialoguebox.GetComponent<Image>())
        {
            dialoguebox.localScale = new Vector3(0, 0, 0);
            Tween verticalscale = dialoguebox.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutExpo).Pause().OnComplete(() => SetDialogueUtilBar(true));
            dialoguebox.DOScale(new Vector3(1, 0.1f, 1), 0.5f).SetEase(Ease.InExpo).SetDelay(0.5f).OnComplete(()=>verticalscale.Play());
            
            return;
        }
        Debug.LogError("ERROR: DialogueBox invalid, null or doesn't have Image component.");
    }

    void SetDialogueUtilBar(bool toggle) {
        if (toggle)
        {
            OpenDialogueUtilBar();
        }
        else
        {
            CloseDialogueUtilBar();
        }
    }

    void OpenDialogueUtilBar()
    {
        OpenNameBar();
        OpenAutoSkipBar();
        
    }

    void OpenNameBar() {
        RectTransform maskrect = namebar.transform.Find("namebar mask").GetComponent<RectTransform>();
        Vector2 OriginalSize = maskrect.sizeDelta;
        maskrect.offsetMax = new Vector2(0,0);
        maskrect.offsetMin = new Vector2(-0,-0);
        maskrect.DOSizeDelta(new Vector2(0, OriginalSize.y), 0.3f).SetDelay(0.4f).OnComplete(()=>
            maskrect.DOSizeDelta(new Vector2(OriginalSize.x , OriginalSize.y),0.5f).SetEase(Ease.InOutBack)
            );
        namebar.SetActive(true);
    }

    void OpenAutoSkipBar()
    {

        RectTransform maskrect = autoskip.transform.Find("autoskip mask").GetComponent<RectTransform>();
        RectTransform circlerect = autoplay_circle.gameObject.GetComponent<RectTransform>();
        Vector2 originalSize = maskrect.sizeDelta;
        Vector2 originalPosition = maskrect.anchoredPosition;
        Vector2 originalPosCircle = circlerect.anchoredPosition;
        Vector2 originalsizeCircle = circlerect.sizeDelta;
        //maskrect.anchoredPosition = new Vector2(originalPosition.x, 140);
        maskrect.sizeDelta = new Vector2(0, originalSize.y);
        circlerect.anchoredPosition = new Vector2(originalPosCircle.x, 70);
        circlerect.sizeDelta = new Vector2(18, 18);
        maskrect.DOAnchorPos(originalPosition,0.3f).SetDelay(0.1f)
            .OnComplete(() =>
            maskrect.DOSizeDelta(new Vector2(originalSize.x, originalSize.y), 1f).SetEase(Ease.InOutExpo)
            );
        circlerect.DOSizeDelta(originalsizeCircle, 0.3f).SetEase(Ease.InExpo).OnComplete(() =>
            circlerect.DOAnchorPos(originalPosCircle,0.3f).SetEase(Ease.InOutExpo).SetDelay(0.2f)
            );

        autoskip.SetActive(true);
    }

    void CloseDialogueUtilBar() {

        namebar.SetActive(false);
        autoskip.SetActive(false);
    }

    public void CloseDialogueBox() 
    {
        dialoguebox.gameObject.SetActive(false);
    }


}
