using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    [SerializeField] GameObject instrucaoUI;
    [SerializeField] GameObject contGarrafasUI;
    [SerializeField] GameObject garrafasObject;
    [SerializeField] GameObject[] garrafas;
    [SerializeField] GameObject papel;
    [SerializeField] GameObject papelUI;
    [SerializeField] TextMeshPro contGarrafaTxt;

    int contGarrafas;

    void Start()
    {
        papel.SetActive(false);
        garrafasObject.SetActive(false);
        papelUI.SetActive(false);
        instrucaoUI.SetActive(false);
        contGarrafasUI.SetActive(false);

        contGarrafaTxt.text = contGarrafas.ToString();
    }

    void Update()
    {
        if (contGarrafas >= 3)
        {
            papel.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("livro") && Input.GetKey(KeyCode.E))
        {
            instrucaoUI.SetActive(true);
            garrafasObject.SetActive(true);
            contGarrafasUI.SetActive(true);
        }

        if (collision.gameObject.CompareTag("garrafa") && Input.GetKey(KeyCode.E))
        {
            Destroy(collision.gameObject);
            PegaGarrafas();
        }

        if (collision.gameObject.CompareTag("papel") && Input.GetKey(KeyCode.E))
        {
            papelUI.SetActive(true);
        }
    }

    void PegaGarrafas()
    {
        contGarrafas++;
        contGarrafaTxt.text = contGarrafas.ToString();
    }

    public void Voltar()
    {
        papelUI.SetActive(false);
        instrucaoUI.SetActive(false);
    }
}
