using UnityEngine;

public class AvancaEtapaAoMorrer : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Se verdadeiro, só avança a história na primeira vez que o inimigo morre.")]
    public bool apenasUmaVez = true;

    private bool jaAtivado = false;

    [Header("Opções de Detecção")]
    [Tooltip("Se o inimigo for desativado (SetActive(false)) em vez de destruído, marque isso.")]
    public bool detectarDesativacao = false;

    private void OnDestroy()
    {
        if (!detectarDesativacao)
            TentarAvancar();
    }

    private void OnDisable()
    {
        if (detectarDesativacao)
            TentarAvancar();
    }

    private void TentarAvancar()
    {
        if (apenasUmaVez && jaAtivado)
            return;

        jaAtivado = true;

        if (StoryProgressManager.instance != null)
        {
            StoryProgressManager.instance.AvancarEtapa();
            Debug.Log($"?? Inimigo {name} derrotado — progresso da história avançado!");
        }
        else
        {
            Debug.LogWarning("?? Nenhum StoryProgressManager encontrado na cena!");
        }
    }
}
