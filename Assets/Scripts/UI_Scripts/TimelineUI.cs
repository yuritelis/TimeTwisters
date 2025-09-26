using UnityEngine;
using UnityEngine.InputSystem;
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
    public PlayerController playerController;
    public static bool isPaused = false;
    public PlayerInput playerInput;


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
        Time.timeScale = 0f;
        TimelineUI.isPaused = true;
        playerInput.enabled = false;
        isPaused = true;
    }

    private void ChooseTimeline(Timeline timeline)
    {
        if (currentTimeObject != null)
        {
            currentTimeObject.SetTimeline(timeline);
        }
        panel.SetActive(false);
        Time.timeScale = 1f;
        TimelineUI.isPaused = false;
        playerInput.enabled = true;
        isPaused = false;
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
