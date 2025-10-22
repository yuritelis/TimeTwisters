using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    [Header("> Sprites da Barra <")]
    [SerializeField] Sprite[] barras;

    [Header("> UI <")]
    [SerializeField] Image barraUI;

    public void SetVidaMax(int hpMax)
    {
        barraUI.sprite = barras[hpMax];
    }

    public void UpdateVidas(int hpAtual)
    {
        if (hpAtual >= 0 && hpAtual < barras.Length)
        {
            barraUI.sprite = barras[hpAtual];
        }
        else
        {
            Debug.LogWarning($"Vida {hpAtual} fora dos limites do array!");
        }
    }
}
