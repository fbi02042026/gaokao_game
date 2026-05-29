using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public string saveId;
    public long createdAt;
    public long updatedAt;
    public string currentStage;
    public PlayerState playerState;
    public InheritData inheritData;
    public List<PlaythroughMemory> pastMemories;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private const string SAVE_KEY = "gk_save";

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    private GameSaveData currentData;

    public void Save(GameSaveData data)
    {
        data.updatedAt = MiniGameUtility.GetTimestampSeconds();
        currentData = data;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("[SaveManager] 存档成功");
    }

    public GameSaveData Load()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");
        if (string.IsNullOrEmpty(json)) return null;
        currentData = JsonUtility.FromJson<GameSaveData>(json);
        return currentData;
    }

    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public bool HasSave() { return HasSaveData(); }

    public void CreateNewGame()
    {
        currentData = new GameSaveData
        {
            saveId = MiniGameUtility.GenerateId(),
            createdAt = MiniGameUtility.GetTimestampSeconds(),
            currentStage = "home",
            playerState = new PlayerState()
        };
    }

    public PlayerState GetCurrentPlayer() { return currentData?.playerState; }

    public void SetCurrentPlayer(PlayerState state)
    {
        if (currentData == null) CreateNewGame();
        if (currentData != null) currentData.playerState = state;
    }

    public bool LoadGame() { return Load() != null; }

    public void SaveGame(int slotIndex = 0)
    {
        if (currentData != null) Save(currentData);
    }

    public void Delete()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }
}