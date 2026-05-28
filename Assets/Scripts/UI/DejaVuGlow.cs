using UnityEngine;
using UnityEngine.UI;

public class DejaVuGlow : MonoBehaviour
{
    private Image targetImage;
    private Color originalColor;
    private float glowSpeed = 2f;
    private bool isGlowing = false;

    void Awake()
    {
        targetImage = GetComponent<Image>();
        if (targetImage != null) originalColor = targetImage.color;
    }

    public void StartGlow()
    {
        if (targetImage == null) return;
        isGlowing = true;
        StartCoroutine(GlowRoutine());
    }

    System.Collections.IEnumerator GlowRoutine()
    {
        Color glowColor = ThemeColors.DejaVuGlow;
        float t = 0f;

        while (isGlowing)
        {
            t += Time.deltaTime * glowSpeed;
            float alpha = (Mathf.Sin(t) + 1f) / 2f;
            targetImage.color = Color.Lerp(originalColor, glowColor, alpha * 0.3f);
            yield return null;
        }

        targetImage.color = originalColor;
    }

    public void StopGlow()
    {
        isGlowing = false;
    }

    void OnDestroy()
    {
        StopGlow();
    }
}