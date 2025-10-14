using UnityEngine;
using System.Collections;

public class BossEdward_Claw_Attack : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject clawPrefab;
    public GameObject telegraphPrefab;

    [Header("Pares Start -> End")]
    public Transform[] startPoints;
    public Transform[] endPoints;

    [Header("Config")]
    public float telegraphDuration = 0.8f;
    public float clawSpeed = 8f;
    public bool showDebugLogs = true;

    // Chamado pelo BossController
    public IEnumerator SpawnClawsCoroutine()
    {
        if (startPoints == null || startPoints.Length == 0)
        {
            if (showDebugLogs) Debug.LogWarning("[ClawAttack] startPoints vazio. Nada a spawnar.");
            yield break;
        }

        // 1️⃣ Spawn de todos os telegraphs simultaneamente
        GameObject[] telegraphs = new GameObject[startPoints.Length];
        for (int i = 0; i < startPoints.Length; i++)
        {
            Transform s = startPoints[i];
            Transform e = (i < endPoints.Length) ? endPoints[i] : null;
            if (s == null || e == null) continue;

            if (telegraphPrefab != null)
            {
                GameObject tele = Instantiate(telegraphPrefab, s.position, Quaternion.identity);
                Vector2 dir = (e.position - s.position);
                float dist = dir.magnitude;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                tele.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                tele.transform.localScale = new Vector3(dist, tele.transform.localScale.y, tele.transform.localScale.z);

                var sr = tele.GetComponent<SpriteRenderer>();
                if (sr != null) { sr.sortingLayerName = "Default"; sr.sortingOrder = 10; }

                telegraphs[i] = tele;
            }
        }

        // 2️⃣ Espera duração do telegraph **uma vez só**
        yield return new WaitForSeconds(telegraphDuration);

        // 3️⃣ Spawn de todas as garras simultaneamente
        for (int i = 0; i < startPoints.Length; i++)
        {
            Transform s = startPoints[i];
            Transform e = (i < endPoints.Length) ? endPoints[i] : null;
            if (s == null || e == null) continue;

            if (clawPrefab != null)
            {
                GameObject claw = Instantiate(clawPrefab, s.position, Quaternion.identity);
                var clawSr = claw.GetComponent<SpriteRenderer>();
                if (clawSr != null) { clawSr.sortingLayerName = "Default"; clawSr.sortingOrder = 11; }

                StartCoroutine(MoveClawToPoint(claw.transform, e.position, clawSpeed));
                if (showDebugLogs) Debug.Log($"[ClawAttack] Claw spawned at {s.position} moving to {e.position}");
            }

            // destrói telegraph imediatamente
            if (telegraphs[i] != null) Destroy(telegraphs[i]);
        }
    }

    // 4️⃣ Move a garra até o end e destrói ao chegar
    private IEnumerator MoveClawToPoint(Transform t, Vector2 target, float speed)
    {
        while (t != null && Vector2.Distance(t.position, target) > 0.05f)
        {
            t.position = Vector2.MoveTowards(t.position, target, speed * Time.deltaTime);
            yield return null;
        }

        if (t != null) Destroy(t.gameObject);
    }
}
