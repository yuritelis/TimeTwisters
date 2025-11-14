using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TimeTravelSceneManager : MonoBehaviour
{
    [Header("Referências das Cenas (arraste aqui)")]
    public Object cenaPresenteAsset;
    public Object cenaPassadoAsset;
    public Object cenaFuturoAsset;

    private string cenaPresente;
    private string cenaPassado;
    private string cenaFuturo;

    public static TimeTravelSceneManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarNomesDasCenas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InicializarNomesDasCenas()
    {
        cenaPresente = ObterNomeCena(cenaPresenteAsset);
        cenaPassado = ObterNomeCena(cenaPassadoAsset);
        cenaFuturo = ObterNomeCena(cenaFuturoAsset);
    }

#if UNITY_EDITOR
    private string ObterNomeCena(Object sceneAsset)
    {
        if (sceneAsset == null)
            return "";

        string path = AssetDatabase.GetAssetPath(sceneAsset);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
        return sceneName;
    }
#endif

    public void CarregarCena(Timeline timeline)
    {
        string nomeCena = timeline switch
        {
            Timeline.Presente => cenaPresente,
            Timeline.Passado => cenaPassado,
            Timeline.Futuro => cenaFuturo,
            _ => null
        };

        if (!string.IsNullOrEmpty(nomeCena))
            StartCoroutine(LoadSceneWithFade(nomeCena));
        else
            Debug.LogWarning($"Nenhuma cena atribuída para {timeline}");
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;

        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeOut();

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null && playerHealth.currentHealth > 1)
            playerHealth.ChangeHealth(-1);

        Time.timeScale = 1f;
        yield return SceneManager.LoadSceneAsync(sceneName);

        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeIn();

        playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = true;
    }
}

