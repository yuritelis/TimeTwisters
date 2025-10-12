using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    [SerializeField] public GameObject instrucaoUI;
    [SerializeField] public GameObject contGarrafasUI;
    [SerializeField] public GameObject garrafasObject;
    [SerializeField] public GameObject papel;
    [SerializeField] public GameObject livro;
    [SerializeField] public GameObject papelUI;
    [SerializeField] public TextMeshProUGUI contGarrafaTxt;

    int contGarrafas;

    public KeyCode interactKey = KeyCode.E;

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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("livro") && Input.GetKey(KeyCode.E))
        {
            instrucaoUI.SetActive(true);
            garrafasObject.SetActive(true);
            contGarrafasUI.SetActive(true);
            Destroy(livro);
        }

        if (collision.gameObject.CompareTag("garrafa") && Input.GetKey(KeyCode.E))
        {
            Destroy(collision.gameObject);
            PegaGarrafas();
        }

        if (collision.CompareTag("papel") && Input.GetKey(KeyCode.E))
        {
            papelUI.SetActive(true);
            Destroy(papel);
        }
    }

    void PegaGarrafas()
    {
        contGarrafas++;
        contGarrafaTxt.text = contGarrafas.ToString();
    }

    public void VoltarIntruc()
    {
        Debug.Log("VoltarInstruc chamado");
        instrucaoUI.SetActive(false);
    }

    public void VoltarPapel()
    {
        Debug.Log("VoltarPapel chamado");
        papelUI.SetActive(false);
    }
}
