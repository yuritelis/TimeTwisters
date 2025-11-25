using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CutsceneFugaController : MonoBehaviour
{
    public CameraSegue cameraSegue;
    public Transform player;

    public int etapaProgressoAoConcluir = 1;
    private bool cutsceneJaExecutada = false;

    public Transform[] playerMovePoints;
    public float playerCutsceneSpeed = 3f;

    public Transform cameraNPCFocusPoint;

    public Transform npcFugitivo;
    public Transform[] npcFugitivoPoints;
    public float npcFugitivoSpeed = 3f;

    public List<Transform> npcsParaRemover;

    public Dialogo dialogoInicialPlayer;
    public Dialogo dialogoNpcScene;
    public Dialogo dialogoFinalPlayer;

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

    public AudioClip cutsceneMusic;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RecuperarReferencias());
    }

    private void Start()
    {
        cutsceneJaExecutada = PlayerPrefs.GetInt($"{name}_Executada", 0) == 1;

        focusProxy = new GameObject($"{name}_CameraProxy").transform;
        focusProxy.hideFlags = HideFlags.HideInHierarchy;

        StartCoroutine(RecuperarReferencias());
    }

    private IEnumerator RecuperarReferencias()
    {
        yield return null;

        GarantirReferenciasDoPlayer();

        if (player == null)
        {
            yield return new WaitForSeconds(0.05f);
            GarantirReferenciasDoPlayer();
        }

        if (cameraSegue == null)
            cameraSegue = FindFirstObjectByType<CameraSegue>();
    }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (cutsceneAtiva)
            return;

        if (cutsceneJaExecutada)
            return;

        GarantirReferenciasDoPlayer();
        StartCoroutine(SequenciaCutscene());
    }

    private IEnumerator SequenciaCutscene()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.TocarMusicaCutscene(cutsceneMusic);

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

        if (AudioManager.instance != null)
            AudioManager.instance.RestaurarMusicaNormal();
    }

    private IEnumerator MoverPlayer(Vector3 destino)
    {
        float stuckTimer = 0f;
        float stuckTime = 0.45f;
        float stuckVelocityThreshold = 0.08f;

        int safety = 0;

        while (true)
        {
            float dist = Vector2.Distance(player.position, destino);

            if (dist <= 0.15f)
                break;

            safety++;
            if (safety > 900)
                break;

            Vector2 dir = (destino - player.position).normalized;

            if (playerRb != null)
                playerRb.linearVelocity = dir * playerCutsceneSpeed;

            float vel = playerRb.linearVelocity.magnitude;

            if (vel < stuckVelocityThreshold)
            {
                stuckTimer += Time.deltaTime;

                if (stuckTimer >= stuckTime)
                    break;
            }
            else
            {
                stuckTimer = 0f;
            }

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