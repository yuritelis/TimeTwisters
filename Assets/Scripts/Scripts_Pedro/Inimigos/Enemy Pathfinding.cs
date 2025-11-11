using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPathfinder : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    private NavMeshAgent agent;

    [Header("Configuração de Movimento")]
    [Tooltip("Distância mínima para parar (sem colidir com o player)")]
    public float stoppingDistance = 0.6f;

    [Tooltip("Velocidade base do inimigo (igual ao Enemy_Movement.speed)")]
    public float moveSpeed = 2f;

    [Header("Evitar Obstáculos (manual)")]
    [Tooltip("Objetos adicionais que o inimigo deve evitar (fora do NavMesh)")]
    public List<Transform> manualObstacles = new List<Transform>();

    [Tooltip("Distância máxima para começar a evitar o obstáculo")]
    public float avoidRadius = 1.2f;

    [Tooltip("Força de desvio aplicada para contornar o obstáculo")]
    public float avoidForce = 1.8f;

    [Header("Sistema Anti-Stuck")]
    [Tooltip("Tempo parado antes de forçar recalcular o caminho")]
    public float stuckTime = 1.2f;

    [Tooltip("Distância mínima de movimento para considerar que está se movendo")]
    public float unstuckThreshold = 0.02f;

    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoBraking = false;

        agent.stoppingDistance = stoppingDistance;
        agent.speed = moveSpeed;
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 target = player.position;
            Vector3 avoidOffset = CalculateAvoidanceOffset();
            MoveTo(target + avoidOffset);
        }

        DetectAndFixStuck();
    }

    // 🔹 Calcula o vetor de desvio com base em todos os obstáculos (incluindo filhos)
    private Vector3 CalculateAvoidanceOffset()
    {
        Vector3 offset = Vector3.zero;

        foreach (Transform root in manualObstacles)
        {
            if (root == null) continue;

            // inclui o próprio transform e todos os filhos
            foreach (Transform t in root.GetComponentsInChildren<Transform>())
            {
                if (t == null || t == transform) continue;

                Vector3 dirToObstacle = transform.position - t.position;
                float dist = dirToObstacle.magnitude;

                if (dist < avoidRadius && dist > 0.01f)
                {
                    float weight = Mathf.Lerp(avoidForce, 0f, dist / avoidRadius);
                    offset += dirToObstacle.normalized * weight;
                }
            }
        }

        return offset;
    }

    public void MoveTo(Vector3 target)
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 0.6f, NavMesh.AllAreas))
        {
            if (agent.destination != hit.position)
                agent.SetDestination(hit.position);
        }
    }

    private void DetectAndFixStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved < unstuckThreshold && agent.velocity.magnitude < 0.1f)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTime)
            {
                Debug.Log($"🌀 {name} preso — recalculando caminho...");

                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.3f, 0.3f),
                    0
                );

                Vector3 newPosition = transform.position + randomOffset;

                if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                    agent.Warp(hit.position);

                agent.ResetPath();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        if (agent != null)
            agent.speed = newSpeed;
    }

    public void ClearPath()
    {
        if (agent != null)
            agent.ResetPath();
    }
}
