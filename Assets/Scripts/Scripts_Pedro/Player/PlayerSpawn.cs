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

        if (currentScene == "DeathScreen") return;

        string spawnName = PlayerPrefs.GetString("SpawnPoint", "");

        if (Application.isEditor && SceneManager.GetActiveScene().buildIndex == 0 && !PlayerPrefs.HasKey("GameStarted"))
        {
            PlayerPrefs.DeleteKey("SpawnPoint");
            PlayerPrefs.SetInt("GameStarted", 1);
            return;
        }

        if (!string.IsNullOrEmpty(spawnName))
        {
            GameObject spawn = GameObject.Find(spawnName);

            if (spawn != null)
            {
                transform.position = spawn.transform.position;
            }
        }

        PlayerHealth hp = GetComponent<PlayerHealth>();

        if (hp != null && PlayerPrefs.GetInt("ReviveFromDeath", 0) == 1)
        {
            hp.ResetPlayer();
            PlayerPrefs.SetInt("ReviveFromDeath", 0);
        }
    }
}
