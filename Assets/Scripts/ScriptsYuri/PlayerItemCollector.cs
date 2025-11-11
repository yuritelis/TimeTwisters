using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventarioController invController;

    void Start()
    {
        invController = FindFirstObjectByType<InventarioController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         
    }
}
