using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GaokaoLife.Models;
using GaokaoLife.Engine;
using GaokaoLife.Managers;
using GaokaoLife.Data;

namespace GaokaoLife
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private PlayerState currentPlayer;
        private GamePhase currentPhase;
        private bool isGameRunning;
        private bool isPaused;

        private List<string> gameSceneNames = new List<string>
        {
            "Home",
            "Province",
            "HighSchool",
            "Event",
            "Gaokao",
            "Zhiyuan",
            "College",
            "Life",
            "Result"
        };

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
            currentPhase = GamePhase.Home;
            isGameRunning = false;
            isPaused = false;

            Debug.Log("[GameManager] 游戏管理器初始化完成");
        }

        void Start()
        {
            CheckForSavedGame();
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
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CreateNewGame();
                currentPlayer = SaveManager.Instance.GetCurrentPlayer();
            }
            else
            {
                currentPlayer = new PlayerState();
            }

            isGameRunning = true;
            currentPhase = GamePhase.Home;

            LoadGameData();

            OnPlayerStateChanged?.Invoke(currentPlayer);
            OnPhaseChanged?.Invoke(currentPhase);
            OnGameStarted?.Invoke();

            Debug.Log("[GameManager] 开始新游戏");

            LoadScene("Home");
        }

        public void ContinueGame()
        {
            if (SaveManager.Instance != null && SaveManager.Instance.LoadGame())
            {
                currentPlayer = SaveManager.Instance.GetCurrentPlayer();
                isGameRunning = true;

                LoadGameData();

                OnPlayerStateChanged?.Invoke(currentPlayer);
                OnPhaseChanged?.Invoke(currentPlayer.currentPhase);

                Debug.Log("[GameManager] 继续游戏");
                LoadScene(currentPlayer.currentPhase.ToString());
            }
            else
            {
                Debug.LogWarning("[GameManager] 无法加载存档");
            }
        }

        private void LoadGameData()
        {
            if (DataLoader.Instance != null)
            {
                DataLoader.Instance.LoadAllData(() =>
                {
                    Debug.Log("[GameManager] 游戏数据加载完成");
                });
            }
        }

        public void ChangePhase(GamePhase newPhase)
        {
            if (currentPhase == newPhase) return;

            currentPhase = newPhase;
            currentPlayer.currentPhase = newPhase;

            OnPhaseChanged?.Invoke(newPhase);

            Debug.Log($"[GameManager] 阶段切换: {newPhase}");

            LoadScene(newPhase.ToString());
        }

        public void LoadScene(string sceneName)
        {
            if (gameSceneNames.Contains(sceneName))
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log($"[GameManager] 加载场景: {sceneName}");
            }
            else
            {
                Debug.LogWarning($"[GameManager] 场景不存在: {sceneName}");
            }
        }

        public void SaveGame(int slotIndex = 0)
        {
            if (currentPlayer == null)
            {
                Debug.LogWarning("[GameManager] 没有玩家数据可保存");
                return;
            }

            currentPlayer.lastPlayTime = DateTime.Now;

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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void UpdatePlayerState(Action<PlayerState> updateAction)
        {
            if (currentPlayer == null) return;

            updateAction?.Invoke(currentPlayer);
            OnPlayerStateChanged?.Invoke(currentPlayer);

            if (TalentEngine.Instance != null)
            {
                TalentEngine.Instance.InitializePlayerTalents(currentPlayer);
            }

            if (DejaVuEngine.Instance != null && currentPlayer.hasPastLifeMemory)
            {
                DejaVuEngine.Instance.InitializePlayerMemories(currentPlayer);
            }
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
                return ScoreEngine.Instance.CalculateGaokaoScore(currentPlayer);
            }

            int total = 0;
            foreach (var score in currentPlayer.subjectScores.Values)
            {
                total += score;
            }
            return total;
        }

        public void StartNewGamePlus()
        {
            if (InheritEngine.Instance != null)
            {
                InheritEngine.Instance.PrepareInheritance(currentPlayer);
            }

            SaveManager.Instance?.CreateNewGame();
            currentPlayer = SaveManager.Instance.GetCurrentPlayer();

            if (InheritEngine.Instance != null)
            {
                InheritEngine.Instance.ApplyInheritance(currentPlayer);
            }

            isGameRunning = true;
            ChangePhase(GamePhase.HighSchool);

            Debug.Log("[GameManager] 开始二周目");
        }
    }
}
