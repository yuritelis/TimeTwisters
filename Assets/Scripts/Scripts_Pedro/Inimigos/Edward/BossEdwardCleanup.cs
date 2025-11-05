using UnityEngine;
using System.Collections;

public class BossEdwardCleanup : MonoBehaviour
{
    public void StartCleanup()
    {
        Debug.Log("<color=yellow>[BossEdwardCleanup]</color> StartCleanup() chamado!");
        StartCoroutine(CleanupRoutine());
    }

    private IEnumerator CleanupRoutine()
    {
        Debug.Log("<color=orange>[BossEdwardCleanup]</color> Iniciando rotina de limpeza...");
        yield return new WaitForSeconds(0.1f);

        var leftovers = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int count = 0;

        foreach (GameObject obj in leftovers)
        {
            if (obj == null) continue;
            string n = obj.name;

            if (n.Contains("ClawPrefab") ||
                n.Contains("Claw_Telegraph_Prefab") ||
                n.Contains("Leap_TelegraphPrefab"))
            {
                Destroy(obj);
                count++;
                Debug.Log($"<color=green>[BossEdwardCleanup]</color> Destruído: {n}");
            }
        }

        Debug.Log($"<color=cyan>[BossEdwardCleanup]</color> Limpeza finalizada. Objetos destruídos: {count}");
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}
