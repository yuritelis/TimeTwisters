using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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
    [Header("Configuração da Transição")]
    public string sceneToLoad;
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
#endif
    public string spawnPointName;
    public bool requireInput = false;
    public KeyCode interactKey = KeyCode.E;

    [Header("Progressão Necessária")]
    [Tooltip("Etapa mínima da história necessária para abrir a porta (0 = sempre pode interagir).")]
    public int etapaNecessaria = 0;

    [Header("Som de Porta Trancada")]
    [Tooltip("Som reproduzido quando o jogador tenta interagir sem ter a progressão necessária.")]
    public AudioClip somPortaTrancada;

    [Header("Diálogo Completo (opcional)")]
    [Tooltip("Se preenchido, será exibido ANTES da transição de cena.")]
    public Dialogo dialogoAntesDaTransicao;

    [Header("Diálogo de Porta Trancada (opcional)")]
    [Tooltip("Se preenchido, será exibido quando o jogador tentar interagir antes da hora.")]
    public Dialogo dialogoBloqueado;

    [Header("Falas Aleatórias (simples, opcionais)")]
    public List<FalaSimples> falasAleatorias = new List<FalaSimples>()
    {
        new FalaSimples { nome = "Julie", fala = "Está trancada..." },
        new FalaSimples { nome = "",      fala = "Nada acontece..." }
    };

    private bool playerInside = false;
    private bool transicionando = false;
    private PlayerController playerController;
    private Player_Combat playerCombat;

    // ============================ //
    // ===== Lógica Principal ===== //
    // ============================ //

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
        if (transicionando) yield break;
        transicionando = true;

        int etapaAtual = StoryProgressManager.instance != null
            ? StoryProgressManager.instance.historiaEtapaAtual
            : 0;

        // 🔒 Porta trancada
        if (etapaAtual < etapaNecessaria)
        {
            // 🔊 Som de porta trancada
            if (somPortaTrancada != null)
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySFX(somPortaTrancada);
                else
                    AudioSource.PlayClipAtPoint(somPortaTrancada, transform.position);
            }

            yield return new WaitForSeconds(0.25f);

            Dialogo dlg = null;

            // 💬 Se há diálogo bloqueado, ele é usado
            if (dialogoBloqueado != null &&
                dialogoBloqueado.dialogoFalas != null &&
                dialogoBloqueado.dialogoFalas.Count > 0)
            {
                dlg = dialogoBloqueado;
            }
            else if (falasAleatorias != null && falasAleatorias.Count > 0)
            {
                dlg = CriarDialogoDeFala(EscolherFalaAleatoria());
            }

            // 🎬 Roda o diálogo, mas NÃO avança progressão automaticamente
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

        // 🍿 Há diálogo antes da transição (não avança progresso)
        if (dialogoAntesDaTransicao != null &&
            dialogoAntesDaTransicao.dialogoFalas != null &&
            dialogoAntesDaTransicao.dialogoFalas.Count > 0)
        {
            TravarJogador(true);

            DialogoManager.Instance.StartDialogo(dialogoAntesDaTransicao);
            while (DialogoManager.Instance.dialogoAtivoPublico)
                yield return null;

            TravarJogador(false);
        }

        // 🌌 Troca de cena
        PerformTransition();
    }

    // ============================ //
    // ===== Métodos Auxiliares ==== //
    // ============================ //

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
        {
            Debug.LogError($"❌ Nenhuma cena configurada em {name}!");
            return;
        }

        if (!string.IsNullOrEmpty(spawnPointName))
            PlayerPrefs.SetString("SpawnPoint", spawnPointName);

        SceneManager.LoadScene(finalSceneName);
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
}
