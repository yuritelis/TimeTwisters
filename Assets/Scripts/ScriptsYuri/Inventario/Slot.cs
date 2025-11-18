using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject itemAtual;
    public bool slotVazio => itemAtual == null;

    public void RemoverItem()
    {
        if (!slotVazio)
        {
            Destroy(itemAtual.gameObject);
            itemAtual = null;
        }
    }
}
