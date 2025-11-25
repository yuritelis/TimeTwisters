using UnityEngine;
using UnityEngine.SceneManagement;


public class DeathMenuBot : MonoBehaviour
{
    public static bool pendingReset = false;

    public string salaPassado;
    public string salaPresente;
    public string salaFuturo;

    public void BotMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void BotReiniciar()
    {
        pendingReset = true;

        string epoca = PlayerPrefs.GetString("ultimaCena", "presente");

        if (epoca == "passado")
            SceneManager.LoadScene(salaPassado);
        else if (epoca == "futuro")
            SceneManager.LoadScene(salaFuturo);
        else
            SceneManager.LoadScene(salaPresente);
    }
}
