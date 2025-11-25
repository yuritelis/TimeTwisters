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
        if (CheckpointManager.instance != null && CheckpointManager.instance.HasCheckpoint())
        {
            transform.position = CheckpointManager.instance.GetCheckpointPosition();
            return;
        }

        string spawnName = PlayerPrefs.GetString("SpawnPoint", "");
        if (!string.IsNullOrEmpty(spawnName))
        {
            GameObject spawn = GameObject.Find(spawnName);
            if (spawn != null)
                transform.position = spawn.transform.position;
        }
    }
}
