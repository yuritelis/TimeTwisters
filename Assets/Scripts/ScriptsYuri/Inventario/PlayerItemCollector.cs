using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventarioController inventarioController;
    [SerializeField] GameObject interactKey;
        Item item;
    private bool playerInRange;

    void Start()
    {
        inventarioController = FindFirstObjectByType<InventarioController>();
        item = FindFirstObjectByType<Item>();
    }

    void Update()
    {
        if (playerInRange)
        {
            interactKey.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (item != null)
                {
                    Debug.Log("Input apertado");
                    bool itemAdded = inventarioController.AddItem(gameObject);

                    if (itemAdded)
                    {
                        Debug.Log("Item adicionado");
                        Destroy(gameObject);
                    }
                }
            }
        }
        else
        {
            interactKey.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
