using UnityEngine;
using UnityEngine.SceneManagement;

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

    private bool playerInside = false;

    private void Update()
    {
        if (requireInput && playerInside && Input.GetKeyDown(interactKey))
            PerformTransition();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (!requireInput)
                PerformTransition();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
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
}
