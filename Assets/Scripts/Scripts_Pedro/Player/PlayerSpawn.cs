using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        TrySpawnPlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySpawnPlayer();
    }

    private void TrySpawnPlayer()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string spawnName = PlayerPrefs.GetString("SpawnPoint", "");

        // 🚫 1) Evita teleporte na primeira inicialização no editor (sem precisar tocar no resto do sistema)
        if (Application.isEditor && SceneManager.GetActiveScene().buildIndex == 0 && !PlayerPrefs.HasKey("GameStarted"))
        {
            PlayerPrefs.DeleteKey("SpawnPoint");
            PlayerPrefs.SetInt("GameStarted", 1);
            Debug.Log("[PlayerSpawn] Jogo iniciado no Editor — limpando spawn e mantendo posição original.");
            return;
        }

        // ✅ 2) Se houver um spawn salvo, aplica normalmente
        if (!string.IsNullOrEmpty(spawnName))
        {
            GameObject spawn = GameObject.Find(spawnName);

            if (spawn != null)
            {
                transform.position = spawn.transform.position;
                Debug.Log($"[PlayerSpawn] Player posicionado em '{spawnName}' na cena '{currentScene}'");
            }
            else
            {
                Debug.LogWarning($"[PlayerSpawn] Spawn '{spawnName}' não encontrado na cena '{currentScene}'.");
            }
        }
        else
        {
            Debug.Log($"[PlayerSpawn] Nenhum spawn salvo — mantendo posição original na cena '{currentScene}'.");
        }
    }
}
