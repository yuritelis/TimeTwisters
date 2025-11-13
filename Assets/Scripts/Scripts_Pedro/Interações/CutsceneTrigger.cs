using UnityEngine;
using System.Collections;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene para iniciar")]
    public DialogoCinematicController cinematicController;

    [Header("Configurações")]
    public bool playOnlyOnce = true;
    public string prefsKey = "Cutscene_";

    private bool executed = false;

    private void Awake()
    {
        prefsKey = prefsKey + gameObject.scene.name + "_" + name;
    }

    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // ⬇ NOVO: espera DialogoManager existir
        while (DialogoManager.Instance == null)
            yield return null;

        // ⬇ NOVO: espera o Player existir na cena
        while (GameObject.FindGameObjectWithTag("Player") == null)
            yield return null;

        // ⬇ NOVO: espera 1 frame extra para UI estabilizar
        yield return null;

        if (playOnlyOnce && PlayerPrefs.GetInt(prefsKey, 0) == 1)
        {
            executed = true;
            yield break;
        }

        if (cinematicController == null)
            cinematicController = GetComponent<DialogoCinematicController>();

        if (cinematicController == null)
        {
            Debug.LogError("❌ CutsceneTrigger: Não há DialogoCinematicController no objeto!");
            yield break;
        }

        StartCutscene();
    }

    private void StartCutscene()
    {
        if (executed) return;
        executed = true;

        if (playOnlyOnce)
            PlayerPrefs.SetInt(prefsKey, 1);

        cinematicController.IniciarCutscene();
    }
}
