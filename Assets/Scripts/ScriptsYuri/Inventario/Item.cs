using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;

    public virtual void UseItem(Slot slot)
    {
        Debug.Log("Usou item" + Name);

        Item item = slot.itemAtual.GetComponent<Item>();

        if (!slot.slotVazio)
        {
            Destroy(item.gameObject);
            slot.RemoverItem();
            slot.itemAtual = null;
        }
    }
}
