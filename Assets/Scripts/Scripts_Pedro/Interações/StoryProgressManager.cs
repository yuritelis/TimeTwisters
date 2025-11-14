using UnityEngine;

public class StoryProgressManager : MonoBehaviour
{
    public static StoryProgressManager instance;

    [Header("Progresso da História")]
    public int historiaEtapaAtual = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            CarregarProgresso();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AvancarEtapa()
    {
        historiaEtapaAtual++;
        PlayerPrefs.SetInt("HistoriaEtapa", historiaEtapaAtual);
        PlayerPrefs.Save();

        Debug.Log($"📖 Progresso avançado automaticamente para Etapa {historiaEtapaAtual}");
    }

    public void DefinirEtapa(int novaEtapa)
    {
        historiaEtapaAtual = Mathf.Max(historiaEtapaAtual, novaEtapa);
        PlayerPrefs.SetInt("HistoriaEtapa", historiaEtapaAtual);
        PlayerPrefs.Save();

        Debug.Log($"📚 Progresso definido manualmente para Etapa {historiaEtapaAtual}");
    }

    public void CarregarProgresso()
    {
        historiaEtapaAtual = PlayerPrefs.GetInt("HistoriaEtapa", 0);
        Debug.Log($"🔁 Progresso carregado: Etapa {historiaEtapaAtual}");
    }
}
