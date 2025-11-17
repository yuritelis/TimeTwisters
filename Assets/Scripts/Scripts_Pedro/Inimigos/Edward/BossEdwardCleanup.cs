using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossEdwardCleanup : MonoBehaviour
{
    public static List<GameObject> spawned = new List<GameObject>();

    public static void Register(GameObject obj)
    {
        if (obj != null && !spawned.Contains(obj))
            spawned.Add(obj);
    }

    public void StartCleanup()
    {
        StartCoroutine(CleanupCoroutine());
    }

    private IEnumerator CleanupCoroutine()
    {
        yield return null;

        foreach (var obj in spawned)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawned.Clear();
        Destroy(gameObject, 0.1f);
    }
}
