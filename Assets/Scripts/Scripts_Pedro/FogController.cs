using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("Referências")]
    public SpriteRenderer fogRenderer;
    public Transform player;

    [Header("Configuração")]
    [Range(0f, 1f)] public float minAlpha = 0.3f;
    [Range(0f, 1f)] public float maxAlpha = 0.9f;
    public float transitionSpeed = 3f;

    private float targetAlpha;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (fogRenderer == null)
            fogRenderer = GetComponentInChildren<SpriteRenderer>();

        targetAlpha = minAlpha;
    }

    void Update()
    {
        if (player == null || fogRenderer == null) return;

        // Centraliza a fog
        Vector3 pos = player.position;
        pos.z = 0f;
        fogRenderer.transform.position = pos;

        // Suaviza transição de opacidade
        Color c = fogRenderer.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * transitionSpeed);
        fogRenderer.color = c;
    }

    public void UpdateFog(float healthPercent)
    {
        targetAlpha = Mathf.Lerp(maxAlpha, minAlpha, healthPercent);
    }
}
