using UnityEngine;
using System.Collections;

public class BossEdward_Wave_Attack : MonoBehaviour
{
    [Header("Wave")]
    public GameObject wavePrefab;        // prefab da onda
    public float waveSpeed = 6f;
    public float waveLifetime = 3f;
    public Color waveColor = Color.blue; // cor de telegraph

    private SpriteRenderer bossSprite;

    void Awake()
    {
        bossSprite = GetComponent<SpriteRenderer>();
    }

    public IEnumerator DoWave()
    {
        // TELEGRAPH: muda cor do boss
        Color originalColor = bossSprite.color;
        bossSprite.color = waveColor;

        yield return null; // instant�neo, sem delay

        // SPAWN DA WAVE
        if (wavePrefab != null)
        {
            GameObject wave = Instantiate(wavePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = wave.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.right * waveSpeed * transform.localScale.x; // usa dire��o do boss
            }

            // Destr�i ap�s tempo de vida
            Destroy(wave, waveLifetime);
        }

        // Volta cor original
        bossSprite.color = originalColor;
    }
}
