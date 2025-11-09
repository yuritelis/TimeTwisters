using UnityEngine;
using UnityEngine.Tilemaps;

public class SanityAffectedObject : MonoBehaviour
{
    [Header("Configurações de Auto-Aplicação")]
    public bool applyToChildren = true;
    public bool includeInactiveChildren = true;
    public string childTagFilter = "SanityAffected"; // ⭐ MUDEI: Agora usa "SanityAffected" por padrão

    [Header("Debug")]
    public bool showDebugInfo = false;

    private SanityVisibilitySystem visibilitySystem;
    private Renderer[] targetRenderers;

    void Start()
    {
        FindVisibilitySystem();
        FindTargetRenderers();
        RegisterAllRenderers();

        if (showDebugInfo)
        {
            Debug.Log($"[SanityAffectedObject] '{gameObject.name}' registrou {targetRenderers.Length} renderers");
            foreach (Renderer renderer in targetRenderers)
            {
                Debug.Log($"   - {renderer.gameObject.name} (Tag: {renderer.gameObject.tag})");
            }
        }
    }

    void FindVisibilitySystem()
    {
        visibilitySystem = Object.FindFirstObjectByType<SanityVisibilitySystem>();
        if (visibilitySystem == null)
        {
            Debug.LogWarning("[SanityAffectedObject] Sistema de visibilidade não encontrado!");
        }
    }

    void FindTargetRenderers()
    {
        if (applyToChildren)
        {
            // Busca todos os renderers nos filhos
            targetRenderers = GetComponentsInChildren<Renderer>(includeInactiveChildren);

            // ⭐ CORREÇÃO: Só filtra se a tag NÃO estiver vazia
            if (!string.IsNullOrEmpty(childTagFilter))
            {
                System.Collections.Generic.List<Renderer> filteredRenderers =
                    new System.Collections.Generic.List<Renderer>();

                foreach (Renderer renderer in targetRenderers)
                {
                    if (renderer.CompareTag(childTagFilter))
                    {
                        filteredRenderers.Add(renderer);
                    }
                }

                targetRenderers = filteredRenderers.ToArray();
            }
        }
        else
        {
            // Apenas o renderer deste objeto
            Renderer selfRenderer = GetComponent<Renderer>();
            targetRenderers = selfRenderer != null ? new Renderer[] { selfRenderer } : new Renderer[0];
        }

        // ⭐ NOVO: Se não encontrou nenhum renderer, avisa
        if (targetRenderers.Length == 0)
        {
            Debug.LogWarning($"[SanityAffectedObject] Nenhum renderer encontrado em '{gameObject.name}' com as configurações atuais");
        }
    }

    void RegisterAllRenderers()
    {
        if (visibilitySystem == null || targetRenderers == null) return;

        foreach (Renderer renderer in targetRenderers)
        {
            if (renderer != null && IsValidRenderer(renderer))
            {
                visibilitySystem.RegisterObject(renderer);

                if (showDebugInfo)
                {
                    Debug.Log($"[SanityAffectedObject] Registrou: {renderer.gameObject.name}");
                }
            }
        }
    }

    void UnregisterAllRenderers()
    {
        if (visibilitySystem == null || targetRenderers == null) return;

        foreach (Renderer renderer in targetRenderers)
        {
            if (renderer != null)
            {
                visibilitySystem.UnregisterObject(renderer);
            }
        }
    }

    bool IsValidRenderer(Renderer renderer)
    {
        // Verifica se é um tipo de renderer válido
        if (renderer is TilemapRenderer)
        {
            if (showDebugInfo) Debug.Log($"Ignorado (Tilemap): {renderer.gameObject.name}");
            return false;
        }
        if (renderer is ParticleSystemRenderer)
        {
            if (showDebugInfo) Debug.Log($"Ignorado (Partícula): {renderer.gameObject.name}");
            return false;
        }

        // Verifica se não é o jogador
        if (renderer.transform == visibilitySystem?.player)
        {
            if (showDebugInfo) Debug.Log($"Ignorado (Jogador): {renderer.gameObject.name}");
            return false;
        }

        return true;
    }

    void OnDestroy()
    {
        UnregisterAllRenderers();
    }

    void OnEnable()
    {
        // Re-registra se o objeto for reativado
        if (visibilitySystem != null)
        {
            RegisterAllRenderers();
        }
    }

    void OnDisable()
    {
        // Remove quando o objeto for desativado
        UnregisterAllRenderers();
    }
}