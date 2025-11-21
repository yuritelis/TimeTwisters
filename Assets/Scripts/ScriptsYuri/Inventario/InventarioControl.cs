using NUnit.Framework;
using System.Collections.Generic;
//using UnityEditor.Tilemaps;
using UnityEngine;

public class InventarioControl : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventarioPanel;
    public GameObject slotPrefab;
    public GameObject[] itemPrefabs;

    public int qtdSlot;

    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();

        //for (int i = 0; i < qtdSlot; i++)
        //{
        //    Slot slot = Instantiate(slotPrefab, inventarioPanel.transform).GetComponent<Slot>();

        //    if (i < itemPrefabs.Length)
        //    {
        //        GameObject item = Instantiate(itemPrefabs[i], slot.transform);
        //        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        //        slot.itemAtual = item;
        //    }
        //}
    }

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventarioPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.itemAtual == null)
            {
                GameObject novoItem = Instantiate(itemPrefab, slot.transform);
                novoItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.itemAtual = novoItem;
                return true;
            }
        }

        Debug.Log("Inventário cheio!");
        return false;
    }

    public List<InventorySaveData> GetInventarioItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();

        foreach (Transform slotTransform in inventarioPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.itemAtual != null)
            {
                Item item = slot.itemAtual.GetComponent<Item>();
                invData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });
            }
        }
        return invData;
    }

    public void SetInventarioItems(List<InventorySaveData> inventarioSaveData)
    {
        Debug.Log("chamou SetInventarioItems");

        foreach (Transform child in inventarioPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < qtdSlot; i++)
        {
            Instantiate(slotPrefab, inventarioPanel.transform);
        }

        foreach (InventorySaveData data in inventarioSaveData)
        {
            Slot slot = inventarioPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
            GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

            if (itemPrefab != null)
            {
                GameObject item = Instantiate(itemPrefab, slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.itemAtual = item;
            }
        }
    }
}
