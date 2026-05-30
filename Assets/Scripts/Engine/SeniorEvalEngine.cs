using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SeniorEvalEngine : MonoBehaviour
{
    public static SeniorEvalEngine Instance { get; private set; }

    private List<SeniorEval> allEvals;
    private HashSet<string> unlockedEvalIds = new HashSet<string>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadFromResources();
        LoadUnlockStates();
    }

    void LoadFromResources()
    {
        var jsonAsset = Resources.Load<TextAsset>("Data/senior_evals");
        if (jsonAsset != null)
        {
            var wrapper = JsonUtility.FromJson<SeniorEvalData>("{\"evals\":" + jsonAsset.text + "}");
            allEvals = wrapper?.evals ?? new List<SeniorEval>();
            Debug.Log($"[SeniorEvalEngine] 从 Resources 加载 {allEvals.Count} 条评价");
        }
        else
        {
            Debug.LogWarning("[SeniorEvalEngine] Data/senior_evals.json 未找到");
            allEvals = new List<SeniorEval>();
        }
    }

    void LoadUnlockStates()
    {
        unlockedEvalIds.Clear();
        foreach (var eval in allEvals)
        {
            if (PlayerPrefs.GetInt($"eval_unlocked_{eval.evalId}", eval.adLocked ? 0 : 1) == 1)
                unlockedEvalIds.Add(eval.evalId);
        }
    }

    public List<SeniorEval> GetEvalsByMajor(string majorId)
    {
        if (allEvals == null) return new List<SeniorEval>();
        return allEvals.Where(e => e.majorId == majorId)
                       .OrderBy(e => e.priority)
                       .ToList();
    }

    public SeniorEval GetFreeTrial(string majorId)
    {
        return GetEvalsByMajor(majorId).FirstOrDefault(e => !e.adLocked);
    }

    public List<SeniorEval> GetUnlocked(string majorId)
    {
        return GetEvalsByMajor(majorId).Where(e => unlockedEvalIds.Contains(e.evalId)).ToList();
    }

    public bool IsUnlocked(string evalId)
    {
        return unlockedEvalIds.Contains(evalId);
    }

    public void UnlockEval(string evalId)
    {
        unlockedEvalIds.Add(evalId);
        PlayerPrefs.SetInt($"eval_unlocked_{evalId}", 1);
        PlayerPrefs.Save();
        Debug.Log($"[SeniorEvalEngine] 解锁评价: {evalId}");
    }

    public void UnlockViaAd(string evalId)
    {
        UnlockEval(evalId);
    }

    public bool HasMoreBehindLock(string majorId)
    {
        return GetEvalsByMajor(majorId).Any(e => e.adLocked && !IsUnlocked(e.evalId));
    }

    public int GetLockedCount(string majorId)
    {
        return GetEvalsByMajor(majorId).Count(e => e.adLocked && !IsUnlocked(e.evalId));
    }

    public List<string> GetAvailableMajorIds()
    {
        if (allEvals == null) return new List<string>();
        return allEvals.Select(e => e.majorId).Distinct().ToList();
    }

    public bool HasEvalsForMajor(string majorId)
    {
        return GetEvalsByMajor(majorId).Count > 0;
    }

    public void ResetAllUnlocks()
    {
        foreach (var eval in allEvals)
        {
            string key = $"eval_unlocked_{eval.evalId}";
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
        unlockedEvalIds.Clear();
        foreach (var eval in allEvals)
        {
            if (!eval.adLocked)
                unlockedEvalIds.Add(eval.evalId);
        }
    }

    public List<string> GetAllReviewerNPCs()
    {
        var npcs = new List<string> { "学霸型学长", "学姐型学姐", "奋斗型学长", "佛系型学姐", "转行型学长" };
        return npcs;
    }
}