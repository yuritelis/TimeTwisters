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
        Presente.gameObject.SetActive(false);
        Passado.gameObject.SetActive(false);
        Futuro.gameObject.SetActive(false);

        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>();
        }

        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        Presente.onClick.AddListener(() => ChooseTimeline(Timeline.Presente));
        Passado.onClick.AddListener(() => ChooseTimeline(Timeline.Passado));
        Futuro.onClick.AddListener(() => ChooseTimeline(Timeline.Futuro));
    }

    public void Open(TimeTravelTilemap timeObject)
    {
        currentTimeObject = timeObject;

        panel.SetActive(true);
        Presente.gameObject.SetActive(true);
        Passado.gameObject.SetActive(true);
        Futuro.gameObject.SetActive(true);

        UpdateButtonStates();
        Time.timeScale = 0f;
        TimelineUI.isPaused = true;
        isPaused = true;
        if (playerInput != null)
            playerInput.enabled = false;
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
        isPaused = false;

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
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
