using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerStealth : MonoBehaviour
{
    [Header("Stealth")]
    public LayerMask enemyLayer;
    public float stealthRange = 1.5f;
    public float stealthKillDelay = 1f;
    public PlayerController playerController;

    private PlayerInput input;
    private InputAction attackAction;
    private bool isStealthExecuting = false;
    private Rigidbody2D rbPlayer;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rbPlayer = GetComponent<Rigidbody2D>();

        if (input == null)
        {
            Debug.LogError("[PlayerStealth] PlayerInput não encontrado no Player.");
            return;
        }

        attackAction = input.actions.FindAction("Player/Attack", true);
        if (attackAction == null)
            attackAction = input.currentActionMap?.FindAction("Attack", true);

        if (attackAction == null)
            Debug.LogError("[PlayerStealth] Ação 'Attack' não encontrada no mapa 'Player'.");
        else
            Debug.Log("[PlayerStealth] Ação 'Attack' carregada corretamente!");
    }

    private void OnEnable()
    {
        if (attackAction != null)
        {
            attackAction.Enable();
            attackAction.performed += TryStealthKill;
        }
    }

    private void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.performed -= TryStealthKill;
            attackAction.Disable();
        }
    }

    private void TryStealthKill(InputAction.CallbackContext ctx)
    {
        if (isStealthExecuting) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stealthRange, enemyLayer);
        if (hits.Length == 0) return;

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy_Movement>();
            if (enemy == null) continue;

            if (CanStealthKill(enemy, out float dist, out float angle, out float dot))
            {
                Debug.Log($"💀 Stealth kill iniciado em {enemy.name}");
                StartCoroutine(DoStealthKill(enemy));
                break;
            }
        }
    }

    private IEnumerator DoStealthKill(Enemy_Movement enemy)
    {
        isStealthExecuting = true;

        // 🔒 Desativa controle e movimentação do player
        if (playerController != null)
        {
            playerController.canMove = false;
        }

        if (input != null)
        {
            input.currentActionMap.Disable(); // ✅ bloqueia todos os inputs temporariamente
        }

        if (rbPlayer != null)
        {
            rbPlayer.linearVelocity = Vector2.zero;
            rbPlayer.constraints = RigidbodyConstraints2D.FreezeAll; // congela fisicamente
        }

        // 🔒 trava o inimigo
        enemy.enabled = false;
        var rbEnemy = enemy.GetComponent<Rigidbody2D>();
        if (rbEnemy) rbEnemy.linearVelocity = Vector2.zero;

        // Desativa knockback temporariamente
        var enemyKnockback = enemy.GetComponent<Enemy_Knockback>();
        if (enemyKnockback != null)
            enemyKnockback.enabled = false;

        // ⏳ espera o tempo da execução (animação futura)
        yield return new WaitForSeconds(stealthKillDelay);

        // 💀 executa a morte do inimigo
        enemy.ReceiveStealthKill();

        // aguarda até o inimigo sumir
        yield return new WaitUntil(() => enemy == null);

        // 🔓 Libera tudo de volta
        if (playerController != null)
        {
            playerController.canMove = true;
        }

        if (input != null)
        {
            input.currentActionMap.Enable(); // reativa input do Player
        }

        if (rbPlayer != null)
        {
            rbPlayer.constraints = RigidbodyConstraints2D.FreezeRotation; // volta ao normal
        }

        isStealthExecuting = false;
    }

    private bool CanStealthKill(Enemy_Movement enemy, out float dist, out float angle, out float dot)
    {
        Transform eye = enemy.detectionPoint != null ? enemy.detectionPoint : enemy.transform;

        dist = Vector2.Distance(transform.position, enemy.transform.position);
        if (dist > stealthRange)
        {
            angle = 0f; dot = 1f;
            return false;
        }

        Vector2 enemyForward = enemy.facingDirection.sqrMagnitude > 0 ? enemy.facingDirection.normalized : Vector2.right;
        Vector2 dirToPlayer = ((Vector2)transform.position - (Vector2)eye.position).normalized;

        angle = Vector2.Angle(enemyForward, dirToPlayer);
        dot = Vector2.Dot(enemyForward, dirToPlayer);

        bool isBehind = dot < 0f;
        bool outsideVisionCone = angle > enemy.visionAngle;

        return isBehind && outsideVisionCone;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stealthRange);
    }
}
