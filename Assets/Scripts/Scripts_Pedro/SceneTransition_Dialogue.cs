using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTransitionDialogProgress : MonoBehaviour
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

    [Header("Diálogo e Progressão")]
    public Dialogo dialogoInicial;
    [Tooltip("Etapa de progressão mínima para pular o diálogo")]
    public int etapaParaPularDialogo = 1;
    [Tooltip("Etapa atribuída após completar o diálogo (ganhar a arma)")]
    public int etapaPosDialogo = 2;

    private bool playerInside = false;
    private bool transicionando = false;
    private PlayerController playerController;
    private Player_Combat playerCombat;

    private void Start()
    {
        if (StoryProgressManager.instance != null)
        {
            if (StoryProgressManager.instance.historiaEtapaAtual < etapaPosDialogo)
            {
                playerCombat = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player_Combat>();
                if (playerCombat != null)
                {
                    playerCombat.enabled = false;
                    Debug.Log("🗡️ Combate bloqueado até o jogador pegar a arma.");
                }
            }
        }
    }

    private void Update()
    {
        if (requireInput && playerInside && Input.GetKeyDown(interactKey))
            StartCoroutine(TentarTransicao());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerController = other.GetComponent<PlayerController>();
            playerCombat = other.GetComponent<Player_Combat>();

            if (!requireInput)
                StartCoroutine(TentarTransicao());
        }
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

        if (StoryProgressManager.instance == null ||
            StoryProgressManager.instance.historiaEtapaAtual <= etapaParaPularDialogo)
        {
            if (dialogoInicial != null)
            {
                TravarJogador(true);

                Debug.Log("🗨️ Iniciando diálogo antes da transição...");
                DialogoManager.Instance.StartDialogo(dialogoInicial);

                while (DialogoManager.Instance != null && DialogoManager.Instance.dialogoAtivoPublico)
                    yield return null;

                Debug.Log("✅ Diálogo encerrado — liberando combate e salvando progresso.");

                if (playerCombat != null)
                    playerCombat.enabled = true;

                if (StoryProgressManager.instance != null)
                    StoryProgressManager.instance.DefinirEtapa(etapaPosDialogo);

                yield return new WaitForSecondsRealtime(0.3f);
            }
        }

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

        Debug.Log(estado ? "🧊 Player travado (durante diálogo de arma)." : "🔥 Player liberado.");
    }
}
