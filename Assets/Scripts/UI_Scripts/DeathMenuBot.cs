using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuBot : MonoBehaviour
{
    public void BotMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void BotReiniciar()
    {
        SceneManager.LoadScene("Sala_Convidados");
    }
}
