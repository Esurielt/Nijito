using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
public class SafeAreaScaler : MonoBehaviour
{
    private static List<SafeAreaScaler> helpers = new List<SafeAreaScaler>();

    public static UnityEvent OnResolutionOrOrientationChanged = new UnityEvent();

    private static bool screenChangeVarsInitialized = false;
    private static ScreenOrientation lastOrientation = ScreenOrientation.Landscape;
    private static Vector2 lastResolution = Vector2.zero;
    private static Rect lastSafeArea = Rect.zero;

    private Canvas canvas;
    private RectTransform rectTransform;
    private RectTransform safeAreaTransform;


    public static Rect ScreenSafeArea()
    {
#if UNITY_EDITOR
        if (Screen.width == 2436 && Screen.height == 1125)
        {
            return new Rect(132, 0, 2172, 1062);
        }
        else return new Rect(0, 0, Screen.width, Screen.height);

#endif
        return Screen.safeArea;
    }

    void Awake()
    {
        if (!helpers.Contains(this))
            helpers.Add(this);

        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        safeAreaTransform = transform.Find("SafeArea") as RectTransform;

        if (!screenChangeVarsInitialized)
        {
            lastOrientation = Screen.orientation;
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;
            lastSafeArea = ScreenSafeArea();

            screenChangeVarsInitialized = true;
        }

        ApplySafeArea();
    }

    void Update()
    {
        if (helpers[0] != this)
            return;

        if (Application.isMobilePlatform && Screen.orientation != lastOrientation)
            OrientationChanged();

        if (ScreenSafeArea() != lastSafeArea)
            SafeAreaChanged();

        if (Screen.width != lastResolution.x || Screen.height != lastResolution.y)
            ResolutionChanged();
    }

    void ApplySafeArea()
    {
        if (safeAreaTransform == null)
            return;

        var safeArea = ScreenSafeArea();

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        safeAreaTransform.anchorMin = anchorMin;
        safeAreaTransform.anchorMax = anchorMax;
    }

    void OnDestroy()
    {
        if (helpers != null && helpers.Contains(this))
            helpers.Remove(this);
    }

    private static void OrientationChanged()
    {
        //Debug.Log("Orientation changed from " + lastOrientation + " to " + Screen.orientation + " at " + Time.time);

        lastOrientation = Screen.orientation;
        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;

        OnResolutionOrOrientationChanged.Invoke();
    }

    private static void ResolutionChanged()
    {
        //Debug.Log("Resolution changed from " + lastResolution + " to (" + Screen.width + ", " + Screen.height + ") at " + Time.time);

        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;

        OnResolutionOrOrientationChanged.Invoke();
    }

    private static void SafeAreaChanged()
    {
        // Debug.Log("Safe Area changed from " + lastSafeArea + " to " + Screen.safeArea.size + " at " + Time.time);

        lastSafeArea = ScreenSafeArea();

        for (int i = 0; i < helpers.Count; i++)
        {
            helpers[i].ApplySafeArea();
        }
    }
}
