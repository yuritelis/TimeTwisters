using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventarioController inventarioController;
    private HotbarController hotbarController;
    //[SerializeField] GameObject interactKey;
    Item item;
    private bool playerInRange;

    void Start()
    {
        if (inventarioController == null)
            inventarioController = FindFirstObjectByType<InventarioController>();

        if (hotbarController == null)
            hotbarController = FindFirstObjectByType<HotbarController>();

        item = FindFirstObjectByType<Item>();
    }

    void Update()
    {
        if (playerInRange)
        {
            //interactKey.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (item != null)
                {
                    bool itemAdded = false;

                    if (hotbarController == null) return;

                    if(hotbarController != null)
                        itemAdded = hotbarController.AddItem(gameObject);

                    if(!itemAdded && inventarioController != null)
                        itemAdded = inventarioController.AddItem(gameObject);

                    if (itemAdded)
                        Destroy(gameObject);
                    else
                        Debug.Log("hotbar cheia e inventario cheio");
                }
            }
        }
        else
        {
            //interactKey.SetActive(false);
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
