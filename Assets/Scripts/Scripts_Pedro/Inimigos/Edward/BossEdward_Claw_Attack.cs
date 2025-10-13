using UnityEngine;
using System.Collections;

public class BossEdward_Claw_Attack : MonoBehaviour
{
    public GameObject leftClaw;  // Filho do Edward
    public GameObject rightClaw; // Filho do Edward
    public Transform leftSpawn;  // Empty child do boss
    public Transform rightSpawn; // Empty child do boss
    public float clawSpeed = 12f;
    public float chargeTime = 1.2f;
    public float lifetime = 3f;

    private Rigidbody2D rbL;
    private Rigidbody2D rbR;

    void Awake()
    {
        leftClaw.SetActive(false);
        rightClaw.SetActive(false);

        rbL = leftClaw.GetComponent<Rigidbody2D>();
        rbR = rightClaw.GetComponent<Rigidbody2D>();
    }

    public IEnumerator DoClaw()
    {
        yield return new WaitForSeconds(chargeTime);

        leftClaw.transform.position = leftSpawn.position;
        rightClaw.transform.position = rightSpawn.position;

        leftClaw.SetActive(true);
        rightClaw.SetActive(true);

        rbL.linearVelocity = Vector2.right * clawSpeed;
        rbR.linearVelocity = Vector2.left * clawSpeed;

        yield return new WaitForSeconds(lifetime);

        rbL.linearVelocity = Vector2.zero;
        rbR.linearVelocity = Vector2.zero;
        leftClaw.SetActive(false);
        rightClaw.SetActive(false);
    }
}
