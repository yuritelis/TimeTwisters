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

    [Header("Nomes usados na Build (auto-gerados)")]
    [SerializeField] private string cenaPresente;
    [SerializeField] private string cenaPassado;
    [SerializeField] private string cenaFuturo;

    public static TimeTravelSceneManager instance;

    private void Awake()
    {
        if (transform.parent != null)
        {
            Debug.LogWarning("[TimeTravelSceneManager] ⚠️ O objeto NÃO é raiz!");
            transform.SetParent(null);
        }

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

        if (!Application.isPlaying)
            EditorUtility.SetDirty(this);
#endif

        Debug.Log($"[TimeTravelSceneManager] Cenas carregadas: P:{cenaPresente}, Pa:{cenaPassado}, F:{cenaFuturo}");
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

        if (string.IsNullOrEmpty(nomeCena))
        {
            Debug.LogWarning($"[TimeTravelSceneManager] ⚠️ Nenhuma cena atribuída para {timeline}!");
            return;
        }

        StartCoroutine(LoadSceneWithFade(nomeCena));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        Debug.Log($"[TimeTravelSceneManager] 🌙 Carregando cena: {sceneName}");

        PlayerInput playerInput = FindSafe<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;

        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeOut();

        PlayerHealth playerHealth = FindSafe<PlayerHealth>();
        if (playerHealth != null && playerHealth.currentHealth > 1)
            playerHealth.ChangeHealth(-1);

        Time.timeScale = 1f;

        yield return SceneManager.LoadSceneAsync(sceneName);

        if (SceneFadeController.instance != null)
            yield return SceneFadeController.instance.FadeIn();

        playerInput = FindSafe<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = true;

        Debug.Log($"[TimeTravelSceneManager] ✅ Cena '{sceneName}' carregada com sucesso!");
    }
    private T FindSafe<T>() where T : Object
    {
        T obj = FindFirstObjectByType<T>();

        if (obj == null)
        {
            // Tenta fallback por tag
            if (typeof(T) == typeof(PlayerInput) || typeof(T) == typeof(PlayerHealth))
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null)
                {
                    obj = p.GetComponent<T>();
                }
            }
        }

        return obj;
    }
}
