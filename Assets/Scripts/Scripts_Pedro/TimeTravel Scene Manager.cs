using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TimeTravelSceneManager : MonoBehaviour
{
    [Header("Arraste as cenas aqui (somente no Editor)")]
    public Object cenaPresenteAsset;
    public Object cenaPassadoAsset;
    public Object cenaFuturoAsset;

    [Header("Nomes gerados automaticamente (usados na Build)")]
    [SerializeField] private string cenaPresente;
    [SerializeField] private string cenaPassado;
    [SerializeField] private string cenaFuturo;

    public static TimeTravelSceneManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            AtualizarNomesDasCenas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void AtualizarNomesDasCenas()
    {
#if UNITY_EDITOR
        cenaPresente = PegarNomeCena(cenaPresenteAsset);
        cenaPassado = PegarNomeCena(cenaPassadoAsset);
        cenaFuturo = PegarNomeCena(cenaFuturoAsset);

        EditorUtility.SetDirty(this);
#endif
    }

#if UNITY_EDITOR
    private string PegarNomeCena(Object sceneAsset)
    {
        if (sceneAsset == null) return "";

        string path = AssetDatabase.GetAssetPath(sceneAsset);
        return System.IO.Path.GetFileNameWithoutExtension(path);
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
        {
            StartCoroutine(LoadSceneWithFade(nomeCena));
        }
        else
        {
            Debug.LogWarning($"Nenhuma cena atribuída para {timeline}");
        }
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        PlayerInput playerInput = FindFirstObjectByName<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;

        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeOut();

        PlayerHealth playerHealth = FindFirstObjectByName<PlayerHealth>();
        if (playerHealth != null && playerHealth.currentHealth > 1)
            playerHealth.ChangeHealth(-1);

        Time.timeScale = 1f;
        yield return SceneManager.LoadSceneAsync(sceneName);

        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeIn();

        playerInput = FindFirstObjectByName<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = true;
    }

    private T FindFirstObjectByName<T>() where T : Object
    {
        return FindFirstObjectByType<T>();
    }
}
