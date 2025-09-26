using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private IInteractable nearbyInteractable;

    void Update()
    {
        if (nearbyInteractable != null && Input.GetKeyDown(interactKey))
        {
            nearbyInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
            nearbyInteractable = interactable;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable == nearbyInteractable)
            nearbyInteractable = null;
    }
}
