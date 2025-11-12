using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogoCinematicController : MonoBehaviour
{
    [Header("Referências")]
    public CameraSegue cameraSegue;
    public Transform player;
    public List<Transform> cameraPoints;

    [Header("Configuração de movimento")]
    public float cameraMoveSpeed = 2f;
    public float holdTime = 2f;

    [Header("Integração com Diálogo")]
    public Dialogo primeiroDialogo;
    public Dialogo dialogoDepoisDaCamera;

    private bool cutsceneAtiva = false;

    private void Start()
    {
        if (cameraSegue == null)
            cameraSegue = FindFirstObjectByType<CameraSegue>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void IniciarCutscene()
    {
        if (cutsceneAtiva) return;
        cutsceneAtiva = true;

        StartCoroutine(CutsceneSequencia());
    }

    private IEnumerator CutsceneSequencia()
    {
        if (primeiroDialogo != null)
        {
            DialogoManager.Instance.StartDialogo(primeiroDialogo);
            yield return new WaitUntil(() => !DialogoManager.Instance.dialogoAtivoPublico);
        }

        if (cameraSegue != null && cameraPoints.Count > 0)
        {
            foreach (var point in cameraPoints)
            {
                if (point == null) continue;

                cameraSegue.BeginTemporaryFocus(point);
                yield return MoverCameraLentamente(cameraSegue.transform, point.position);
                yield return new WaitForSeconds(holdTime);
            }

            cameraSegue.BeginTemporaryFocus(player);
            yield return MoverCameraLentamente(cameraSegue.transform, player.position);

            cameraSegue.EndTemporaryFocus();
        }

        if (dialogoDepoisDaCamera != null)
        {
            DialogoManager.Instance.StartDialogo(dialogoDepoisDaCamera);
            yield return new WaitUntil(() => !DialogoManager.Instance.dialogoAtivoPublico);
        }

        cutsceneAtiva = false;
        Debug.Log("🎬 Cutscene finalizada.");
    }

    private IEnumerator MoverCameraLentamente(Transform cam, Vector3 destino)
    {
        float t = 0;
        Vector3 inicio = cam.position;
        destino.z = cam.position.z;

        while (t < 1)
        {
            t += Time.deltaTime * cameraMoveSpeed;
            cam.position = Vector3.Lerp(inicio, destino, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }
}
