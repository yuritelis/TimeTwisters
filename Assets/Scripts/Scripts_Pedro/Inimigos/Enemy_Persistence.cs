using UnityEngine;

public class EnemyPersistence : MonoBehaviour
{
    [Tooltip("ID único do inimigo (se vazio, será o nome do GameObject).")]
    public string enemyID;

    private string saveKey;

    private void Awake()
    {
        if (string.IsNullOrEmpty(enemyID))
            enemyID = gameObject.name;

        saveKey = "Enemy_" + gameObject.scene.name + "_" + enemyID;

        if (PlayerPrefs.GetInt(saveKey, 0) == 1)
        {
            gameObject.SetActive(false);
            Debug.Log($"☠️ {enemyID} já estava morto, removido da cena.");
        }
    }

    public void MarkAsDead()
    {
        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();
        Debug.Log($"💀 {enemyID} marcado como morto permanentemente.");
    }

    public void ResetDeathState()
    {
        PlayerPrefs.DeleteKey(saveKey);
        Debug.Log($"♻️ Estado de morte resetado para {enemyID}.");
    }
}
