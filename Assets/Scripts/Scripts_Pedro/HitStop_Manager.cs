using UnityEngine;
using System.Collections;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    [Header("Configuração padrão")]
    public float defaultDuration = 0.08f;

    private bool isHitStopping = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Aplica hitstop global no jogo inteiro por duration segundos.
    /// </summary>
    public void DoGlobalHitStop(float duration = -1f)
    {
        if (duration < 0) duration = defaultDuration;
        if (!isHitStopping)
            StartCoroutine(HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        isHitStopping = true;

        // pausa tudo
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // pausa real em tempo de jogo real
        float realTime = 0f;
        while (realTime < duration)
        {
            realTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // volta ao normal
        Time.timeScale = originalTimeScale;
        isHitStopping = false;
    }
}
