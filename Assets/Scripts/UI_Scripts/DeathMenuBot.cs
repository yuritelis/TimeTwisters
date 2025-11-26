using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuBot : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Transform spawnPoint;
    public string spawnPointName;

    public void BotMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void BotReiniciar()
    {
        PlayerPrefs.SetInt("ReviveFromDeath", 1);

        if (!string.IsNullOrEmpty(spawnPointName))
            PlayerPrefs.SetString("SpawnPoint", spawnPointName);

        SceneManager.LoadScene("Sala_Convidados");
    }
}
