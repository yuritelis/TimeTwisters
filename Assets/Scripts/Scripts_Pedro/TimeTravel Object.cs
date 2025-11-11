using UnityEngine;

public class TimeTravelMarker : MonoBehaviour, IInteractable
{
    private TimelineUI timelineUI;

    private void Awake()
    {
        timelineUI = FindFirstObjectByType<TimelineUI>();
    }

    public void Interact()
    {
        if (TimelineUI.instance != null)
        {
            TimelineUI.instance.Open(null);
        }
        else
        {
            Debug.LogError("❌ Nenhum TimelineUI ativo encontrado (instance é null)!");
        }
    }

    public bool CanInteract() => true;
}
