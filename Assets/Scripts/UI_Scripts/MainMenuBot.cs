using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem.LowLevel;
using TMPro;

public class BotController : MonoBehaviour
{
    [Header("Telas do Menu")]
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject optionsScreen;
    [SerializeField] GameObject definicoesScreen;
    [SerializeField] GameObject inputsScreen;
    [SerializeField] GameObject creditsScreen;

    [Header("Botões")]
    [SerializeField] Button botDefinicoes;
    [SerializeField] Button botControles;
    [SerializeField] TextMeshProUGUI definicoesTxt;
    [SerializeField] TextMeshProUGUI controlesTxt;

    [Header("Transição")]
    [SerializeField] CanvasGroup fadeCanvas;
    [SerializeField] float fadeSpeed = 1.2f;
    [SerializeField] float holdBeforeLoad = 0.3f;

    public AudioManager aManager;
    private string nomeCena = "Saguão";
    private TelaAtiva telaAtiva = TelaAtiva.Definicoes;

    private void Awake()
    {
        aManager = GameObject.FindFirstObjectByType<AudioManager>();
    }

    private void Start()
    {
        titleScreen.SetActive(true);
        optionsScreen.SetActive(false);
        inputsScreen.SetActive(false);
        creditsScreen.SetActive(false);

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        DefinirEstadoBotao();
    }

    public void BotPlay()
    {
        if (aManager != null) aManager.PlaySFX(aManager.botClick);

        if (PlayerPrefs.HasKey("HasSave") && PlayerPrefs.GetInt("HasSave") == 1)
        {
            nomeCena = "Sala_Convidados";
            PlayerPrefs.SetString("SpawnPoint", "SpawnInicial");
        }
        else
        {
            nomeCena = "Saguão";
        }

        StartCoroutine(FadeAndLoad());
    }


    private IEnumerator FadeAndLoad()
    {
        fadeCanvas.gameObject.SetActive(true);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            fadeCanvas.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(holdBeforeLoad);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nomeCena);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        asyncLoad.allowSceneActivation = true;
    }


    public void BotOptions()
    {
        if(aManager != null) aManager.PlaySFX(aManager.botClick);

        DefinirEstadoBotao();

        titleScreen.SetActive(false);
        optionsScreen.SetActive(true);
        definicoesScreen.SetActive(true);
        inputsScreen.SetActive(false);
    }

    public void BotControles()
    {
        if (aManager != null) aManager.PlaySFX(aManager.botClick);
        definicoesScreen.SetActive(false);
        inputsScreen.SetActive(true);

        telaAtiva = TelaAtiva.Controles;
    }

    public void BotDefinicoes()
    {
        if (aManager != null) aManager.PlaySFX(aManager.botClick);
        definicoesScreen.SetActive(true);
        inputsScreen.SetActive(false);

        telaAtiva = TelaAtiva.Definicoes;
    }

    public void BotCredits()
    {
        if (aManager != null) aManager.PlaySFX(aManager.botClick);
        titleScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void BotMenu()
    {
        if (aManager != null) aManager.PlaySFX(aManager.botClick);
        optionsScreen.SetActive(false);
        inputsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        titleScreen.SetActive(true);
    }

    public void BotSair()
    {
        if (aManager != null) aManager.PlaySFX(aManager.botClick);
        Application.Quit();
    }

    public void DefinirEstadoBotao()
    {
        AtivarBotao(botDefinicoes, definicoesTxt);
        AtivarBotao(botControles, controlesTxt);

        switch (telaAtiva)
        {
            case TelaAtiva.Definicoes:
                DesativarBotao(botDefinicoes, definicoesTxt);
                break;
            case TelaAtiva.Controles:
                DesativarBotao(botControles, controlesTxt);
                break;
        }
    }

    public void DesativarBotao(Button bot, TextMeshProUGUI text)
    {
        if (bot == null) return;
        bot.interactable = false;
        text.color = Color.black;
    }

    public void AtivarBotao(Button bot, TextMeshProUGUI text)
    {
        if (bot == null) return;
        bot.interactable = true;
        text.color = Color.white;
    }

    public enum TelaAtiva
    {
        Definicoes,
        Controles
    }
}
