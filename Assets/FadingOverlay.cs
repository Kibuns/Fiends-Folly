using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class FadingOverlay : MonoBehaviour
{
    private Image image;
    public float fadeSpeed = 10f;
    void Start()
    {
        image = GetComponent<Image>();
        
        Color temp = image.color;
        temp.a = 1;
        image.color = temp;
        FadeIn();
    }


    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutSequence());
    }

    private IEnumerator FadeOutSequence()
    {
        float targetAlpha = 1;
        Color tempColor = image.color;

        while(image.color.a < 0.99)
        {
            tempColor.a = Mathf.Lerp(tempColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            image.color = tempColor;
            yield return null;
        }
        tempColor.a = 1f;
        image.color = tempColor;
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInSequence());
    }

    private IEnumerator FadeInSequence()
    {
        float targetAlpha = 0;
        Color tempColor = image.color;

        while (image.color.a > 0.01)
        {
            tempColor.a = Mathf.Lerp(tempColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            image.color = tempColor;
            yield return null;
        }
        tempColor.a = 0f;
        image.color = tempColor;
    }
}
