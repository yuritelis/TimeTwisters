using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CutsceneFugaController : MonoBehaviour
{
    [Header("Referências Principais")]
    public CameraSegue cameraSegue;
    public Transform player;

    [Header("Sistema de Execução Única")]
    public int etapaProgressoAoConcluir = 1;
    private bool cutsceneJaExecutada = false;

    [Header("Movimentação do Player por Pontos")]
    public Transform[] playerMovePoints;
    public float playerCutsceneSpeed = 3f;

    [Header("Ponto onde estão os NPCs que terão diálogo")]
    public Transform cameraNPCFocusPoint;

    [Header("NPC Que Vai Fugir")]
    public Transform npcFugitivo;
    public Transform[] npcFugitivoPoints;
    public float npcFugitivoSpeed = 3f;

    [Header("NPCs Removidos ao Final")]
    public List<Transform> npcsParaRemover;

    [Header("Diálogos")]
    public Dialogo dialogoInicialPlayer;
    public Dialogo dialogoNpcScene;
    public Dialogo dialogoFinalPlayer;

    [Header("Camera Config")]
    public float cameraMoveDuration = 1f;
    public AnimationCurve cameraEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Transform focusProxy;
    private Coroutine cameraRoutine;
    private bool cutsceneAtiva = false;

    private Rigidbody2D playerRb;
    private Animator playerAnim;
    private PlayerController playerController;
    private Player_Combat playerCombat;
    private PlayerDash playerDash;
    private PlayerInput playerInput;

    private void GarantirReferenciasDoPlayer()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null) return;

        playerRb = player.GetComponent<Rigidbody2D>();
        playerAnim = player.GetComponent<Animator>();
        playerController = player.GetComponent<PlayerController>();
        playerCombat = player.GetComponent<Player_Combat>();
        playerDash = player.GetComponent<PlayerDash>();
        playerInput = player.GetComponent<PlayerInput>();
    }

    private void Start()
    {
        cutsceneJaExecutada = PlayerPrefs.GetInt($"{name}_Executada", 0) == 1;

        focusProxy = new GameObject($"{name}_CameraProxy").transform;
        focusProxy.hideFlags = HideFlags.HideInHierarchy;

        GarantirReferenciasDoPlayer();

        if (cameraSegue == null)
            cameraSegue = FindFirstObjectByType<CameraSegue>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (cutsceneAtiva) return;
        if (cutsceneJaExecutada) return;

        GarantirReferenciasDoPlayer();

        StartCoroutine(SequenciaCutscene());
    }

    private IEnumerator SequenciaCutscene()
    {
        cutsceneAtiva = true;

        TravarJogador(true);

        DialogoManager.Instance.OnFalaIniciada += HandleFalaIniciada;

        if (dialogoInicialPlayer != null)
        {
            DialogoManager.Instance.StartDialogo(dialogoInicialPlayer);
            yield return new WaitUntil(() => !DialogoManager.Instance.dialogoAtivoPublico);
        }

        for (int i = 0; i < playerMovePoints.Length; i++)
            yield return StartCoroutine(MoverPlayer(playerMovePoints[i].position));

        yield return StartCoroutine(FocarCamera(cameraNPCFocusPoint.position));

        if (dialogoNpcScene != null)
        {
            DialogoManager.Instance.StartDialogo(dialogoNpcScene);
            yield return new WaitUntil(() => !DialogoManager.Instance.dialogoAtivoPublico);
        }

        cameraRoutine = StartCoroutine(FocarCameraSeguindo(npcFugitivo));

        for (int i = 0; i < npcFugitivoPoints.Length; i++)
            yield return StartCoroutine(MoverTransform(npcFugitivo, npcFugitivoPoints[i].position, npcFugitivoSpeed));

        npcFugitivo.gameObject.SetActive(false);

        if (cameraRoutine != null)
            StopCoroutine(cameraRoutine);

        yield return StartCoroutine(FocarCamera(player.position));

        if (dialogoFinalPlayer != null)
        {
            DialogoManager.Instance.StartDialogo(dialogoFinalPlayer);
            yield return new WaitUntil(() => !DialogoManager.Instance.dialogoAtivoPublico);
        }

        foreach (var npc in npcsParaRemover)
            if (npc != null) Destroy(npc.gameObject);

        DialogoManager.Instance.OnFalaIniciada -= HandleFalaIniciada;

        if (cameraRoutine != null)
        {
            StopCoroutine(cameraRoutine);
            cameraRoutine = null;
        }

        cameraSegue.BeginTemporaryFocus(null);

        TravarJogador(false);

        if (!cutsceneJaExecutada)
        {
            cutsceneJaExecutada = true;
            PlayerPrefs.SetInt($"{name}_Executada", 1);
            PlayerPrefs.Save();

            if (StoryProgressManager.instance != null)
                StoryProgressManager.instance.AvancarEtapa();
        }
    }

    private IEnumerator MoverPlayer(Vector3 destino)
    {
        while (Vector2.Distance(player.position, destino) > 0.1f)
        {
            Vector2 dir = (destino - player.position).normalized;

            if (playerRb != null)
                playerRb.linearVelocity = dir * playerCutsceneSpeed;

            if (playerAnim != null)
            {
                playerAnim.SetBool("isWalking", true);
                playerAnim.SetFloat("InputX", dir.x);
                playerAnim.SetFloat("InputY", dir.y);
                playerAnim.SetFloat("LastInputX", dir.x);
                playerAnim.SetFloat("LastInputY", dir.y);
            }

            yield return null;
        }

        if (playerRb != null)
            playerRb.linearVelocity = Vector2.zero;

        if (playerAnim != null)
        {
            playerAnim.SetBool("isWalking", false);
            playerAnim.SetFloat("InputX", 0);
            playerAnim.SetFloat("InputY", 0);
            playerAnim.SetFloat("LastInputX", 0);
            playerAnim.SetFloat("LastInputY", -1);
        }

        yield return null;
    }

    private IEnumerator MoverTransform(Transform t, Vector3 destino, float speed)
    {
        while (Vector2.Distance(t.position, destino) > 0.1f)
        {
            Vector2 dir = (destino - t.position).normalized;
            t.position += (Vector3)(dir * speed * Time.deltaTime);
            yield return null;
        }
        t.position = destino;
    }

    private IEnumerator FocarCamera(Vector3 target)
    {
        cameraSegue.BeginTemporaryFocus(focusProxy);
        focusProxy.position = cameraSegue.transform.position;

        Vector3 start = focusProxy.position;
        Vector3 end = new Vector3(target.x, target.y, start.z);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / cameraMoveDuration;
            focusProxy.position = Vector3.Lerp(start, end, cameraEase.Evaluate(t));
            yield return null;
        }

        focusProxy.position = end;
    }

    private IEnumerator FocarCameraSeguindo(Transform alvo)
    {
        while (alvo != null && alvo.gameObject.activeInHierarchy)
        {
            Vector3 pos = new Vector3(alvo.position.x, alvo.position.y, focusProxy.position.z);
            focusProxy.position = pos;
            cameraSegue.BeginTemporaryFocus(focusProxy);
            yield return null;
        }
    }

    private void HandleFalaIniciada(DialogoFalas fala)
    {
        if (fala.focoCamera == null) return;
        if (cameraRoutine != null)
            StopCoroutine(cameraRoutine);
        cameraRoutine = StartCoroutine(FocarCamera(fala.focoCamera.position));
    }

    private void TravarJogador(bool estado)
    {
        if (playerController != null)
            playerController.enabled = !estado;

        if (playerInput != null)
            playerInput.enabled = !estado;

        if (playerCombat != null)
            playerCombat.enabled = !estado;

        if (playerDash != null)
            playerDash.enabled = !estado;

        if (estado)
        {
            if (playerRb != null)
                playerRb.linearVelocity = Vector2.zero;

            if (playerAnim != null)
                playerAnim.SetBool("isWalking", false);
        }
    }
}
