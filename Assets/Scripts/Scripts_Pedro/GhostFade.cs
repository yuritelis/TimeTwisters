using UnityEngine;

public class GhostFade : MonoBehaviour
{
    [Header("Transparência")]
    [Range(0f, 1f)] public float minAlpha = 0.10f;
    [Range(0f, 1f)] public float maxAlpha = 0.40f;

    [Header("Espectral Azul")]
    public Color spectralTint = new Color(0.3f, 0.6f, 1f, 1f); // azul suave
    public float tintIntensity = 0.35f; // quão forte é o azul
    public float tintPulseSpeed = 1.4f;

    [Header("Fade")]
    public float fadeSpeed = 2f;
    public float pulseSpeed = 1f;

    private SpriteRenderer sr;
    private float targetAlpha;
    private float pulseTimer;
    private float tintTimer;

    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        SetAlpha(minAlpha);
        targetAlpha = minAlpha;
    }

    void Update()
    {
        // --- Pulsar de alpha ---
        pulseTimer += Time.deltaTime * pulseSpeed;
        float pulse = Mathf.Sin(pulseTimer) * 0.5f + 0.5f; // 0 a 1
        targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);

        // Interpolação suave
        float newAlpha = Mathf.Lerp(sr.color.a, targetAlpha, fadeSpeed * Time.deltaTime);

        // --- Pulsar de azul espectral ---
        tintTimer += Time.deltaTime * tintPulseSpeed;
        float tintPulse = Mathf.Sin(tintTimer) * 0.5f + 0.5f; // 0–1
        float tintAmount = tintPulse * tintIntensity;

        // Combina a cor original com o azul spectral
        Color finalColor = Color.Lerp(originalColor, spectralTint, tintAmount);
        finalColor.a = newAlpha;

        sr.color = finalColor;
    }

    private void SetAlpha(float a)
    {
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }
}
