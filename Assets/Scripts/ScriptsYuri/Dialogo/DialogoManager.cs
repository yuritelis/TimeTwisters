using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance;

    private Dialogo dialogoData;

    public GameObject dialogoPanel;
    public GameObject sanidadeBar;
    public GameObject hotbarPanel;
    public TextMeshProUGUI dialogoTxt;
    public TextMeshProUGUI personagemNome;
    public Image personagemIcon;
    public DialogoEscolhaUI escolhaUI;

    public float velFala = 30f;

    private int dialogoIndex;
    private bool isTyping;
    private bool isDialogoAtivo;
    public bool dialogoAtivoPublico;

    public GameObject player;

    public System.Action<DialogoFalas> OnFalaIniciada;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        RecarregarReferencias();
    }

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
        RecarregarReferencias();

        if (scene.name == "TitleScreen")
            Destroy(gameObject);
    }

    private void RecarregarReferencias()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (dialogoPanel == null)
        {
            GameObject obj = GameObject.Find("DialogoPanel");
            if (obj != null) dialogoPanel = obj.gameObject;
        }

        if (sanidadeBar == null)
        {
            GameObject obj = GameObject.Find("SanidadeBar");
            if (obj != null) sanidadeBar = obj.gameObject;
        }
    }

    public void StartDialogo(Dialogo dialogo)
    {
        if (dialogo == null || dialogo.dialogoFalas.Count == 0)
            return;

        int etapaAtual = StoryProgressManager.instance != null
            ? StoryProgressManager.instance.historiaEtapaAtual
            : 0;

        List<DialogoFalas> filtradas = new List<DialogoFalas>();

        foreach (var f in dialogo.dialogoFalas)
            if (etapaAtual >= f.etapaMinima)
                filtradas.Add(f);

        if (filtradas.Count == 0)
            return;

        dialogoData = new Dialogo { dialogoFalas = filtradas };
        dialogoIndex = 0;
        isDialogoAtivo = true;
        dialogoAtivoPublico = true;

        TravarJogador(true);

        dialogoPanel.SetActive(true);
        if (sanidadeBar != null) sanidadeBar.SetActive(false);
        if(hotbarPanel != null) hotbarPanel.SetActive(false);

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

        OnFalaIniciada?.Invoke(falaAtual);

        if (falaAtual.avancaProgressoAqui)
        {
            if (StoryProgressManager.instance != null)
                StoryProgressManager.instance.AvancarEtapa();
        }

        if (falaAtual.sfxAposFala != null)
            StartCoroutine(PlaySfxDepois(falaAtual.sfxAposFala));

        personagemNome.text = falaAtual.personagem.nome ?? "";

        if (falaAtual.personagem.portrait != null)
        {
            personagemIcon.sprite = falaAtual.personagem.portrait;
            personagemIcon.gameObject.SetActive(true);
        }
        else personagemIcon.gameObject.SetActive(false);

        StartCoroutine(TypeLine(falaAtual.fala));
    }

    private IEnumerator TypeLine(string fala)
    {
        isTyping = true;
        dialogoTxt.text = "";

        float delay = 1f / velFala;

        foreach (char c in fala)
        {
            dialogoTxt.text += c;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        dialogoIndex++;
    }

    private void Update()
    {
        if (isDialogoAtivo &&
            (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            ProxLinha();
        }
    }

    public void FimDialogo()
    {
        StopAllCoroutines();
        isTyping = false;
        isDialogoAtivo = false;
        dialogoAtivoPublico = false;

        dialogoPanel.SetActive(false);
        if (sanidadeBar != null) sanidadeBar.SetActive(true);
        if (hotbarPanel != null) hotbarPanel.SetActive(true);

        dialogoTxt.text = "";

        TravarJogador(false);
    }

    private void TravarJogador(bool estado)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var con = player.GetComponent<PlayerController>();
        var comb = player.GetComponent<Player_Combat>();

        if (con != null) con.canMove = !estado;
        if (comb != null) comb.enabled = !estado;
    }

    private IEnumerator PlaySfxDepois(AudioClip clip)
    {
        while (isTyping) yield return null;
        yield return new WaitForSeconds(0.05f);

        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(clip);
        else
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    public int GetCurrentIndex() => dialogoIndex;

    public string GetCurrentSpeakerName()
    {
        if (dialogoData == null) return "";
        int i = Mathf.Clamp(dialogoIndex, 0, dialogoData.dialogoFalas.Count - 1);
        return dialogoData.dialogoFalas[i].personagem.nome ?? "";
    }
}
