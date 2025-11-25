using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    private Vector3 lastCheckpointPosition;
    private string lastSceneName = "";
    private int lastStoryProgress;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SaveCheckpoint(Vector3 position, string sceneName, int storyProgress)
    {
        lastCheckpointPosition = position;
        lastSceneName = sceneName;
        lastStoryProgress = storyProgress;
        hasCheckpoint = true;
    }

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }

    public Vector3 GetCheckpointPosition()
    {
        return lastCheckpointPosition;
    }

    public int GetStoryProgress()
    {
        return lastStoryProgress;
    }

    public string GetCheckpointScene()
    {
        return lastSceneName;
    }
}
