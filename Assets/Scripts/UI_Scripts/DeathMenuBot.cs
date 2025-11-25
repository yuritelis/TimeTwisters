using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathMenuBot : MonoBehaviour
{
    public void BotMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void BotReiniciar()
    {
        if (CheckpointManager.instance != null && CheckpointManager.instance.HasCheckpoint())
            SceneManager.LoadScene(CheckpointManager.instance.GetCheckpointScene());
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        StartCoroutine(ResetPlayerAfterLoad());
    }

    private IEnumerator ResetPlayerAfterLoad()
    {
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.ResetPlayer();

        if (CheckpointManager.instance != null && CheckpointManager.instance.HasCheckpoint())
            player.transform.position = CheckpointManager.instance.GetCheckpointPosition();

        if (StoryProgressManager.instance != null && CheckpointManager.instance != null)
            StoryProgressManager.instance.historiaEtapaAtual = CheckpointManager.instance.GetStoryProgress();
    }
}
