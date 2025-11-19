using UnityEngine;

public class ItemCollection : MonoBehaviour
{
    private InventarioControl invControl;

    public bool playerInRange = false;

    void Start()
    {
        invControl = FindFirstObjectByType<InventarioControl>();
    }

    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                bool itemAdd = invControl.AddItem(gameObject);

                if (itemAdd)
                    Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            playerInRange = false;
    }
}
