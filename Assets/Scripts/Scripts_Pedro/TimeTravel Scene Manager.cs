using UnityEngine;
using UnityEngine.SceneManagement;
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
#if UNITY_EDITOR
        cenaPresente = ObterNomeCena(cenaPresenteAsset);
        cenaPassado = ObterNomeCena(cenaPassadoAsset);
        cenaFuturo = ObterNomeCena(cenaFuturoAsset);
#else
        // No build, salva o nome manualmente
        cenaPresente = cenaPresenteAsset != null ? cenaPresenteAsset.name : "";
        cenaPassado = cenaPassadoAsset != null ? cenaPassadoAsset.name : "";
        cenaFuturo = cenaFuturoAsset != null ? cenaFuturoAsset.name : "";
#endif
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
        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeOut();

        Time.timeScale = 1f;
        yield return SceneManager.LoadSceneAsync(sceneName);

        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeIn();
    }
}
