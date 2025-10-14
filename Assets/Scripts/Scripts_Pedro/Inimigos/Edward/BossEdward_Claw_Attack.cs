using UnityEngine;
using System.Collections;

public class BossEdward_Claw_Attack : MonoBehaviour
{
    [Header("Garras/Claws")]
    public GameObject clawPrefab;           // prefab da garra/fogo
    public Transform[] clawPositions;       // posições fixas da arena
    public float clawLifetime = 2f;         // tempo que a garra fica ativa
    public float telegraphTime = 0.5f;      // telegraph antes do dano
    public Color telegraphColor = Color.red;

    private SpriteRenderer bossSprite;

    void Awake()
    {
        bossSprite = GetComponent<SpriteRenderer>();
    }

    public void SpawnClaws()
    {
        // Não bloqueia o boss, ataque paralelo
        StartCoroutine(DoClaws());
    }

    private IEnumerator DoClaws()
    {
        // TELEGRAPH: muda cor do boss rapidamente
        Color originalColor = bossSprite.color;
        bossSprite.color = telegraphColor;

        yield return new WaitForSeconds(telegraphTime);

        // SPAWN de todas as garras
        foreach (Transform pos in clawPositions)
        {
            GameObject claw = Instantiate(clawPrefab, pos.position, Quaternion.identity);
            claw.SetActive(true);
            Destroy(claw, clawLifetime);
        }

        // Volta cor do boss
        bossSprite.color = originalColor;
    }
}
