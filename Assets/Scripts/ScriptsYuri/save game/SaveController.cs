using System.IO;
using UnityEngine;
using System.Collections;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventarioControl inventarioControl;
    private HotbarControl hotbarControl;

    private IEnumerator Start()
    {
        yield return null;

        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventarioControl = FindFirstObjectByType<InventarioControl>();
        hotbarControl = FindFirstObjectByType<HotbarControl>();

        LoadGame();
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            inventarioSaveData = inventarioControl.GetInventarioItems(),
            hotbarSaveData = hotbarControl.GetHotbarItems(),
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (!File.Exists(saveLocation))
        {
            SaveGame();
            return;
        }

        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.playerPosition;
        }

        inventarioControl.SetInventarioItems(saveData.inventarioSaveData);
        hotbarControl.SetHotbarItems(saveData.hotbarSaveData);
    }
}
