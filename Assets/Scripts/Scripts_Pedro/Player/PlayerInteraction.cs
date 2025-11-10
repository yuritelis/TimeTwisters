using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private IInteractable nearbyInteractable;
    private TimeTravelMarker nearbyTimeTravel;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (nearbyTimeTravel != null)
            {
                if (nearbyTimeTravel.timelineUI == null)
                    nearbyTimeTravel.timelineUI = FindFirstObjectByType<TimelineUI>();

                if (nearbyTimeTravel.timelineUI != null)
                    nearbyTimeTravel.timelineUI.Open(null);

                return;
            }

            if (nearbyInteractable != null)
            {
                nearbyInteractable.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            nearbyInteractable = interactable;

        TimeTravelMarker timeTravel = other.GetComponent<TimeTravelMarker>();
        if (timeTravel != null)
            nearbyTimeTravel = timeTravel;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable == nearbyInteractable)
            nearbyInteractable = null;

        TimeTravelMarker timeTravel = other.GetComponent<TimeTravelMarker>();
        if (timeTravel == nearbyTimeTravel)
            nearbyTimeTravel = null;
    }
}
