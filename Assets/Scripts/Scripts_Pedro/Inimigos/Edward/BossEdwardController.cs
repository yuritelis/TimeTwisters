using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossEdwardController : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Configurações Gerais")]
    public float attackInterval = 5f;
    public float attackRange = 6f;

    [Header("Ataques")]
    public BossEdward_Leap_Attack leapAttack;
    public BossEdward_Claw_Attack clawAttack;

    [Header("Ataque Normal")]
    public EnemyCombat normalAttack;
    public float normalAttackRange = 2f;
    public float normalAttackCooldown = 1f;

    private bool isAttackingSpecial = false;
    private SpriteRenderer sr;
    private EdwardMovement movement;
    private bool canDoNormalAttack = true;

    public bool isDead = false;

    public Enemy_Health bossHp;
    private int lastHp;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        movement = GetComponent<EdwardMovement>();

        bossHp = GetComponent<Enemy_Health>();
        if (bossHp != null)
            lastHp = bossHp.currentHealth;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (movement != null)
            movement.player = player;
    }

    void Start()
    {
        StartCoroutine(AttackCycle());
        StartCoroutine(NormalAttackLoop());
    }

    void Update()
    {
        if (isDead) return;
        if (bossHp == null) return;

        lastHp = bossHp.currentHealth;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();

        leapAttack?.CleanupAfterDeath();
        clawAttack?.CleanupAfterDeath();

        GameObject cleaner = new GameObject("EdwardCleanupExecutor");
        DontDestroyOnLoad(cleaner);
        cleaner.AddComponent<BossEdwardCleanup>().StartCleanup();
    }

    IEnumerator NormalAttackLoop()
    {
        while (!isDead)
        {
            if (!isAttackingSpecial && player != null && canDoNormalAttack)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= normalAttackRange && movement != null)
                {
                    movement.ChangeState(EdwardState.Attacking);
                    movement.BossAttack();

                    canDoNormalAttack = false;
                    yield return new WaitForSeconds(normalAttackCooldown);
                    canDoNormalAttack = true;
                }
            }
            yield return null;
        }
    }

    IEnumerator AttackCycle()
    {
        yield return new WaitForSeconds(2f);

        while (!isDead)
        {
            if (!isAttackingSpecial && player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= attackRange)
                {
                    yield return StartCoroutine(ChooseRandomAttack());
                    yield return new WaitForSeconds(attackInterval);
                }
            }
            yield return null;
        }
    }

    IEnumerator ChooseRandomAttack()
    {
        isAttackingSpecial = true;
        if (movement != null) movement.canMove = false;

        int attackIndex = Random.Range(0, 2);

        switch (attackIndex)
        {
            case 0:
                if (leapAttack != null)
                    yield return StartCoroutine(leapAttack.DoLeap(player, this));
                break;
            case 1:
                if (clawAttack != null)
                    yield return StartCoroutine(clawAttack.SpawnClawsCoroutine(this));
                break;
        }

        if (movement != null) movement.canMove = true;
        isAttackingSpecial = false;
    }
}
