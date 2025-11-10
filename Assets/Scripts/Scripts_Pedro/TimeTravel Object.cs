using UnityEngine;

public class TimeTravelMarker : MonoBehaviour
{
    [Tooltip("Referência para a UI de viagem no tempo")]
    public TimelineUI timelineUI;

    private void Reset()
    {
        if (timelineUI == null)
            timelineUI = FindFirstObjectByType<TimelineUI>();
    }
}
