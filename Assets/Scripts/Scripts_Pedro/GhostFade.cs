using UnityEngine;

public class GhostFade : MonoBehaviour
{
    public float alpha = 0.4f;
    public Color ghostColor = new Color(0.8f, 0.9f, 1f);
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        ApplyGhostEffect();
    }

    public void ApplyGhostEffect()
    {
        if (sr == null) return;

        Color c = ghostColor;
        c.a = alpha;
        sr.color = c;

        sr.sortingLayerName = sr.sortingLayerName;
        sr.material = new Material(Shader.Find("Sprites/Default"));
    }
}
