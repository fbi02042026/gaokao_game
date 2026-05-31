using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AchievementEngine : MonoBehaviour
{
    public static AchievementEngine Instance { get; private set; }

    private List<Achievement> allAchievements;
    private Dictionary<string, int> progressDict = new Dictionary<string, int>();
    private HashSet<string> unlockedSet = new HashSet<string>();
    private int totalUnlocked = 0;

    public System.Action<Achievement> OnAchievementUnlocked;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadFromResources();
        LoadUnlockStates();
    }

    void LoadFromResources()
    {
        var jsonAsset = Resources.Load<TextAsset>("Data/achievements");
        if (jsonAsset != null)
        {
            var wrapper = JsonUtility.FromJson<AchievementData>("{\"achievements\":" + jsonAsset.text + "}");
            allAchievements = wrapper?.achievements ?? new List<Achievement>();
            Debug.Log($"[AchievementEngine] 加载 {allAchievements.Count} 个成就");
        }
        else
        {
            allAchievements = new List<Achievement>();
        }
    }

    void LoadUnlockStates()
    {
        unlockedSet.Clear();
        foreach (var ach in allAchievements)
        {
            if (PlayerPrefs.GetInt($"ach_unlocked_{ach.id}", 0) == 1)
                unlockedSet.Add(ach.id);
        }
        totalUnlocked = unlockedSet.Count;
    }

    public void ReportProgress(string category, string statKey, int value)
    {
        foreach (var ach in allAchievements)
        {
            if (ach.category != category) continue;
            if (unlockedSet.Contains(ach.id)) continue;

            int current = 0;
            switch (statKey)
            {
                case "intellect":
                    current = value;
                    break;
                case "mental":
                    current = value;
                    break;
                case "social":
                    current = value;
                    break;
                case "health":
                    current = value;
                    break;
                default:
                    string progressKey = $"{category}_{statKey}";
                    if (!progressDict.ContainsKey(progressKey))
                        progressDict[progressKey] = 0;
                    current = progressDict[progressKey];
                    break;
            }

            if (current >= ach.totalRequired)
                UnlockAchievement(ach);
        }
    }

    public void IncrementProgress(string category, string statKey, int delta)
    {
        string key = $"{category}_{statKey}";
        if (!progressDict.ContainsKey(key))
            progressDict[key] = 0;
        progressDict[key] += delta;

        foreach (var ach in allAchievements)
        {
            if (ach.category != category) continue;
            if (unlockedSet.Contains(ach.id)) continue;
            if (progressDict[key] >= ach.totalRequired)
                UnlockAchievement(ach);
        }
    }

    public void CheckGaokaoScore(int score)
    {
        foreach (var ach in allAchievements)
        {
            if (unlockedSet.Contains(ach.id)) continue;
            if (ach.id == "ACH_STUDY_06" && score >= 700) UnlockAchievement(ach);
            if (ach.id == "ACH_STUDY_07" && score >= 650) UnlockAchievement(ach);
            if (ach.id == "ACH_STUDY_08" && score >= 680) UnlockAchievement(ach);
        }
    }

    public void CheckGraduationChoice(string choice)
    {
        foreach (var ach in allAchievements)
        {
            if (unlockedSet.Contains(ach.id)) continue;
            if (ach.id == "ACH_LIFE_01" && choice == "考研") UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_02" && choice == "就业") UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_03" && choice == "出国") UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_04" && choice == "创业") UnlockAchievement(ach);
        }
    }

    public void CheckLifeResult(int satisfactionScore, int careerLevel, int monthlyIncome, int mentalValue)
    {
        foreach (var ach in allAchievements)
        {
            if (unlockedSet.Contains(ach.id)) continue;
            if (ach.id == "ACH_LIFE_05" && satisfactionScore >= 70) UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_06" && satisfactionScore >= 90) UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_11" && careerLevel >= 5) UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_12" && careerLevel >= 8) UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_13" && monthlyIncome >= 10000) UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_14" && monthlyIncome >= 20000) UnlockAchievement(ach);
            if (ach.id == "ACH_LIFE_08" && mentalValue <= 30) UnlockAchievement(ach);
        }
    }

    void UnlockAchievement(Achievement ach)
    {
        if (unlockedSet.Contains(ach.id)) return;
        unlockedSet.Add(ach.id);
        totalUnlocked++;
        PlayerPrefs.SetInt($"ach_unlocked_{ach.id}", 1);
        PlayerPrefs.SetInt("ach_total_unlocked", totalUnlocked);
        PlayerPrefs.Save();

        Debug.Log($"[AchievementEngine] 解锁成就: {ach.name} — {ach.description}");
        OnAchievementUnlocked?.Invoke(ach);
    }

    public bool IsUnlocked(string achievementId)
    {
        return unlockedSet.Contains(achievementId);
    }

    public int GetUnlockedCount()
    {
        return totalUnlocked;
    }

    public int GetTotalCount()
    {
        return allAchievements?.Count ?? 0;
    }

    public List<Achievement> GetAllAchievements()
    {
        return allAchievements ?? new List<Achievement>();
    }

    public List<Achievement> GetUnlockedAchievements()
    {
        return allAchievements?.Where(a => unlockedSet.Contains(a.id)).ToList() ?? new List<Achievement>();
    }

    public List<Achievement> GetAchievementsByCategory(string category)
    {
        return allAchievements?.Where(a => a.category == category).ToList() ?? new List<Achievement>();
    }

    public int GetUnlockedCountByCategory(string category)
    {
        return allAchievements?.Count(a => a.category == category && unlockedSet.Contains(a.id)) ?? 0;
    }

    public int GetProgress(string achievementId)
    {
        var ach = allAchievements?.Find(a => a.id == achievementId);
        if (ach == null) return 0;

        string key = $"{ach.category}_{achievementId}";
        return progressDict.ContainsKey(key) ? Mathf.Min(progressDict[key], ach.totalRequired) : 0;
    }

    public void ResetAll()
    {
        foreach (var ach in allAchievements)
        {
            string key = $"ach_unlocked_{ach.id}";
            if (PlayerPrefs.HasKey(key)) PlayerPrefs.DeleteKey(key);
        }
        if (PlayerPrefs.HasKey("ach_total_unlocked")) PlayerPrefs.DeleteKey("ach_total_unlocked");
        PlayerPrefs.Save();
        unlockedSet.Clear();
        progressDict.Clear();
        totalUnlocked = 0;
    }
}