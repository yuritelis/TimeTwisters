using UnityEngine;
using System.Collections;

public class BossEdward_Wave_Attack : MonoBehaviour
{
    public GameObject waveObject; // Filho do Edward
    public Transform waveSpawnPoint; // Empty filho do boss para spawn da onda
    public float waveSpeed = 6f;
    public float waveLifetime = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        waveObject.SetActive(false);
        rb = waveObject.GetComponent<Rigidbody2D>();
    }

    public IEnumerator DoWave(Transform player)
    {
        waveObject.SetActive(true);

        Vector2 spawnPos = waveSpawnPoint != null ? (Vector2)waveSpawnPoint.position : (Vector2)transform.position;
        waveObject.transform.position = spawnPos;

        Vector2 dir = ((Vector2)player.position - spawnPos).normalized;

        rb.linearVelocity = dir * waveSpeed;

        yield return new WaitForSeconds(waveLifetime);

        rb.linearVelocity = Vector2.zero;
        waveObject.SetActive(false);
    }
}
