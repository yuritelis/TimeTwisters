using System.Collections;
using UnityEngine;

public class WaypointMove : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int waypointIndexAtual;
    private bool isWaiting;

    void Start()
    {
        waypoints = new Transform[waypointParent.childCount];

        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (PauseController.IsGamePaused || isWaiting)
        {
            return;
        }

        MoverWaypoint();
    }

    void MoverWaypoint()
    {
        Transform target = waypoints[waypointIndexAtual];

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitWaypoint());
        }
    }

    IEnumerator WaitWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        waypointIndexAtual = loopWaypoints ? (waypointIndexAtual + 1) % waypoints.Length : Mathf.Min(waypointIndexAtual + 1, waypoints.Length - 1);

        isWaiting = false;
    }
}
