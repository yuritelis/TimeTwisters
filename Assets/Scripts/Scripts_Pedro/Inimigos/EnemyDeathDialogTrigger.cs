using UnityEngine;
using System.Collections;

public class EnemyDeathDialogTrigger : MonoBehaviour
{
    [Header("Diálogo que será reproduzido após o inimigo morrer.")]
    public Dialogo dialogoAoMorrer;

    [Tooltip("Executa apenas uma vez.")]
    public bool apenasUmaVez = true;

    [Header("Progresso ganho ao derrotar este inimigo")]
    [Tooltip("Quantidade de progresso. Padrão = 1")]
    public int progresso = 1; // ✅ valor padrão

    private string saveKey;
    private bool jaExecutou = false;

    private void Awake()
    {
        saveKey = "EnemyDeathDialog_" + gameObject.scene.name + "_" + gameObject.name;
        jaExecutou = PlayerPrefs.GetInt(saveKey, 0) == 1;
    }

    public void OnEnemyDefeated()
    {
        if (apenasUmaVez && jaExecutou)
            return;

        jaExecutou = true;

        if (apenasUmaVez)
        {
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();
        }

        int progressoAtual = PlayerPrefs.GetInt("ProgressoGlobal", 0);
        PlayerPrefs.SetInt("ProgressoGlobal", progressoAtual + progresso);
        PlayerPrefs.Save();

        GameObject tempObj = new GameObject("EnemyDeathDialogRunner");
        DontDestroyOnLoad(tempObj);

        tempObj.AddComponent<EnemyDeathDialogRunner>().Init(dialogoAoMorrer);
    }
}

public class EnemyDeathDialogRunner : MonoBehaviour
{
    private Dialogo dialogo;

    public void Init(Dialogo d)
    {
        dialogo = d;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        yield return null;

        DialogoManager.Instance.StartDialogo(dialogo);

        while (DialogoManager.Instance.dialogoAtivoPublico)
            yield return null;

        Destroy(gameObject);
    }
}
