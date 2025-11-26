using TMPro;
using UnityEngine;
using System.Collections;

public class PuzzleCadeado : MonoBehaviour
{
    [SerializeField] GameObject cadeadoPanel;
    [SerializeField] GameObject hotbarPanel, sanidadeBar, dialogoPanel;
    [SerializeField] TextMeshProUGUI cadeadoNum1, cadeadoNum2, cadeadoNum3, cadeadoNum4;

    string resposta = "1879";
    string tentativaJogador;

    int num1, num2, num3, num4;

    public static bool cadeadoActive;
    public bool respostaCorreta, playerPerto = false;

    private void Start()
    {
        cadeadoPanel.SetActive(false);
        dialogoPanel.SetActive(false);

        num1 = 0;
        num2 = 0;
        num3 = 0;
        num4 = 0;

        tentativaJogador = num1.ToString() + num2.ToString() + num3.ToString() + num4.ToString();
    }

    private void Update()
    {
        // Revalidar referências que podem ter perdido na troca de cena
        if (sanidadeBar == null)
            sanidadeBar = GameObject.Find("SanidadeBar");

        if (dialogoPanel == null)
            dialogoPanel = GameObject.Find("DialogoPanel");

        if (cadeadoPanel == null) return; // evita erro fatal

        tentativaJogador = num1.ToString() + num2.ToString() + num3.ToString() + num4.ToString();

        cadeadoNum1.SetText(num1.ToString());
        cadeadoNum2.SetText(num2.ToString());
        cadeadoNum3.SetText(num3.ToString());
        cadeadoNum4.SetText(num4.ToString());

        TestarResposta();

        if (playerPerto)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                Open();
            }
        }
        if (cadeadoActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = true;
        }
    }

    public void TestarResposta()
    {
        if (tentativaJogador == resposta)
        {
            if (sanidadeBar != null)
                sanidadeBar.SetActive(true);
            //hotbarPanel.SetActive(true);
            if (!respostaCorreta)
            {
                respostaCorreta = true;
                cadeadoActive = false;
                PauseController.SetPause(false);
                StartCoroutine(FadeOutCadeado());

                this.enabled = false;
            }
        }
    }

    void Open()
    {
        if (cadeadoPanel != null)
        {
            PauseController.SetPause(true);

            cadeadoPanel.SetActive(true);
            if (sanidadeBar != null)
                sanidadeBar.SetActive(false);
            if (dialogoPanel != null)
                dialogoPanel.SetActive(false);
            //hotbarPanel.SetActive(false);

            cadeadoActive = true;
        }
    }

    public void Close()
    {
        PauseController.SetPause(false);

        if (cadeadoPanel != null)
            cadeadoPanel.SetActive(false);
        if (sanidadeBar != null)
            sanidadeBar.SetActive(true);
        //hotbarPanel.SetActive(true);

        cadeadoActive = false;
    }

    public void AumentaNum1()
    {
        num1++;
        if (num1 == 10)
        {
            num1 = 0;
        }
    }
    public void DiminuiNum1()
    {
        num1--;
        if (num1 == -1)
        {
            num1 = 9;
        }
    }
    public void AumentaNum2()
    {
        num2++;
        if (num2 == 10)
        {
            num2 = 0;
        }
    }
    public void DiminuiNum2()
    {
        num2--;
        if (num2 == -1)
        {
            num2 = 9;
        }
    }
    public void AumentaNum3()
    {
        num3++;
        if (num3 == 10)
        {
            num3 = 0;
        }
    }
    public void DiminuiNum3()
    {
        num3--;
        if (num3 == -1)
        {
            num3 = 9;
        }
    }
    public void AumentaNum4()
    {
        num4++;
        if (num4 == 10)
        {
            num4 = 0;
        }
    }
    public void DiminuiNum4()
    {
        num4--;
        if (num4 == -1)
        {
            num4 = 9;
        }
    }

    private IEnumerator FadeOutCadeado()
    {
        CanvasGroup cg = cadeadoPanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = cadeadoPanel.AddComponent<CanvasGroup>();

        float t = 1f;

        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * 2f;
            cg.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        if (cadeadoPanel != null)
            cadeadoPanel.SetActive(false);
    }
}
