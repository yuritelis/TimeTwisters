using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Barreira_Progressão : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Etapa mínima da história necessária para passar.")]
    public int progressaoNecessaria = 1;

    [Tooltip("Distância que o jogador recua ao tentar passar.")]
    public float recuoDistancia = 0.5f;

    [Tooltip("Velocidade de recuo.")]
    public float recuoVelocidade = 3f;

    [Header("Diálogo")]
    [Tooltip("Diálogo exibido se o jogador tentar passar antes da hora (deixe vazio para usar o texto padrão).")]
    public Dialogo dialogoBloqueio;

    [Tooltip("Texto usado se nenhum diálogo for atribuído.")]
    [TextArea(2, 4)]
    public string falaPadrao = "Acho que não devo ir por aqui ainda...";

    private bool bloqueando = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (bloqueando) return;
        if (!other.CompareTag("Player")) return;

        int progressoAtual = StoryProgressManager.instance != null
            ? StoryProgressManager.instance.historiaEtapaAtual
            : 0;

        if (progressoAtual < progressaoNecessaria)
            StartCoroutine(RejeitarEntrada(other.gameObject));
    }

    private IEnumerator RejeitarEntrada(GameObject player)
    {
        bloqueando = true;

        var controller = player.GetComponent<PlayerController>();
        var combat = player.GetComponent<Player_Combat>();
        var rb = player.GetComponent<Rigidbody2D>();
        var anim = player.GetComponent<Animator>();

        // 1️⃣ Desativa o controle completamente (DIÁLOGO + MOVIMENTAÇÃO)
        if (controller != null) controller.canMove = false;
        if (combat != null) combat.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 2️⃣ Pega a direção atual do Input (não LastInput)
        float inputX = anim.GetFloat("InputX");
        float inputY = anim.GetFloat("InputY");

        // 3️⃣ Calcula direção de recuo (oposta ao input atual)
        Vector2 direcaoRecuo = new Vector2(-inputX, -inputY).normalized;

        // 4️⃣ Vira o jogador pra direção de recuo
        if (anim != null)
        {
            anim.SetFloat("InputX", direcaoRecuo.x);
            anim.SetFloat("InputY", direcaoRecuo.y);
        }

        // 5️⃣ Exibe diálogo (CONTROLE JÁ ESTÁ BLOQUEADO)
        Dialogo dlg = dialogoBloqueio ?? CriarDialogoAutomatico(falaPadrao);

        if (DialogoManager.Instance != null)
        {
            DialogoManager.Instance.StartDialogo(dlg);
            while (DialogoManager.Instance.dialogoAtivoPublico)
                yield return null;
        }
        else
        {
            Debug.Log($"🗣️ {falaPadrao}");
            yield return new WaitForSeconds(1f); // Pequena pausa sem diálogo
        }

        // 6️⃣ Move automaticamente pra trás (CONTROLE AINDA BLOQUEADO)
        if (anim != null)
            anim.SetBool("isWalking", true);

        Vector2 origem = player.transform.position;
        Vector2 destino = origem + direcaoRecuo * recuoDistancia;

        float t = 0;
        while (t < 1f)
        {
            player.transform.position = Vector2.Lerp(origem, destino, t);
            t += Time.deltaTime * recuoVelocidade;
            yield return null;
        }

        // 7️⃣ Para a animação
        if (anim != null)
            anim.SetBool("isWalking", false);

        // 8️⃣ Só devolve o controle APÓS terminar toda a movimentação
        if (controller != null) controller.canMove = true;
        if (combat != null) combat.enabled = true;

        bloqueando = false;
    }

    private Dialogo CriarDialogoAutomatico(string texto)
    {
        return new Dialogo
        {
            dialogoFalas = new List<DialogoFalas>
            {
                new DialogoFalas
                {
                    personagem = new PersoInfos { nome = "Julie", portrait = null },
                    fala = texto
                }
            }
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawCube(transform.position, GetComponent<Collider2D>().bounds.size);
    }
}