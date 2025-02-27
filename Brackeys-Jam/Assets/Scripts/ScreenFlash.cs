using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public Image flashImage;  // Assign the UI Image in Inspector
    public float flashDuration = 0.2f;  // How long the flash lasts
    public float maxAlpha = 0.5f;  // How bright the flash is

    private void Start()
    {
        if (flashImage != null)
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0);
    }

    public void Flash()
    {
        StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        float timer = 0f;

        // Fade in
        while (timer < flashDuration / 2)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, maxAlpha, timer / (flashDuration / 2));
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, alpha);
            yield return null;
        }

        // Fade out
        timer = 0f;
        while (timer < flashDuration / 2)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0, timer / (flashDuration / 2));
            flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, alpha);
            yield return null;
        }

        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, 0);
    }
}
