using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class FalaSimples
{
    public string nome;
    public Sprite retrato;
    [TextArea(2, 4)] public string fala;
}

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;

#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif

    public string spawnPointName;
    public bool requireInput = false;
    public KeyCode interactKey = KeyCode.E;

    public int etapaNecessaria = 0;
    public int etapaParaDialogoBloqueado = 0;

    public AudioClip somPortaTrancada;

    public Dialogo dialogoAntesDaTransicao;
    public Dialogo dialogoBloqueado;

    public List<FalaSimples> falasAleatorias = new List<FalaSimples>()
    {
        new FalaSimples { nome = "Julie", fala = "Está trancada..." },
        new FalaSimples { nome = "",      fala = "Nada acontece..." }
    };

    public float transitionCooldown = 1.2f;
    private static float lastTransitionTime = -999f;

    private bool playerInside = false;
    private bool transicionando = false;
    private PlayerController playerController;
    private Player_Combat playerCombat;

    private void Update()
    {
        if (requireInput && playerInside && Input.GetKeyDown(interactKey))
            StartCoroutine(TentarTransicao());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        playerController = other.GetComponent<PlayerController>();
        playerCombat = other.GetComponent<Player_Combat>();

        if (!requireInput)
            StartCoroutine(TentarTransicao());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private IEnumerator TentarTransicao()
    {
        if (Time.time < lastTransitionTime + transitionCooldown)
            yield break;

        lastTransitionTime = Time.time;

        if (transicionando) yield break;
        transicionando = true;

        int etapaAtual = StoryProgressManager.instance != null
            ? StoryProgressManager.instance.historiaEtapaAtual
            : 0;

        if (etapaAtual < etapaNecessaria)
        {
            if (somPortaTrancada != null)
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySFX(somPortaTrancada);
                else
                    AudioSource.PlayClipAtPoint(somPortaTrancada, transform.position);
            }

            yield return new WaitForSeconds(0.25f);

            Dialogo dlg = null;

            bool podeMostrarDialogoBloqueado =
                etapaParaDialogoBloqueado == 0 || etapaAtual >= etapaParaDialogoBloqueado;

            bool temFalasValidasDoBloqueado = false;

            if (podeMostrarDialogoBloqueado &&
                dialogoBloqueado != null &&
                dialogoBloqueado.dialogoFalas != null &&
                dialogoBloqueado.dialogoFalas.Count > 0)
            {
                foreach (var fala in dialogoBloqueado.dialogoFalas)
                {
                    if (etapaAtual >= fala.etapaMinima)
                    {
                        temFalasValidasDoBloqueado = true;
                        break;
                    }
                }
            }

            if (podeMostrarDialogoBloqueado && temFalasValidasDoBloqueado)
            {
                dlg = dialogoBloqueado;
            }
            else
            {
                dlg = CriarDialogoDeFala(EscolherFalaAleatoria());
            }

            if (dlg != null && DialogoManager.Instance != null)
            {
                TravarJogador(true);

                DialogoManager.Instance.StartDialogo(dlg);
                while (DialogoManager.Instance.dialogoAtivoPublico)
                    yield return null;

                TravarJogador(false);
            }

            transicionando = false;
            yield break;
        }

        if (dialogoAntesDaTransicao != null &&
            dialogoAntesDaTransicao.dialogoFalas != null &&
            dialogoAntesDaTransicao.dialogoFalas.Count > 0)
        {
            if (DialogoManager.Instance != null)
            {
                TravarJogador(true);

                DialogoManager.Instance.StartDialogo(dialogoAntesDaTransicao);
                while (DialogoManager.Instance.dialogoAtivoPublico)
                    yield return null;

                TravarJogador(false);
            }
        }

        PerformTransition();
    }

    private FalaSimples EscolherFalaAleatoria()
    {
        int i = Random.Range(0, falasAleatorias.Count);
        return falasAleatorias[i];
    }

    private Dialogo CriarDialogoDeFala(FalaSimples fala)
    {
        return new Dialogo
        {
            dialogoFalas = new List<DialogoFalas>
            {
                new DialogoFalas
                {
                    personagem = new PersoInfos
                    {
                        nome = fala.nome,
                        portrait = fala.retrato
                    },
                    fala = fala.fala
                }
            }
        };
    }

    private void PerformTransition()
    {
        string finalSceneName = GetSceneName();

        if (string.IsNullOrEmpty(finalSceneName))
            return;

        if (!string.IsNullOrEmpty(spawnPointName))
            PlayerPrefs.SetString("SpawnPoint", spawnPointName);

        StartCoroutine(FadeAndLoad(finalSceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeOut();

        SceneManager.LoadScene(sceneName);

        yield return null;

        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeIn();
    }

    private string GetSceneName()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
            return sceneAsset.name;
#endif
        return sceneToLoad;
    }

    private void TravarJogador(bool estado)
    {
        if (playerController != null)
            playerController.canMove = !estado;

        if (playerCombat != null)
            playerCombat.enabled = !estado;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(sceneAsset);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

            if (sceneToLoad != sceneName)
            {
                sceneToLoad = sceneName;
                EditorUtility.SetDirty(this);
            }
        }
    }
#endif
}