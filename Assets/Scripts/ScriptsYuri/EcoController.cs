using System.Collections;
using UnityEngine;

public class EcoController : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private Animator anim;

    private int waypointIndexAtual;
    private bool isWaiting;
    private float lastInputX;
    private float lastInputY;

    void Start()
    {
        anim = GetComponent<Animator>();

        waypoints = new Transform[waypointParent.childCount];

        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
        
        anim.SetBool("isWalking", false);
    }

    void Update()
    {
        if (PauseController.IsGamePaused || isWaiting)
        {
            anim.SetBool("isWalking", false);
            return;
        }

        anim.SetFloat("LastInputX", lastInputX);
        anim.SetFloat("LastInputY", lastInputY);

        MoverWaypoint();
    }

    void MoverWaypoint()
    {
        Transform target = waypoints[waypointIndexAtual];
        Vector2 direction = (target.position - transform.position).normalized;

        anim.SetFloat("InputX", direction.x);
        anim.SetFloat("InputY", direction.y);
        anim.SetBool("isWalking", direction.magnitude > 0f);

        if (direction.magnitude > 0f)
        {
            lastInputX = direction.x;
            lastInputY = direction.y;
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitWaypoint());
        }
    }

    IEnumerator WaitWaypoint()
    {
        isWaiting = true;

        anim.SetBool("isWalking", false);
        anim.SetFloat("LastInputX", lastInputX);
        anim.SetFloat("LastInputY", lastInputY);

        yield return new WaitForSeconds(waitTime);

        waypointIndexAtual = loopWaypoints ? (waypointIndexAtual + 1) % waypoints.Length : Mathf.Min(waypointIndexAtual + 1, waypoints.Length - 1);

        isWaiting = false;
    }
}
