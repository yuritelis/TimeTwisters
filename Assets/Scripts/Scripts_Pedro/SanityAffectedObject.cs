using UnityEngine;
using UnityEngine.Tilemaps;

public class SanityAffectedObject : MonoBehaviour
{
    [Header("Configurações de Auto-Aplicação")]
    public bool applyToChildren = true;
    public bool includeInactiveChildren = true;
    public string childTagFilter = "SanityAffected";

    private SanityVisibilitySystem visibilitySystem;
    private Renderer[] targetRenderers;

    void Start()
    {
        FindVisibilitySystem();
        FindTargetRenderers();
        RegisterAllRenderers();
    }

    void FindVisibilitySystem()
    {
        visibilitySystem = Object.FindFirstObjectByType<SanityVisibilitySystem>();
    }

    void FindTargetRenderers()
    {
        if (applyToChildren)
        {
            targetRenderers = GetComponentsInChildren<Renderer>(includeInactiveChildren);

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
            Renderer selfRenderer = GetComponent<Renderer>();
            targetRenderers = selfRenderer != null ? new Renderer[] { selfRenderer } : new Renderer[0];
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
        if (renderer is TilemapRenderer)
        {
            return false;
        }
        if (renderer is ParticleSystemRenderer)
        {
            return false;
        }

        if (renderer.transform == visibilitySystem?.player)
        {
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
        if (visibilitySystem != null)
        {
            RegisterAllRenderers();
        }
    }

    void OnDisable()
    {
        UnregisterAllRenderers();
    }
}