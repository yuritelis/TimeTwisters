using UnityEngine;

public class TimeTravelMarker : MonoBehaviour, IInteractable
{
    private TimelineUI timelineUI;

    [Header("Condição de Desbloqueio")]
    public int etapaNecessaria = 3;

    private void Awake()
    {
        timelineUI = FindFirstObjectByType<TimelineUI>();
    }

    public void Interact()
    {
        if (!CanInteract())
        {
            Debug.Log("🚫 O marcador de viagem no tempo ainda não pode ser usado.");
            return;
        }

        if (TimelineUI.instance != null)
        {
            TimelineUI.instance.Open(null);
        }
        else
        {
            Debug.LogError("❌ Nenhum TimelineUI ativo encontrado (instance é null)!");
        }
    }

    public bool CanInteract()
    {
        if (StoryProgressManager.instance == null)
            return true;

        return StoryProgressManager.instance.historiaEtapaAtual >= etapaNecessaria;
    }
}
