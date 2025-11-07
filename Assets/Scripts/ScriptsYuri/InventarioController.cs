using UnityEngine;

public class InventarioController : MonoBehaviour
{
    public GameObject inventarioPanel;
    public GameObject slotPrefab;
    public GameObject[] itemPrefabs;
    public int slotCount;

    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventarioPanel.transform).GetComponent<Slot>();

            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                slot.currentItem = item;
            }
        }
    }
}
