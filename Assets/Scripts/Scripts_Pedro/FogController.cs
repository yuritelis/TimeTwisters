using UnityEngine;

[ExecuteAlways]
public class FogController : MonoBehaviour
{
    public Material fogMat;
    public Transform cameraTransform;
    public Camera targetCamera;

    [Header("Transição")]
    public float transitionSpeed = 4f;
    public float smoothTransitionTime = 1.5f; // ⏳ tempo total da animação de transição
    [Range(0.05f, 1f)] public float followSmoothness = 1f;

    [Header("Densidade e Velocidade")]
    public float maxDensity = 1.8f;
    public float minDensity = 0.9f;
    public float maxSpeed = 0.45f;
    public float minSpeed = 0.1f;

    [Header("Cores")]
    public Color calmBase = new(0.15f, 0f, 0f, 1f);
    public Color calmPulse = new(0.5f, 0f, 0f, 1f);
    public Color insaneBase = new(0.1f, 0f, 0f, 1f);
    public Color insanePulse = new(0.7f, 0f, 0f, 1f);

    [Header("Sistema de Visibilidade")]
    public SanityVisibilitySystem visibilitySystem;

    // Valores atuais e alvo
    private float targetDensity, targetSpeed, targetCrack, targetRim, targetHeartbeatInt, targetHeartbeatSpeed;
    private float currentDensity, currentSpeed, currentCrack, currentRim, currentHeartbeatInt, currentHeartbeatSpeed;
    private Color targetBaseColor, targetPulseColor, currentBaseColor, currentPulseColor;
    private Vector3 smoothedPos;

    // Propriedades públicas para acesso externo (Modificação do passo 4)
    public float CurrentDensity => currentDensity;
    public float CurrentCrackIntensity => currentCrack;
    public float CurrentRimDarkness => currentRim;
    public float CurrentInnerDarkness { get; private set; } = 1f;

    void Start()
    {
        cameraTransform ??= Camera.main?.transform;
        targetCamera ??= Camera.main;

        if (fogMat)
        {
            currentDensity = minDensity;
            currentSpeed = minSpeed;
            currentCrack = 0;
            currentRim = 0.8f;
            currentHeartbeatInt = 0.25f;
            currentHeartbeatSpeed = 1.2f;
            CurrentInnerDarkness = 1f;

            fogMat.SetFloat("_Density", currentDensity);
            fogMat.SetFloat("_Speed", currentSpeed);
            fogMat.SetFloat("_CrackIntensity", currentCrack);
            fogMat.SetFloat("_RimDarkness", currentRim);
            fogMat.SetFloat("_InnerDarkness", CurrentInnerDarkness);
            fogMat.SetFloat("_HeartbeatIntensity", currentHeartbeatInt);
            fogMat.SetFloat("_HeartbeatSpeed", currentHeartbeatSpeed);
        }

        if (cameraTransform != null)
            smoothedPos = cameraTransform.position;
    }

    void LateUpdate()
    {
        if (fogMat == null) return;

        if (cameraTransform != null)
        {
            Vector3 targetPos = cameraTransform.position;
            targetPos.z = 0;
            smoothedPos = Vector3.Lerp(smoothedPos, targetPos, followSmoothness);
            transform.position = smoothedPos;
        }

        if (targetCamera != null)
        {
            float h = targetCamera.orthographicSize * 2f;
            float w = h * targetCamera.aspect;
            transform.localScale = new Vector3(w, h, 1f);
        }

        float t = Time.deltaTime * (1f / smoothTransitionTime);

        currentDensity = Mathf.Lerp(currentDensity, targetDensity, t);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, t);
        currentCrack = Mathf.Lerp(currentCrack, targetCrack, t);
        currentRim = Mathf.Lerp(currentRim, targetRim, t);
        currentHeartbeatInt = Mathf.Lerp(currentHeartbeatInt, targetHeartbeatInt, t);
        currentHeartbeatSpeed = Mathf.Lerp(currentHeartbeatSpeed, targetHeartbeatSpeed, t);
        currentBaseColor = Color.Lerp(currentBaseColor, targetBaseColor, t);
        currentPulseColor = Color.Lerp(currentPulseColor, targetPulseColor, t);

        fogMat.SetFloat("_Density", currentDensity);
        fogMat.SetFloat("_Speed", currentSpeed);
        fogMat.SetFloat("_CrackIntensity", currentCrack);
        fogMat.SetFloat("_RimDarkness", currentRim);
        fogMat.SetFloat("_HeartbeatIntensity", currentHeartbeatInt);
        fogMat.SetFloat("_HeartbeatSpeed", currentHeartbeatSpeed);
        fogMat.SetColor("_BaseColor", currentBaseColor);
        fogMat.SetColor("_PulseColor", currentPulseColor);
    }

    public void UpdateFog(float sanityPercent)
    {
        sanityPercent = Mathf.Clamp01(sanityPercent);

        float fogStart = 0.8f;
        float cracksStart = 0.6f;
        float collapse = 0.4f;
        float finalCollapse = 0.2f;

        float insanityFactor = 0f;
        float crackFactor = 0f;

        if (sanityPercent < fogStart)
        {
            float t = Mathf.InverseLerp(fogStart, 0f, sanityPercent);
            insanityFactor = Mathf.Pow(t, 1.2f);
        }

        if (sanityPercent < cracksStart)
        {
            float t = Mathf.InverseLerp(cracksStart, 0f, sanityPercent);
            crackFactor = Mathf.Pow(t, 2.3f);
        }

        if (sanityPercent <= collapse)
        {
            float t2 = Mathf.InverseLerp(collapse, 0f, sanityPercent);
            insanityFactor = Mathf.Lerp(insanityFactor, 1.4f, Mathf.Pow(t2, 1.1f));
            crackFactor = Mathf.Lerp(crackFactor, 4f, Mathf.Pow(t2, 2f));
        }

        if (sanityPercent <= finalCollapse)
        {
            insanityFactor = 1.6f;
            crackFactor = 5f;
            CurrentInnerDarkness = 1.2f;
            fogMat.SetFloat("_InnerDarkness", CurrentInnerDarkness);
        }
        else
        {
            CurrentInnerDarkness = 1f;
            fogMat.SetFloat("_InnerDarkness", CurrentInnerDarkness);
        }

        // 🧩 define alvos (suavizados com o tempo)
        targetDensity = Mathf.Lerp(minDensity, maxDensity, insanityFactor);
        targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, insanityFactor);
        targetCrack = Mathf.Lerp(0f, 14f, crackFactor);
        targetRim = Mathf.Lerp(1f, 4f, insanityFactor);
        targetHeartbeatInt = Mathf.Lerp(0.2f, 0.8f, insanityFactor);
        targetHeartbeatSpeed = Mathf.Lerp(1.2f, 2.5f, insanityFactor);

        targetBaseColor = Color.Lerp(insaneBase, calmBase, sanityPercent);
        targetPulseColor = Color.Lerp(insanePulse, calmPulse, sanityPercent);

        // 🔄 Sincroniza com o sistema de visibilidade
        if (visibilitySystem != null)
        {
            // Opcional: você pode querer ajustar o raio baseado na sanidade
            // O SanityVisibilitySystem já faz isso automaticamente
        }
    }
}