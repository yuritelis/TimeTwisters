using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1.0f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();
        if (dropSlot == null)
        {
            GameObject item = eventData.pointerEnter;
            if (item != null)
            {
                dropSlot = item.GetComponentInParent<Slot>();
            }
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot == null)
        {
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return;
        }
        if (dropSlot != null)
        {
            if (dropSlot.itemAtual != null)
            {
                GameObject otherItem = dropSlot.itemAtual;

                transform.SetParent(dropSlot.transform);
                dropSlot.itemAtual = gameObject;

                otherItem.transform.SetParent(originalSlot.transform);
                originalSlot.itemAtual = otherItem;

                otherItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                //dropSlot.itemAtual.transform.SetParent(originalSlot.transform);
                //originalSlot.itemAtual = dropSlot.itemAtual;
                //dropSlot.itemAtual.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.itemAtual = null;

                transform.SetParent(dropSlot.transform);
                dropSlot.itemAtual = gameObject;
            }
        }
        else
        {
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}