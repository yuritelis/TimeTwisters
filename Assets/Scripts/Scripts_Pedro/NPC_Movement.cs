using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Animator animator;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 delta = transform.position - lastPosition;
        float speed = delta.magnitude / Time.deltaTime;

        animator.SetBool("isMoving", speed > 0.01f);

        if (speed > 0.01f)
        {
            Vector2 dir = delta.normalized;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                animator.SetFloat("moveX", Mathf.Sign(dir.x));
                animator.SetFloat("moveY", 0f);
            }
            else
            {
                animator.SetFloat("moveX", 0f);
                animator.SetFloat("moveY", Mathf.Sign(dir.y));
            }
        }

        lastPosition = transform.position;
    }
}
