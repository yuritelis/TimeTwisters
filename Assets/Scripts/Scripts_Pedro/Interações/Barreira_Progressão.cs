using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Barreira_Progressao : MonoBehaviour
{
    public int progressaoNecessaria = 1;
    public float recuoDistancia = 0.5f;
    public float recuoVelocidade = 3f;

    public Dialogo dialogoBloqueio;

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
        var dash = player.GetComponent<PlayerDash>();
        var input = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        var rb = player.GetComponent<Rigidbody2D>();
        var anim = player.GetComponent<Animator>();

        if (controller != null) controller.enabled = false;
        if (combat != null) combat.enabled = false;
        if (dash != null)
        {
            dash.enabled = false;
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
        if (input != null) input.enabled = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        float inputX = anim.GetFloat("InputX");
        float inputY = anim.GetFloat("InputY");

        Vector2 direcaoRecuo = new Vector2(-inputX, -inputY).normalized;

        if (Mathf.Abs(direcaoRecuo.x) > Mathf.Abs(direcaoRecuo.y))
            direcaoRecuo = new Vector2(Mathf.Sign(direcaoRecuo.x), 0);
        else
            direcaoRecuo = new Vector2(0, Mathf.Sign(direcaoRecuo.y));

        anim.SetFloat("InputX", direcaoRecuo.x);
        anim.SetFloat("InputY", direcaoRecuo.y);
        anim.SetFloat("LastInputX", direcaoRecuo.x);
        anim.SetFloat("LastInputY", direcaoRecuo.y);

        if (dialogoBloqueio != null && DialogoManager.Instance != null)
        {
            DialogoManager.Instance.StartDialogo(dialogoBloqueio);
            while (DialogoManager.Instance.dialogoAtivoPublico)
                yield return null;
        }

        anim.SetBool("isWalking", true);

        Vector2 origem = player.transform.position;
        Vector2 destino = origem + direcaoRecuo * recuoDistancia;

        float t = 0f;
        while (t < 1f)
        {
            Vector2 pos = Vector2.Lerp(origem, destino, t);
            player.transform.position = pos;

            anim.SetFloat("InputX", direcaoRecuo.x);
            anim.SetFloat("InputY", direcaoRecuo.y);
            anim.SetFloat("LastInputX", direcaoRecuo.x);
            anim.SetFloat("LastInputY", direcaoRecuo.y);

            t += Time.deltaTime * recuoVelocidade;
            yield return null;
        }

        anim.SetBool("isWalking", false);
        anim.SetFloat("InputX", 0);
        anim.SetFloat("InputY", 0);
        anim.SetFloat("LastInputX", 0);
        anim.SetFloat("LastInputY", -1);

        if (controller != null) controller.enabled = true;
        if (combat != null) combat.enabled = true;
        if (dash != null) dash.enabled = true;
        if (input != null) input.enabled = true;

        bloqueando = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawCube(transform.position, GetComponent<Collider2D>().bounds.size);
    }
}
