using UnityEngine;
using System.Collections;

public class UIFadeIn : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
