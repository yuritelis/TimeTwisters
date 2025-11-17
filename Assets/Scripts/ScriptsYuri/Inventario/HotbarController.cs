using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;

    public int slotCount = 10;
    public bool isHotbarCheia = false;
    public bool usarItem = false;

    public string nomeItem;

    private ItemDictionary itemDictionary;

    private Key[] hotbarKeys;

    private int slotAtual = 0;
    private int slotAnterior = -1;

    void Awake()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : (Key)(int)Key.Digit0;
        }
    }

    private void Start()
    {
        SelecionarSlot(0);
    }

    // Meu update
    void Update()
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                SelecionarSlot(i);
                break;
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int novoSlot = slotAtual + (scroll > 0 ? 1 : -1);
            if (novoSlot < 0) novoSlot = 9;
            if (novoSlot > 9) novoSlot = 0;

            SelecionarSlot(novoSlot);
        }

        //if (!itemCollector.playerInRange)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        UsarItem(slotAtual);
        //    }
        //}
    }

    void SelecionarSlot(int novoSlot)
    {
        slotAnterior = slotAtual;
        slotAtual = novoSlot;
        
        if(slotAnterior >= 0)
            SlotForaDeUso(slotAnterior);

        SlotEmUso(slotAtual);

        Debug.Log($"Slot alterado: {slotAnterior} -> {slotAtual}");
    }

    void SlotEmUso(int index)
    {
        if (index < 0 || index >= hotbarPanel.transform.childCount) return;

        Image slotImg = hotbarPanel.transform.GetChild(index).GetComponent<Image>();
        if (slotImg != null)
            slotImg.color = Color.gray;
    }

    void SlotForaDeUso(int index)
    {
        if (index < 0 || index >= hotbarPanel.transform.childCount) return;

        Image slotImg = hotbarPanel.transform.GetChild(index).GetComponent<Image>();
        if (slotImg != null)
            slotImg.color = Color.white;
    }

    void UsarItem(int index)
    {
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();

        if(slot == null || slot.isVazio)
        {
            Debug.Log("Slot inválido ou slot vazio");
            return;
        }

        if (!slot.isVazio)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            nomeItem = item.Name;
            item.UseItem(slot);
            slot.RemoverItem();
        }
    }

    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();

        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                hotbarData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });
            }
        }
        return hotbarData;
    }

    public void SetHotbarItems(List<InventorySaveData> hotbarSaveData)
    {
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        foreach (InventorySaveData data in hotbarSaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
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
