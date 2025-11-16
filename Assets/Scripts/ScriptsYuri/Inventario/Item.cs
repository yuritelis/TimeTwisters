using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;

    public virtual void UseItem(Slot slot)
    {
        Debug.Log("Usou item" + Name);

        Item item = slot.currentItem.GetComponent<Item>();

        if (!slot.isVazio)
        {
            Destroy(item);
            slot.currentItem = null;
        }
    }
}
