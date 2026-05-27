using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;
using GaokaoLife.Engine;
using GaokaoLife.Managers;

namespace GaokaoLife.Data
{
    public class DataLoader : MonoBehaviour
    {
        public static DataLoader Instance { get; private set; }

        private Dictionary<string, TextAsset> loadedAssets;
        private bool isDataLoaded;

        public event Action OnDataLoaded;
        public event Action<float> OnLoadProgress;

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

            loadedAssets = new Dictionary<string, TextAsset>();
            isDataLoaded = false;

            Debug.Log("[DataLoader] 数据加载器初始化完成");
        }

        public void LoadAllData(Action onComplete)
        {
            StartCoroutine(LoadAllDataCoroutine(onComplete));
        }

        private System.Collections.IEnumerator LoadAllDataCoroutine(Action onComplete)
        {
            float totalSteps = 10f;
            float currentStep = 0f;

            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            var talents = LoadData<TalentData>("talents");
            if (talents != null && TalentEngine.Instance != null)
            {
                TalentEngine.Instance.LoadTalents(talents);
            }
            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            var events = LoadEventsFromFolder("Data/Events");
            if (EventEngine.Instance != null)
            {
                EventEngine.Instance.LoadEvents(events);
            }
            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            var colleges = LoadData<CollegeData>("colleges");
            if (colleges != null && ScoreEngine.Instance != null)
            {
                ScoreEngine.Instance.LoadCollegeData(colleges);
            }
            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            var memories = LoadData<MemoryData>("memories");
            if (memories != null && DejaVuEngine.Instance != null)
            {
                DejaVuEngine.Instance.LoadMemories(memories);
            }
            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            var npcs = LoadData<NPCData>("npcs");
            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            var provinces = LoadData<ProvinceData>("provinces");
            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            var personalities = LoadData<PersonalityData>("personalities");
            if (personalities != null && PersonalityEngine.Instance != null)
            {
                PersonalityEngine.Instance.LoadPersonalities(personalities);
            }
            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            LoadMajors();
            OnLoadProgress?.Invoke(currentStep / totalSteps);
            yield return null;

            currentStep++;
            isDataLoaded = true;
            OnLoadProgress?.Invoke(1f);
            OnDataLoaded?.Invoke();

            Debug.Log("[DataLoader] 所有数据加载完成");
            onComplete?.Invoke();
        }

        public T LoadData<T>(string dataName) where T : new()
        {
            TextAsset asset = Resources.Load<TextAsset>($"Data/{dataName}");
            
            if (asset == null)
            {
                Debug.LogWarning($"[DataLoader] 找不到数据文件: {dataName}");
                return new T();
            }

            try
            {
                T data = JsonUtility.FromJson<T>(asset.text);
                Debug.Log($"[DataLoader] 加载数据: {dataName}");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataLoader] 解析数据失败 {dataName}: {e.Message}");
                return new T();
            }
        }

        private List<GameEvent> LoadEventsFromFolder(string folderPath)
        {
            var allEvents = new List<GameEvent>();
            TextAsset[] eventFiles = Resources.LoadAll<TextAsset>(folderPath);

            foreach (var file in eventFiles)
            {
                try
                {
                    var events = JsonUtility.FromJson<EventList>(file.text);
                    if (events != null && events.events != null)
                    {
                        allEvents.AddRange(events.events);
                        Debug.Log($"[DataLoader] 加载事件文件: {file.name}, 事件数: {events.events.Count}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[DataLoader] 解析事件文件失败 {file.name}: {e.Message}");
                }
            }

            return allEvents;
        }

        private void LoadMajors()
        {
            TextAsset asset = Resources.Load<TextAsset>("Data/majors");
            if (asset != null)
            {
                Debug.Log("[DataLoader] 加载专业数据");
            }
        }

        public bool IsDataLoaded()
        {
            return isDataLoaded;
        }

        public void ReloadData(string dataName)
        {
            Debug.Log($"[DataLoader] 重新加载数据: {dataName}");
        }

        public void ClearCache()
        {
            loadedAssets.Clear();
            Debug.Log("[DataLoader] 缓存已清除");
        }
    }

    [Serializable]
    public class EventList
    {
        public List<GameEvent> events;
    }

    public static class DataExtensions
    {
        public static T FromJson<T>(this string json) where T : new()
        {
            return JsonUtility.FromJson<T>(json);
        }

        public static string ToJson<T>(this T obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public static string ToJsonPretty<T>(this T obj)
        {
            return JsonUtility.ToJson(obj, true);
        }
    }
}
