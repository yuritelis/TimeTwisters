using TMPro;
using UnityEngine;

public class PuzzleCadeado : MonoBehaviour
{
    [SerializeField] GameObject cadeadoPanel;
    [SerializeField] GameObject carta;
    [SerializeField] TextMeshProUGUI senhaCadeado;

    string resposta = "1234";
    string tentativaJogador;

    int num1, num2, num3, num4;

    bool cadeadoActive, playerPerto = false;

    private void Start()
    {
        cadeadoPanel.SetActive(false);
        carta.SetActive(false);

        num1 = 0;
        num2 = 0;
        num3 = 0;
        num4 = 0;

        tentativaJogador = num1.ToString() + num2.ToString() + num3.ToString() + num4.ToString();
        senhaCadeado.SetText(tentativaJogador);
    }

    private void Update()
    {
        tentativaJogador = num1.ToString() + num2.ToString() + num3.ToString() + num4.ToString();
        senhaCadeado.SetText(tentativaJogador);

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

        if(tentativaJogador == resposta)
        {
            carta.SetActive(true);
            Destroy(gameObject);
            Destroy(cadeadoPanel);
        }
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

    void Open()
    {
        cadeadoPanel.SetActive(true);
        cadeadoActive = true;
    }

    public void Close()
    {
        cadeadoPanel.SetActive(false);
        cadeadoActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = true;
        }
    }
}
