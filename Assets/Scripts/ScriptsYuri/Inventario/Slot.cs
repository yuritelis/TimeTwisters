using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject itemAtual;
    public bool slotVazio => itemAtual == null;
}
