using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance;

    private Dialogo dialogoData;

    [Header("Referências de UI")]
    public GameObject dialogoPanel;
    public GameObject sanidadeBar;
    public TextMeshProUGUI dialogoTxt;
    public TextMeshProUGUI personagemNome;
    public Image personagemIcon;
    public DialogoEscolhaUI escolhaUI;

    [Header("Configuração")]
    public float velFala = 0.03f;

    private int dialogoIndex;
    private bool isTyping;
    private bool isDialogoAtivo;
    [HideInInspector] public bool dialogoAtivoPublico;

    [Header("Referência ao Player")]
    public GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogo(Dialogo dialogo)
    {
        if (dialogo == null || dialogo.dialogoFalas.Count == 0)
        {
            Debug.LogError("⚠️ Dados do diálogo estão vazios ou nulos!");
            return;
        }

        isDialogoAtivo = true;
        dialogoAtivoPublico = true;
        dialogoData = dialogo;
        dialogoIndex = 0;

        TravarJogador(true);

        dialogoPanel.SetActive(true);
        if (sanidadeBar != null) sanidadeBar.SetActive(false);

        ProxLinha();
    }

    public void ProxLinha()
    {
        if (!isDialogoAtivo || dialogoData == null) return;

        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
            dialogoTxt.text = dialogoData.dialogoFalas[dialogoIndex].fala;
            dialogoIndex++;
            return;
        }

        if (dialogoIndex >= dialogoData.dialogoFalas.Count)
        {
            FimDialogo();
            return;
        }

        var falaAtual = dialogoData.dialogoFalas[dialogoIndex];

        if (personagemNome != null)
            personagemNome.text = falaAtual.personagem.nome;

        if (personagemIcon != null && falaAtual.personagem.portrait != null)
            personagemIcon.sprite = falaAtual.personagem.portrait;

        StartCoroutine(TypeLine(falaAtual.fala));

        if (falaAtual.fala.Contains("Deseja pegá-lo?") && escolhaUI != null)
        {
            StartCoroutine(EsperarParaMostrarEscolha());
        }
    }

    private IEnumerator TypeLine(string fala)
    {
        isTyping = true;
        dialogoTxt.text = "";

        foreach (char letter in fala.ToCharArray())
        {
            dialogoTxt.text += letter;
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(PersoInfos.somVoz);
            yield return new WaitForSeconds(velFala);
        }

        isTyping = false;
        dialogoIndex++;
    }

    private IEnumerator EsperarParaMostrarEscolha()
    {
        yield return new WaitUntil(() => !isTyping);
        escolhaUI.Mostrar(OnEscolhaVergalhao);
    }

    private void OnEscolhaVergalhao(bool pegou)
    {
        if (pegou)
        {
            Debug.Log("✅ Jogador escolheu pegar o vergalhão!");

            var combate = player?.GetComponent<Player_Combat>();
            if (combate != null) combate.enabled = true;

            StoryProgressManager.instance?.AvancarEtapa();
        }
        else
        {
            Debug.Log("🚫 Jogador recusou o vergalhão.");
        }

        FimDialogo();
    }

    private void Update()
    {
        if (isDialogoAtivo && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            ProxLinha();
        }
    }

    public void FimDialogo()
    {
        StopAllCoroutines();
        isDialogoAtivo = false;
        dialogoAtivoPublico = false;
        isTyping = false;

        if (dialogoTxt != null) dialogoTxt.text = "";
        if (dialogoPanel != null) dialogoPanel.SetActive(false);
        if (sanidadeBar != null) sanidadeBar.SetActive(true);

        TravarJogador(false);
        Debug.Log("✅ Diálogo finalizado.");
    }

    private void TravarJogador(bool estado)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.canMove = !estado;

        var combat = player.GetComponent<Player_Combat>();
        if (combat != null)
            combat.enabled = !estado;

        Debug.Log(estado ? "🎬 Player travado durante diálogo." : "🎮 Player liberado.");
    }
}
