using Unity.VisualScripting;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject currentItem;
    public bool isVazio => currentItem == null;

    public void RemoverItem()
    {
        if (!isVazio)
        {
            Destroy(currentItem);
            currentItem = null;
        }
    }
}
