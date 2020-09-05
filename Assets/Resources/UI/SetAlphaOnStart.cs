using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAlphaOnStart : MonoBehaviour
{
    public float targetAlpha = 0;
    // Start is called before the first frame update
    void Start()
    {
        Image targetImage = gameObject.GetComponent<Image>();
        targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, targetAlpha);
    }

}
