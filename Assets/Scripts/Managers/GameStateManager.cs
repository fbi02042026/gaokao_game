using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private GameSaveData currentSave;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void NewGame()
    {
        var playerState = new PlayerState();

        if (SaveManager.Instance != null)
        {
            var savedPlayer = SaveManager.Instance.GetCurrentPlayer();
            if (savedPlayer != null)
                playerState.gender = savedPlayer.gender;
        }

        currentSave = new GameSaveData
        {
            saveId = MiniGameUtility.GenerateId(),
            createdAt = MiniGameUtility.GetTimestampSeconds(),
            currentStage = "highschool",
            playerState = playerState
        };
        Debug.Log($"[GameState] 新游戏创建, 性别={playerState.gender}");
    }

    public void RestoreFromSave(GameSaveData save)
    {
        currentSave = save;
        Debug.Log($"[GameState] 恢复存档, 阶段={save.currentStage}, 性别={save.playerState?.gender}");

        if (save.inheritData != null && InheritEngine.Instance != null)
            InheritEngine.Instance.LoadData(save.inheritData);

        if (save.pastMemories != null && DejaVuEngine.Instance != null)
            DejaVuEngine.Instance.LoadMemories(save.pastMemories);

        GamePhase phase = save.currentStage switch
        {
            "highschool" => GamePhase.HighSchool,
            "college" => GamePhase.College,
            "life" => GamePhase.Life,
            "result" => GamePhase.Result,
            _ => GamePhase.Home
        };

        if (GameManager.Instance != null)
            GameManager.Instance.ChangePhase(phase);
    }

    public PlayerState GetPlayerState()
    {
        return currentSave?.playerState;
    }

    public void SetPlayerState(PlayerState state)
    {
        if (currentSave != null)
            currentSave.playerState = state;
    }

    public Gender GetPlayerGender()
    {
        if (currentSave?.playerState != null)
            return currentSave.playerState.gender;

        if (SaveManager.Instance != null)
        {
            var p = SaveManager.Instance.GetCurrentPlayer();
            if (p != null) return p.gender;
        }

        return Gender.Male;
    }

    public InheritData GetInheritData()
    {
        return currentSave?.inheritData;
    }

    public void SetStage(string stage)
    {
        if (currentSave != null)
            currentSave.currentStage = stage;
    }

    public GameSaveData GetCurrentSave()
    {
        return currentSave;
    }

    public void QuickSave()
    {
        if (currentSave != null)
            SaveManager.Instance?.Save(currentSave);
    }

    public void QuickLoad()
    {
        var save = SaveManager.Instance?.Load();
        if (save != null)
            RestoreFromSave(save);
    }
}