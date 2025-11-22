using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HotbarControl : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    private ItemDictionary itemDictionary;
    private ItemCollection itemCollect;

    private Key[] hotbarKeys;

    public int qtdSlot = 7; // teclas 1 -> 7 no teclado alfanumérico
    private int slotAtual = 0;
    private int slotAnterior = -1;
    private int slotIndex = 0;

    void Awake()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
        itemCollect = FindFirstObjectByType<ItemCollection>();

        hotbarKeys = new Key[qtdSlot];
        for (int i = 0; i < qtdSlot; i++)
            hotbarKeys[i] = i < 7 ? (Key)((int)Key.Digit1 + i) : Key.Digit7;
    }

    void Update()
    {
        for (int i = 0; i < qtdSlot; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                SelecionarSlot(i);
                //UsarItem(i);

                slotIndex = i;

                break;
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int novoSlot = slotAtual + (scroll > 0 ? 1 : -1);
            if (novoSlot < 0) novoSlot = 6;
            if (novoSlot > 6) novoSlot = 0;

            slotIndex = novoSlot;

            SelecionarSlot(novoSlot);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse1)) && !itemCollect.playerInRange)
        {
            UsarItem(slotIndex);
        }
    }

    void UsarItem(int index)
    {
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();

        if (slot == null || slot.slotVazio)
        {
            Debug.Log("Slot inválido ou slot vazio");
            return;
        }

        if (!slot.slotVazio)
        {
            Item item = slot.itemAtual.GetComponent<Item>();
            item.UseItem(slot);
            Debug.Log($"Usando item de ID {item.ID} e nome {item.Name}");
        }
    }

    void SelecionarSlot(int novoSlot)
    {
        Slot slot = hotbarPanel.transform.GetChild(novoSlot).GetComponent<Slot>();

        slotAnterior = slotAtual;
        slotAtual = novoSlot;

        if (slotAnterior >= 0)
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

    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();

        foreach (Transform slotTransform in hotbarPanel.transform)
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

    public void SetHotbarItems(List<InventorySaveData> inventarioSaveData)
    {
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < qtdSlot; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        foreach (InventorySaveData data in inventarioSaveData)
        {
            Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
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
