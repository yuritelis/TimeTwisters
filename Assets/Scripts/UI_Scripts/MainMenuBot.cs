    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using System.Collections;

    public class BotController : MonoBehaviour
    {
        [Header("Telas do Menu")]
        [SerializeField] GameObject titleScreen;
        [SerializeField] GameObject optionsScreen;
        [SerializeField] GameObject creditsScreen;

        [Header("Transição")]
        [SerializeField] CanvasGroup fadeCanvas;  // imagem preta cobrindo a tela
        [SerializeField] float fadeSpeed = 1.2f;  // velocidade do fade
        [SerializeField] float holdBeforeLoad = 0.3f; // leve pausa antes de carregar

        public AudioManager aManager;
        private string nomeCena = "Saguão";

        private void Awake()
        {
            aManager = GameObject.FindFirstObjectByType<AudioManager>();
        }

        private void Start()
        {

            titleScreen.SetActive(true);
            optionsScreen.SetActive(false);
            creditsScreen.SetActive(false);

            // 🔹 Garante que o fade começa invisível
            if (fadeCanvas != null)
            {
                fadeCanvas.alpha = 0f;
                fadeCanvas.gameObject.SetActive(false);
            }
        }

        public void BotPlay()
        {
            aManager.PlaySFX(aManager.botClick);
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

            // 🔹 Espera um pouquinho antes de carregar
            yield return new WaitForSecondsRealtime(holdBeforeLoad);

            // 🔹 Carrega de forma assíncrona
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nomeCena);
            asyncLoad.allowSceneActivation = false;

            // 🔹 Espera a nova cena terminar de carregar
            while (asyncLoad.progress < 0.9f)
                yield return null;

            // 🔹 Agora sim, libera a transição
            asyncLoad.allowSceneActivation = true;
        }


        public void BotOptions()
        {
            aManager.PlaySFX(aManager.botClick);
            titleScreen.SetActive(false);
            optionsScreen.SetActive(true);
        }

        public void BotCredits()
        {
            aManager.PlaySFX(aManager.botClick);
            titleScreen.SetActive(false);
            creditsScreen.SetActive(true);
        }

        public void BotMenu()
        {
            aManager.PlaySFX(aManager.botClick);
            optionsScreen.SetActive(false);
            creditsScreen.SetActive(false);
            titleScreen.SetActive(true);
        }

        public void BotSair()
        {
            aManager.PlaySFX(aManager.botClick);
            Application.Quit();
        }
    }
