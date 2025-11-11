using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogoEscolhaUI : MonoBehaviour
{
    public Button botaoSim;
    public Button botaoNao;

    private Action<bool> callback;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Mostrar(Action<bool> onEscolha)
    {
        callback = onEscolha;
        gameObject.SetActive(true);
    }

    public void EscolherSim()
    {
        callback?.Invoke(true);
        gameObject.SetActive(false);
    }

    public void EscolherNao()
    {
        callback?.Invoke(false);
        gameObject.SetActive(false);
    }
}
