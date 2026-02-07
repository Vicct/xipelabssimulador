using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    public static SaveManager Instance => instance;

    private string savePath;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public void SaveGame(GameState gameState)
    {
        try
        {
            string json = JsonUtility.ToJson(gameState, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Game saved to {savePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public GameState LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found");
            return null;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            GameState gameState = JsonUtility.FromJson<GameState>(json);
            Debug.Log("Game loaded successfully");
            return gameState;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }

    public bool HasSaveGame()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted");
        }
    }
}
