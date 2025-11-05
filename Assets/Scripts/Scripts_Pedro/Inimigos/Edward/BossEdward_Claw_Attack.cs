using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private readonly List<GameObject> spawnedObjects = new();

    public IEnumerator SpawnClawsCoroutine(BossEdwardController boss)
    {
        if (boss == null || boss.isDead) yield break;

        GameObject[] telegraphs = new GameObject[startPoints.Length];

        for (int i = 0; i < startPoints.Length; i++)
        {
            if (boss.isDead) yield break;
            Transform s = startPoints[i];
            Transform e = (i < endPoints.Length) ? endPoints[i] : null;
            if (s == null || e == null) continue;

            if (telegraphPrefab != null)
            {
                GameObject tele = Instantiate(telegraphPrefab, s.position, Quaternion.identity);
                spawnedObjects.Add(tele);

                Vector2 dir = (e.position - s.position);
                float dist = dir.magnitude;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                tele.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                tele.transform.localScale = new Vector3(dist, tele.transform.localScale.y, tele.transform.localScale.z);

                var sr = tele.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "Default";
                    sr.sortingOrder = 10;
                }

                telegraphs[i] = tele;
            }
        }

        float elapsed = 0f;
        while (elapsed < telegraphDuration)
        {
            if (boss.isDead)
            {
                CleanupAfterDeath();
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < startPoints.Length; i++)
        {
            if (boss.isDead)
            {
                CleanupAfterDeath();
                yield break;
            }

            Transform s = startPoints[i];
            Transform e = (i < endPoints.Length) ? endPoints[i] : null;
            if (s == null || e == null) continue;

            if (clawPrefab != null)
            {
                GameObject claw = Instantiate(clawPrefab, s.position, Quaternion.identity);
                spawnedObjects.Add(claw);

                var clawSr = claw.GetComponent<SpriteRenderer>();
                if (clawSr != null)
                {
                    clawSr.sortingLayerName = "Default";
                    clawSr.sortingOrder = 11;
                }

                StartCoroutine(MoveClawToPoint(claw.transform, e.position, clawSpeed, boss));
            }

            if (telegraphs[i] != null)
            {
                Destroy(telegraphs[i]);
                spawnedObjects.Remove(telegraphs[i]);
            }
        }
    }

    private IEnumerator MoveClawToPoint(Transform t, Vector2 target, float speed, BossEdwardController boss)
    {
        while (t != null && Vector2.Distance(t.position, target) > 0.05f)
        {
            if (boss == null || boss.isDead)
            {
                if (t != null) Destroy(t.gameObject);
                yield break;
            }
            t.position = Vector2.MoveTowards(t.position, target, speed * Time.deltaTime);
            yield return null;
        }

        if (t != null)
            Destroy(t.gameObject);
    }

    public void CleanupAfterDeath()
    {
        if (showDebugLogs) Debug.Log("[ClawAttack] Limpando prefabs após morte do boss.");
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedObjects.Clear();
    }
}
