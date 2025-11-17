using UnityEngine;
using UnityEngine.EventSystems;

public class BracoVinilBehaviour : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    bool mousePress = false;
    public BracoVinilStats bracoVinilS;
    public GameObject vitrolaPanel;

    void Start()
    {
        bracoVinilS = gameObject.GetComponent<BracoVinilStats>();
    }

    void Update()
    {
        MexerBraco();
    }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    mousePress = true;
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    bracoVinilS.podeMexer = true;
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{

    //}

    void OnMouseDown()
    {
        mousePress = true;
    }

    void OnMouseOver()
    {
        bracoVinilS.podeMexer = true;
    }

    void OnMouseExit()
    {
        if (!bracoVinilS.movendo)
            bracoVinilS.podeMexer = false;
    }

    void OnMouseUp()
    {
        mousePress = false;

        gameObject.transform.position = bracoVinilS.posInicial;
    }

    void MexerBraco()
    {
        if (mousePress && bracoVinilS.podeMexer)
        {
            bracoVinilS.movendo = true;

            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;

            gameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(mouseX, mouseY));
        }
        else
            bracoVinilS.movendo = false;
    }
}
