using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerState currentPlayer;
    private GamePhase currentPhase;
    private bool isGameRunning;
    private bool isPaused;

    public event Action<PlayerState> OnPlayerStateChanged;
    public event Action<GamePhase> OnPhaseChanged;
    public event Action OnGameStarted;
    public event Action OnGameEnded;

    public PlayerState CurrentPlayer => currentPlayer;
    public GamePhase CurrentPhase => currentPhase;
    public bool IsGameRunning => isGameRunning;
    public bool IsPaused => isPaused;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Initialize();
    }

    private void Initialize()
    {
        currentPhase = GamePhase.Login;
        isGameRunning = false;
        isPaused = false;
        Debug.Log("[GameManager] 游戏管理器初始化完成");
    }

    void Start()
    {
        CheckForSavedGame();
        ChangePhase(GamePhase.Login);
    }

    private void CheckForSavedGame()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSaveData())
        {
            Debug.Log("[GameManager] 发现存档，可以继续游戏");
        }
        else
        {
            Debug.Log("[GameManager] 没有存档，创建新游戏");
        }
    }

    public void StartNewGame()
    {
        Gender savedGender = Gender.Male;
        if (SaveManager.Instance != null)
        {
            var existing = SaveManager.Instance.GetCurrentPlayer();
            if (existing != null) savedGender = existing.gender;
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CreateNewGame();
            currentPlayer = SaveManager.Instance.GetCurrentPlayer();
            if (currentPlayer != null) currentPlayer.gender = savedGender;
        }
        else
        {
            currentPlayer = new PlayerState();
        }

        isGameRunning = true;
        currentPhase = GamePhase.Home;

        OnPlayerStateChanged?.Invoke(currentPlayer);
        OnPhaseChanged?.Invoke(currentPhase);
        OnGameStarted?.Invoke();

        Debug.Log("[GameManager] 开始新游戏");
        ChangePhase(GamePhase.Home);
    }

    public void ContinueGame()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.LoadGame())
        {
            currentPlayer = SaveManager.Instance.GetCurrentPlayer();
            isGameRunning = true;

            OnPlayerStateChanged?.Invoke(currentPlayer);
            OnPhaseChanged?.Invoke(currentPlayer.currentPhase);

            Debug.Log("[GameManager] 继续游戏");
            ChangePhase(currentPlayer.currentPhase);
        }
        else
        {
            Debug.LogWarning("[GameManager] 无法加载存档");
        }
    }

    public void ChangePhase(GamePhase newPhase)
    {
        if (currentPhase == newPhase && isGameRunning == false)
        {
            currentPhase = newPhase;
        }

        currentPhase = newPhase;
        if (currentPlayer != null)
            currentPlayer.currentPhase = newPhase;

        OnPhaseChanged?.Invoke(newPhase);
        Debug.Log($"[GameManager] 阶段切换: {newPhase}");

        if (UIManager.Instance != null)
            UIManager.Instance.ShowPanel(newPhase);
    }

    public void SaveGame(int slotIndex = 0)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("[GameManager] 没有玩家数据可保存");
            return;
        }

        currentPlayer.lastPlayTime = System.DateTime.UtcNow;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetCurrentPlayer(currentPlayer);
            SaveManager.Instance.SaveGame(slotIndex);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("[GameManager] 游戏暂停");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("[GameManager] 游戏继续");
    }

    public void EndGame()
    {
        isGameRunning = false;
        SaveGame();

        OnGameEnded?.Invoke();
        Debug.Log("[GameManager] 游戏结束");
    }

    public void QuitGame()
    {
        if (PlatformManager.Instance != null)
            PlatformManager.Instance.QuitGame();
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void UpdatePlayerState(Action<PlayerState> updateAction)
    {
        if (currentPlayer == null) return;

        updateAction?.Invoke(currentPlayer);
        OnPlayerStateChanged?.Invoke(currentPlayer);
    }

    public void AddDays(int days)
    {
        if (currentPlayer == null) return;

        currentPlayer.studyDays += days;

        if (currentPlayer.studyDays >= 1095 && currentPhase == GamePhase.HighSchool)
        {
            ChangePhase(GamePhase.Gaokao);
        }

        OnPlayerStateChanged?.Invoke(currentPlayer);
    }

    public int CalculateGaokaoScore()
    {
        if (ScoreEngine.Instance != null)
        {
            string talentId = TalentEngine.Instance?.GetCurrentTalent()?.id ?? "";
            return ScoreEngine.Instance.CalculateScore(currentPlayer, talentId);
        }

        return 400;
    }

    public void StartNewGamePlus()
    {
        Gender savedGender = Gender.Male;
        var existing = SaveManager.Instance?.GetCurrentPlayer();
        if (existing != null) savedGender = existing.gender;

        SaveManager.Instance?.CreateNewGame();
        currentPlayer = SaveManager.Instance?.GetCurrentPlayer() ?? new PlayerState();
        currentPlayer.gender = savedGender;
        currentPlayer.hasPastLifeMemory = true;

        isGameRunning = true;
        ChangePhase(GamePhase.TalentSelect);

        Debug.Log("[GameManager] 开始二周目");
    }
}