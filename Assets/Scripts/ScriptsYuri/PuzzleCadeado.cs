using TMPro;
using UnityEngine;

public class PuzzleCadeado : MonoBehaviour
{
    [SerializeField] GameObject cadeadoPanel;
    [SerializeField] TextMeshProUGUI senhaCadeado;

    string resposta = "1234";
    string tentativaJogador;

    int num1, num2, num3, num4 = 0;

    

    private void Start()
    {
        cadeadoPanel.SetActive(false);

        tentativaJogador = num1.ToString() + num2.ToString() + num3.ToString() + num4.ToString();
        senhaCadeado.SetText(tentativaJogador);
    }

    private void Update()
    {
        tentativaJogador = num1.ToString() + num2.ToString() + num3.ToString() + num4.ToString();
        senhaCadeado.SetText(tentativaJogador);
    }

    public void AumentaNum1()
    {
        num1++;
        if (num1 == 9)
        {
            num1 = 0;
        }
    }
    public void DiminuiNum1()
    {
        num1--;
        if (num1 == 0)
        {
            num1 = 9;
        }
    }
    public void AumentaNum2()
    {
        num2++;
        if (num2 == 9)
        {
            num2 = 0;
        }
    }
    public void DiminuiNum2()
    {
        num2--;
        if (num2 == 0)
        {
            num2 = 9;
        }
    }
    public void AumentaNum3()
    {
        num3++;
        if (num3 == 9)
        {
            num3 = 0;
        }
    }
    public void DiminuiNum3()
    {
        num3--;
        if (num3 == 0)
        {
            num3 = 9;
        }
    }
    public void AumentaNum4()
    {
        num4++;
        if (num4 == 9)
        {
            num4 = 0;
        }
    }
    public void DiminuiNum4()
    {
        num4--;
        if (num4 == 0)
        {
            num4 = 9;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                cadeadoPanel.SetActive(true);
            }
        }
    }
}
