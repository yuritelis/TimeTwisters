using UnityEngine;

public class Progredir : MonoBehaviour, IInteractable
{
    [Header("Configuração")]
    [Tooltip("Se verdadeiro, só pode avançar uma vez (mesmo entre cenas).")]
    public bool apenasUmaVez = true;

    private bool jaAtivado = false;
    private string chaveSalvamento;

    private void Awake()
    {
        chaveSalvamento = "Progredir_" + gameObject.scene.name + "_" + gameObject.name;

        jaAtivado = PlayerPrefs.GetInt(chaveSalvamento, 0) == 1;
    }

    public void Interact()
    {
        if (apenasUmaVez && jaAtivado)
        {
            Debug.Log($"⚠️ {name} já foi usado antes, não avança progresso novamente.");
            return;
        }

        jaAtivado = true;
        PlayerPrefs.SetInt(chaveSalvamento, 1);
        PlayerPrefs.Save();

        if (StoryProgressManager.instance != null)
        {
            StoryProgressManager.instance.AvancarEtapa();
            Debug.Log($"🧩 Objeto {name} avançou o progresso da história!");
        }
        else
        {
            Debug.LogWarning("⚠️ Nenhum StoryProgressManager encontrado na cena!");
        }
    }

    public bool CanInteract()
    {
        return !apenasUmaVez || !jaAtivado;
    }
}
