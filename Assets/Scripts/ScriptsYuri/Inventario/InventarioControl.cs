using UnityEditor.Tilemaps;
using UnityEngine;

public class InventarioControl : MonoBehaviour
{
    public GameObject inventarioPanel;
    public GameObject slotPrefab;
    public GameObject[] itemPrefabs;

    public int qtdSlot;

    void Start()
    {
        for (int i = 0; i < qtdSlot; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventarioPanel.transform).GetComponent<Slot>();

            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.itemAtual = item;
            }
        }
    }
}
