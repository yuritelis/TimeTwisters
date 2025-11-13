using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogoCinematicController : MonoBehaviour
{
    [Header("Referências")]
    public CameraSegue cameraSegue;
    public Transform player;

    [Header("Diálogos da Cutscene")]
    public Dialogo primeiroDialogo;
    public Dialogo dialogoDepoisDaCamera;

    [Header("Câmera / Movimento Suave")]
    public float cameraMoveDuration = 0.9f;
    public AnimationCurve cameraEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Transform focusProxy;
    private bool cutsceneAtiva = false;
    private Coroutine cameraRoutine;

    private void Start()
    {
        if (cameraSegue == null)
            cameraSegue = FindFirstObjectByType<CameraSegue>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        focusProxy = new GameObject($"{name}_CameraProxy").transform;
        focusProxy.hideFlags = HideFlags.HideInHierarchy;
    }

    public void IniciarCutscene()
    {
        if (cutsceneAtiva) return;

        cutsceneAtiva = true;

        StartCoroutine(CutsceneSequencia());
    }

    private IEnumerator CutsceneSequencia()
    {
        DialogoManager.Instance.OnFalaIniciada += HandleFalaIniciada;

        if (primeiroDialogo != null)
        {
            DialogoManager.Instance.StartDialogo(primeiroDialogo);
            yield return new WaitUntil(() => !DialogoManager.Instance.dialogoAtivoPublico);
        }

        DialogoManager.Instance.OnFalaIniciada -= HandleFalaIniciada;

        if (dialogoDepoisDaCamera != null)
        {
            DialogoManager.Instance.OnFalaIniciada += HandleFalaIniciada;

            DialogoManager.Instance.StartDialogo(dialogoDepoisDaCamera);
            yield return new WaitUntil(() => !DialogoManager.Instance.dialogoAtivoPublico);

            DialogoManager.Instance.OnFalaIniciada -= HandleFalaIniciada;
        }

        cutsceneAtiva = false;
        Debug.Log("🎬 Cutscene inicial finalizada.");
    }

    private void HandleFalaIniciada(DialogoFalas fala)
    {
        Vector3 alvo = (fala.focoCamera != null ? fala.focoCamera.position : player.position);

        if (cameraRoutine != null)
            StopCoroutine(cameraRoutine);

        focusProxy.position = new Vector3(alvo.x, alvo.y, focusProxy.position.z);

        cameraRoutine = StartCoroutine(SmoothFocusTo(alvo));
    }

    private IEnumerator SmoothFocusTo(Vector3 pos)
    {
        cameraSegue.BeginTemporaryFocus(focusProxy);

        Vector3 start = focusProxy.position;
        Vector3 end = new(pos.x, pos.y, start.z);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / cameraMoveDuration;
            focusProxy.position = Vector3.Lerp(start, end, cameraEase.Evaluate(t));
            yield return null;
        }

        focusProxy.position = end;
    }
}
