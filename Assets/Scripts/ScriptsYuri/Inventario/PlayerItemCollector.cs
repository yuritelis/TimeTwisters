using UnityEditor.Rendering;
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

                    Debug.Log("=== INICIANDO TENTATIVA DE ADICIONAR ITEM ===");

                    // 1. Tenta na Hotbar primeiro
                    if (hotbarController != null)
                    {
                        Debug.Log("1. Tentando adicionar na HOTBAR...");
                        itemAdded = hotbarController.AddItem(gameObject);
                        Debug.Log($"Resultado Hotbar: {itemAdded}");

                        if (!itemAdded)
                        {
                            Debug.Log("Hotbar retornou FALSE - Possivelmente cheia");
                        }
                    }
                    else
                    {
                        Debug.LogError("HotbarController não encontrado!");
                    }

                    // 2. Se não conseguiu na Hotbar, tenta Inventário
                    if (!itemAdded && inventarioController != null)
                    {
                        Debug.Log("2. Tentando adicionar no INVENTÁRIO...");
                        itemAdded = inventarioController.AddItem(gameObject);
                        Debug.Log($"Resultado Inventário: {itemAdded}");

                        if (!itemAdded)
                        {
                            Debug.Log("Inventário também retornou FALSE - Ambos cheios?");
                        }
                    }
                    else if (!itemAdded)
                    {
                        Debug.Log("Pulou inventário porque: " + (inventarioController == null ? "InventarioController é null" : "Item já foi adicionado"));
                    }

                    // 3. Processa resultado final
                    if (itemAdded)
                    {
                        Debug.Log("Item adicionado com SUCESSO - Destruindo objeto...");
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("FALHA: Item não foi adicionado em nenhum lugar!");
                    }

                    Debug.Log("=== FIM DA TENTATIVA ===");
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
