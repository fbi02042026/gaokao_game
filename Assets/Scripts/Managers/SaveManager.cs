using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;

namespace GaokaoLife.Managers
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private PlayerState currentPlayer;
        private string saveFileName = "gaokao_save.json";
        private bool isAutoSaveEnabled = true;
        private float autoSaveInterval = 300f;
        private float lastAutoSaveTime;

        private List<string> saveSlots;
        private const int MAX_SAVE_SLOTS = 3;

        public event Action OnSaveCompleted;
        public event Action OnLoadCompleted;
        public event Action OnAutoSaveTriggered;

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

            saveSlots = new List<string>();
            lastAutoSaveTime = Time.time;

            Debug.Log("[SaveManager] 存档管理器初始化完成");
        }

        void Update()
        {
            if (isAutoSaveEnabled && currentPlayer != null)
            {
                if (Time.time - lastAutoSaveTime >= autoSaveInterval)
                {
                    AutoSave();
                    lastAutoSaveTime = Time.time;
                }
            }
        }

        public void CreateNewGame()
        {
            currentPlayer = new PlayerState();
            Debug.Log("[SaveManager] 创建新游戏");
        }

        public bool SaveGame(int slotIndex = 0)
        {
            if (currentPlayer == null)
            {
                Debug.LogWarning("[SaveManager] 没有玩家数据可保存");
                return false;
            }

            try
            {
                currentPlayer.lastPlayTime = DateTime.Now;

                string savePath = GetSavePath(slotIndex);
                string jsonData = JsonUtility.ToJson(currentPlayer);
                
                PlayerPrefs.SetString(savePath, jsonData);
                PlayerPrefs.Save();

                UpdateSaveSlots(slotIndex);

                OnSaveCompleted?.Invoke();
                Debug.Log($"[SaveManager] 保存成功: 槽位 {slotIndex}");
                
                SyncToCloud(slotIndex);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 保存失败: {e.Message}");
                return false;
            }
        }

        public bool LoadGame(int slotIndex = 0)
        {
            try
            {
                string savePath = GetSavePath(slotIndex);
                string jsonData = PlayerPrefs.GetString(savePath, "");

                if (string.IsNullOrEmpty(jsonData))
                {
                    Debug.LogWarning($"[SaveManager] 槽位 {slotIndex} 没有存档");
                    return false;
                }

                currentPlayer = JsonUtility.FromJson<PlayerState>(jsonData);

                OnLoadCompleted?.Invoke();
                Debug.Log($"[SaveManager] 加载成功: 槽位 {slotIndex}");
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 加载失败: {e.Message}");
                return false;
            }
        }

        public bool HasSaveData(int slotIndex = 0)
        {
            string savePath = GetSavePath(slotIndex);
            return PlayerPrefs.HasKey(savePath);
        }

        public bool DeleteSave(int slotIndex = 0)
        {
            try
            {
                string savePath = GetSavePath(slotIndex);
                PlayerPrefs.DeleteKey(savePath);
                PlayerPrefs.Save();

                RemoveFromSaveSlots(slotIndex);

                Debug.Log($"[SaveManager] 删除存档: 槽位 {slotIndex}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 删除存档失败: {e.Message}");
                return false;
            }
        }

        private void AutoSave()
        {
            if (SaveGame(0))
            {
                OnAutoSaveTriggered?.Invoke();
                Debug.Log("[SaveManager] 自动保存完成");
            }
        }

        public void EnableAutoSave(bool enable)
        {
            isAutoSaveEnabled = enable;
            Debug.Log($"[SaveManager] 自动保存: {(enable ? "启用" : "禁用")}");
        }

        public void SetAutoSaveInterval(float seconds)
        {
            autoSaveInterval = Mathf.Max(60f, seconds);
        }

        private void SyncToCloud(int slotIndex)
        {
            var platform = PlatformManager.Instance?.CurrentPlatform ?? PlatformType.Editor;
            
            if (!PlatformManager.Instance?.HasFeature("cloud_save") ?? true)
            {
                Debug.Log("[SaveManager] 云存档功能不可用");
                return;
            }

            Debug.Log($"[SaveManager] 同步到云端: 槽位 {slotIndex}");
        }

        private void UpdateSaveSlots(int slotIndex)
        {
            string slotKey = $"save_slot_{slotIndex}";
            if (!saveSlots.Contains(slotKey))
            {
                saveSlots.Add(slotKey);
            }
        }

        private void RemoveFromSaveSlots(int slotIndex)
        {
            string slotKey = $"save_slot_{slotIndex}";
            saveSlots.Remove(slotKey);
        }

        private string GetSavePath(int slotIndex)
        {
            return $"{saveFileName}_slot{slotIndex}";
        }

        public PlayerState GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public void SetCurrentPlayer(PlayerState player)
        {
            currentPlayer = player;
        }

        public List<SaveSlotInfo> GetAllSaveSlots()
        {
            var slots = new List<SaveSlotInfo>();
            
            for (int i = 0; i < MAX_SAVE_SLOTS; i++)
            {
                var info = new SaveSlotInfo
                {
                    slotIndex = i,
                    hasData = HasSaveData(i),
                    playTime = 0,
                    lastPlayTime = DateTime.MinValue
                };

                if (info.hasData)
                {
                    try
                    {
                        string savePath = GetSavePath(i);
                        string jsonData = PlayerPrefs.GetString(savePath, "");
                        var saveData = JsonUtility.FromJson<PlayerState>(jsonData);
                        
                        info.playTime = saveData.totalPlayMinutes;
                        info.lastPlayTime = saveData.lastPlayTime;
                        info.playerName = saveData.playerName;
                        info.currentPhase = saveData.currentPhase;
                    }
                    catch
                    {
                        info.hasData = false;
                    }
                }

                slots.Add(info);
            }

            return slots;
        }

        public string ExportSaveData()
        {
            if (currentPlayer == null) return "";
            return JsonUtility.ToJson(currentPlayer);
        }

        public bool ImportSaveData(string jsonData)
        {
            try
            {
                currentPlayer = JsonUtility.FromJson<PlayerState>(jsonData);
                Debug.Log("[SaveManager] 存档数据导入成功");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 存档数据导入失败: {e.Message}");
                return false;
            }
        }

        public void ClearAllSaves()
        {
            for (int i = 0; i < MAX_SAVE_SLOTS; i++)
            {
                DeleteSave(i);
            }
            
            currentPlayer = null;
            Debug.Log("[SaveManager] 所有存档已清除");
        }
    }

    [Serializable]
    public class SaveSlotInfo
    {
        public int slotIndex;
        public bool hasData;
        public int playTime;
        public DateTime lastPlayTime;
        public string playerName;
        public GamePhase currentPhase;
    }
}
