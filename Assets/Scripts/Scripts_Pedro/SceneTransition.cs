using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTransition : MonoBehaviour
{
    [Header("Configuração da Transição")]
    [Tooltip("Nome exato da cena para carregar (se não arrastar uma cena)")]
    public string sceneToLoad;

#if UNITY_EDITOR
    [Tooltip("Você pode arrastar uma cena aqui como alternativa ao nome")]
    public SceneAsset sceneAsset;
#endif

    [Tooltip("Nome do ponto de spawn na próxima cena (opcional)")]
    public string spawnPointName;

    [Tooltip("Requer apertar uma tecla para transicionar (ex: E)")]
    public bool requireInput = false;

    [Tooltip("Tecla usada para ativar a transição")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Progressão Necessária")]
    [Tooltip("Etapa mínima da história necessária para abrir a porta (0 = sempre pode interagir)")]
    public int etapaNecessaria = 0;

    [Tooltip("Diálogo exibido se o jogador tentar interagir antes da hora (deixe vazio para usar o padrão automático)")]
    public Dialogo dialogoPortaTrancada;

    private bool playerInside = false;
    private bool transicionando = false;
    private PlayerController playerController;
    private Player_Combat playerCombat;

    private void Reset()
    {
        etapaNecessaria = 0; // garante default 0 ao adicionar o componente
    }

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

        // 🔒 Verifica progressão mínima
        if (etapaAtual < etapaNecessaria)
        {
            // Usa o diálogo configurado ou monta um padrão seguro
            Dialogo dlg = dialogoPortaTrancada ?? CriarDialogoAutomatico("Está trancada...");

            if (DialogoManager.Instance != null)
            {
                TravarJogador(true);
                Debug.Log($"🚪 Porta trancada — etapa atual ({etapaAtual}) < necessária ({etapaNecessaria}).");

                DialogoManager.Instance.StartDialogo(dlg);

                while (DialogoManager.Instance != null && DialogoManager.Instance.dialogoAtivoPublico)
                    yield return null;

                TravarJogador(false);
            }
            else
            {
                Debug.LogWarning("⚠️ Nenhum DialogoManager encontrado na cena.");
            }

            transicionando = false;
            yield break;
        }

        // ✅ Progresso suficiente: transiciona
        PerformTransition();
    }

    private void PerformTransition()
    {
        string finalSceneName = GetSceneName();

        if (string.IsNullOrEmpty(finalSceneName))
        {
            Debug.LogError($"❌ Nenhuma cena configurada em {name}. Insira o nome ou arraste a cena no Inspector!");
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

        Debug.Log(estado ? "🧊 Player travado (porta trancada)." : "🔥 Player liberado.");
    }

    // 🧩 Cria um Dialogo simples em runtime evitando NullReference no DialogoManager
    private Dialogo CriarDialogoAutomatico(string texto)
    {
        return new Dialogo
        {
            dialogoFalas = new List<DialogoFalas>
            {
                new DialogoFalas
                {
                    // personagem preenchido para evitar NRE ao acessar .nome
                    personagem = new PersoInfos { nome = "", portrait = null },
                    fala = texto
                }
            }
        };
    }
}
