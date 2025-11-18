using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public string Tag;

    public virtual void UseItem(Slot slot)
    {

        Item item = slot.itemAtual.GetComponent<Item>();

        if (!slot.slotVazio)
        {
            if (Tag.ToLower() == "arma")
            {
                Debug.Log($"Usou arma: {Name}");
            }

            if (Tag.ToLower() == "utilitario")
            {
                Debug.Log($"Usou item {Name}");
                Destroy(item.gameObject);
                slot.RemoverItem();
                slot.itemAtual = null;
            }
        }
    }
}
