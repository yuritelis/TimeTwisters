using UnityEngine;
using TMPro;

public class StoryProgressDebugUI : MonoBehaviour
{
    [Header("Referência")]
    public TextMeshProUGUI textoEtapa;

    [Header("Configuração")]
    public KeyCode teclaAvancar = KeyCode.F9;

    private void Awake()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
        if (textoEtapa == null)
        {
            Debug.LogError("⚠️ Nenhum TextMeshProUGUI atribuído ao StoryProgressDebugUI!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (StoryProgressManager.instance != null)
        {
            int etapa = StoryProgressManager.instance.historiaEtapaAtual;
            textoEtapa.text = $"Etapa: <b>{etapa}</b>";
        }
        else
        {
            textoEtapa.text = "Etapa: <b>?</b>";
        }

        if (Input.GetKeyDown(teclaAvancar))
        {
            if (StoryProgressManager.instance != null)
            {
                StoryProgressManager.instance.AvancarEtapa();
            }
        }
    }
}
