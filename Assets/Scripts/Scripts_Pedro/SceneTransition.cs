using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Configuração da Transição")]
    [Tooltip("Nome exato da cena para carregar (deve estar adicionada no Build Settings)")]
    public string sceneToLoad;

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
            if (!requireInput) PerformTransition();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void PerformTransition()
    {
        if (!string.IsNullOrEmpty(spawnPointName))
            PlayerPrefs.SetString("SpawnPoint", spawnPointName);

        SceneManager.LoadScene(sceneToLoad);
    }
}
