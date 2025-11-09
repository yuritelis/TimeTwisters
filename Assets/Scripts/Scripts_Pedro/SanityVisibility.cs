using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class SanityVisibilitySystem : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public PlayerHealth playerHealth;
    public FogController fogController;
    public Camera mainCamera;

    [Header("Configurações")]
    public float checkInterval = 0.1f;
    public float visibilityThreshold = 0.1f;
    [Header("Fade Settings")]
    public float fadeDuration = 0.3f;

    [Header("Modo de Registro")]
    public bool autoFindObjects = false; // ⭐ NOVO: Controla busca automática

    private List<Renderer> affectedRenderers = new List<Renderer>();
    private Dictionary<Renderer, Coroutine> fadeCoroutines = new Dictionary<Renderer, Coroutine>();
    private float checkTimer;

    // Layers que NÃO serão afetadas pela sanidade
    public LayerMask ignoredLayers = 1;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // ⭐ MODIFICADO: Só busca automaticamente se habilitado
        if (autoFindObjects)
        {
            FindAllAffectedObjects();
        }
    }

    void FindAllAffectedObjects()
    {
        Renderer[] allRenderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);

        foreach (Renderer renderer in allRenderers)
        {
            RegisterObject(renderer);
        }

        Debug.Log($"[SanityVisibility] {affectedRenderers.Count} objetos afetados pela sanidade (busca automática)");
    }

    void Update()
    {
        if (playerHealth == null || fogController == null || mainCamera == null) return;

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            UpdateAllObjectsVisibility();
        }
    }

    void UpdateAllObjectsVisibility()
    {
        foreach (Renderer renderer in affectedRenderers)
        {
            if (renderer != null && renderer.isVisible)
            {
                UpdateObjectVisibility(renderer);
            }
        }
    }

    void UpdateObjectVisibility(Renderer renderer)
    {
        Vector3 screenPos = mainCamera.WorldToViewportPoint(renderer.bounds.center);
        Vector2 fogUV = new Vector2(screenPos.x, screenPos.y);
        float sanityPercent = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        float visibleRadius = Mathf.Lerp(0.2f, 0.8f, sanityPercent);
        Vector2 center = new Vector2(0.5f, 0.5f);

        float distanceFromCenter = Vector2.Distance(fogUV, center);
        bool isVisible = distanceFromCenter <= visibleRadius;

        // ⭐ DEBUG TEMPORÁRIO para objetos do cenário
        if (renderer.gameObject.name.Contains("Saguão") ||
            renderer.gameObject.name.Contains("Tapete") ||
            renderer.gameObject.name.Contains("relogio") ||
            renderer.gameObject.name.Contains("Armário") ||
            renderer.gameObject.name.Contains("porta"))
        {
            Debug.Log($"[DEBUG] {renderer.gameObject.name} - Sanidade: {sanityPercent:F2}, " +
                      $"Raio: {visibleRadius:F2}, Dist: {distanceFromCenter:F2}, Visível: {isVisible}, " +
                      $"ScreenPos: {screenPos}, Alpha: {renderer.material.color.a:F2}");
        }

        SetRendererVisibility(renderer, isVisible);
    }

    void SetRendererVisibility(Renderer renderer, bool visible)
    {
        if (renderer != null)
        {
            float targetAlpha = visible ? 1f : 0f;

            // ⭐ CORREÇÃO: Cria cópias dos materiais para não afetar outros objetos
            if (renderer.materials.Length > 0 && renderer.materials[0] != null)
            {
                // Verifica se o material é instanciado
                if (!renderer.materials[0].name.Contains("Instance"))
                {
                    // Se não é uma instância, cria uma
                    renderer.material = new Material(renderer.material);
                }
            }

            if (fadeCoroutines.ContainsKey(renderer) && fadeCoroutines[renderer] != null)
            {
                StopCoroutine(fadeCoroutines[renderer]);
            }

            fadeCoroutines[renderer] = StartCoroutine(FadeAlphaCoroutine(renderer, targetAlpha));
        }
    }

    IEnumerator FadeAlphaCoroutine(Renderer renderer, float targetAlpha)
    {
        if (renderer == null) yield break;

        Material[] materials = renderer.materials;
        float[] startAlphas = new float[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null)
            {
                startAlphas[i] = materials[i].color.a;
            }
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration && renderer != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            float currentT = t * t * (3f - 2f * t);

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    float currentAlpha = Mathf.Lerp(startAlphas[i], targetAlpha, currentT);
                    Color color = materials[i].color;
                    color.a = currentAlpha;
                    materials[i].color = color;
                }
            }

            yield return null;
        }

        if (renderer != null)
        {
            foreach (Material mat in materials)
            {
                if (mat != null)
                {
                    Color color = mat.color;
                    color.a = targetAlpha;
                    mat.color = color;
                }
            }
        }

        if (fadeCoroutines.ContainsKey(renderer))
        {
            fadeCoroutines.Remove(renderer);
        }
    }

    // ⭐ MÉTODO PÚBLICO: Para registro manual (usado pelo SanityAffectedObject)
    public void RegisterObject(Renderer objectRenderer)
    {
        if (objectRenderer == null || affectedRenderers.Contains(objectRenderer))
            return;

        // Pula se estiver em layer ignorada
        if (((1 << objectRenderer.gameObject.layer) & ignoredLayers) != 0)
            return;

        // Pula o próprio jogador
        if (objectRenderer.transform == player)
            return;

        // Pula tilemap renderers
        if (objectRenderer is TilemapRenderer)
            return;

        affectedRenderers.Add(objectRenderer);

        Debug.Log($"[SanityVisibility] Registrou: {objectRenderer.gameObject.name}");
    }

    // Método para remover objetos
    public void UnregisterObject(Renderer objectRenderer)
    {
        if (objectRenderer != null && affectedRenderers.Contains(objectRenderer))
        {
            if (fadeCoroutines.ContainsKey(objectRenderer) && fadeCoroutines[objectRenderer] != null)
            {
                StopCoroutine(fadeCoroutines[objectRenderer]);
                fadeCoroutines.Remove(objectRenderer);
            }

            affectedRenderers.Remove(objectRenderer);
            SetRendererVisibilityImmediate(objectRenderer, true);

            Debug.Log($"[SanityVisibility] Removeu: {objectRenderer.gameObject.name}");
        }
    }

    void SetRendererVisibilityImmediate(Renderer renderer, bool visible)
    {
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat != null)
                {
                    Color color = mat.color;
                    color.a = visible ? 1f : 0f;
                    mat.color = color;
                }
            }
        }
    }

    void OnDestroy()
    {
        foreach (var coroutine in fadeCoroutines.Values)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        foreach (Renderer renderer in affectedRenderers)
        {
            SetRendererVisibilityImmediate(renderer, true);
        }
    }
}