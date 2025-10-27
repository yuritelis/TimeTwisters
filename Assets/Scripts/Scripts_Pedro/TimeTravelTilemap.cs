using UnityEngine;
using UnityEngine.Tilemaps;

public enum Timeline
{
    Presente,
    Passado,
    Futuro
}

public class TimeTravelTilemap : MonoBehaviour, IInteractable
{
    public Timeline CurrentTimeline => currentTimeline;
    [Header("Tilemaps Visuais")]
    public Tilemap Tilemap_Presente;
    public Tilemap Tilemap_Passado;
    public Tilemap Tilemap_Futuro;

    [Header("Tilemaps de Colisão")]
    public Tilemap Tilemap_Presente_Colisao;
    public Tilemap Tilemap_Passado_Colisao;
    public Tilemap Tilemap_Futuro_Colisao;

    private Timeline currentTimeline = Timeline.Presente;
    public TimelineUI timelineUI;


    void Start()
    {
        SetTimeline(currentTimeline);
    }

    public void Interact()
    {
        if (timelineUI != null)
        {
            timelineUI.Open(this);
        }
    }
    public void SetTimeline(Timeline timeline)
    {
        currentTimeline = timeline;

        Tilemap_Presente.gameObject.SetActive(timeline == Timeline.Presente);
        Tilemap_Passado.gameObject.SetActive(timeline == Timeline.Passado);
        Tilemap_Futuro.gameObject.SetActive(timeline == Timeline.Futuro);

        Tilemap_Presente_Colisao.gameObject.SetActive(timeline == Timeline.Presente);
        Tilemap_Passado_Colisao.gameObject.SetActive(timeline == Timeline.Passado);
        Tilemap_Futuro_Colisao.gameObject.SetActive(timeline == Timeline.Futuro);
    }

    private void CycleTimeline()
    {
        Timeline nextTimeline = (Timeline)(((int)currentTimeline + 1) % 3);
        SetTimeline(nextTimeline);
        Debug.Log("Mudou para: " + nextTimeline);
    }

    public bool CanInteract()
    {
        throw new System.NotImplementedException();
    }
}
