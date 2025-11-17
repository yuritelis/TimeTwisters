using UnityEditor.Rendering;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    //[SerializeField] GameObject interactKey;

    private InventarioController inventarioController;
    private HotbarController hotbarController;
    Item item;

    public bool playerInRange;

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

                    if (inventarioController != null && !inventarioController.isInventarioCheio && hotbarController.isHotbarCheia)
                        itemAdded = inventarioController.AddItem(gameObject);

                    if (itemAdded)
                    {
                        Destroy(gameObject);
                    }

                    if (inventarioController.isInventarioCheio)
                        Debug.Log("Não dá pra pegar item, inventário cheio");
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
