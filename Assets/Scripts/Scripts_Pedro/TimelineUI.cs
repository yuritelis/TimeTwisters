using UnityEngine;
using UnityEngine.UI;

public class TimelineUI : MonoBehaviour
{
    public GameObject panel; // TimelinePanel
    public Button Presente;
    public Button Passado;
    public Button Futuro;

    private TimeTravelTilemap currentTimeObject;

    private Color normalColor = Color.white;
    private Color disabledColor = Color.gray;

    void Start()
    {
        panel.SetActive(false); // garante que comece desativado

        Presente.onClick.AddListener(() => ChooseTimeline(Timeline.Presente));
        Passado.onClick.AddListener(() => ChooseTimeline(Timeline.Passado));
        Futuro.onClick.AddListener(() => ChooseTimeline(Timeline.Futuro));
    }

    public void Open(TimeTravelTilemap timeObject)
    {
        currentTimeObject = timeObject;
        panel.SetActive(true);
        UpdateButtonStates(); // atualiza os botões quando abre
    }

    private void ChooseTimeline(Timeline timeline)
    {
        if (currentTimeObject != null)
        {
            currentTimeObject.SetTimeline(timeline);
        }
        panel.SetActive(false);
    }

    private void UpdateButtonStates()
    {
        // reseta todos para normal
        ResetButton(Presente);
        ResetButton(Passado);
        ResetButton(Futuro);

        // desativa o botão da timeline atual
        switch (currentTimeObject.CurrentTimeline)
        {
            case Timeline.Presente:
                DisableButton(Presente);
                break;
            case Timeline.Passado:
                DisableButton(Passado);
                break;
            case Timeline.Futuro:
                DisableButton(Futuro);
                break;
        }
    }

    private void ResetButton(Button btn)
    {
        btn.interactable = true;
        btn.GetComponent<Image>().color = normalColor;
    }

    private void DisableButton(Button btn)
    {
        btn.interactable = false;
        btn.GetComponent<Image>().color = disabledColor;
    }
}
