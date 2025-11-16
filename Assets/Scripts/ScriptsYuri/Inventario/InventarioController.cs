using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class InventarioController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    private HotbarController hotbarController;

    public GameObject inventarioPanel;
    public GameObject slotPrefab;
    public GameObject[] itemPrefabs;

    public int slotCount;
    public bool isInventarioCheio = false;

    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();

        //for (int i = 0; i < slotCount; i++)
        //{
        //    Slot slot = Instantiate(slotPrefab, inventarioPanel.transform).GetComponent<Slot>();

        //    if (i < itemPrefabs.Length)
        //    {
        //        GameObject item = Instantiate(itemPrefabs[i], slot.transform);
        //        item.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        //        slot.currentItem = item;
        //    }
        //}
    }

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in inventarioPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.isVazio && hotbarController.isHotbarCheia)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }
        }

        Debug.Log("Inventário cheio!!!");
        isInventarioCheio = true;
        return false;
    }

    public List<InventorySaveData> GetInventarioItems()
    {
         List<InventorySaveData> invData = new List<InventorySaveData>();

        foreach (Transform slotTransform in inventarioPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });
            }
        }
        return invData;
    }

    public void SetInventarioItems(List<InventorySaveData> inventarioSaveData)
    {
        foreach (Transform child in inventarioPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventarioPanel.transform);
        }

        foreach (InventorySaveData data in inventarioSaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventarioPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
            }
        }
    }
}
