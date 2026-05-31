using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class TierInfo
{
    public string id;
    public string name;
    public string icon;
    public int minScore;
    public string description;
}

[Serializable]
public class RankingListWrapper { public List<RankingEntry> entries; }

[Serializable]
public class RankingEntry
{
    public string name;
    public int score;
}

public class TierEngine : MonoBehaviour
{
    public static TierEngine Instance { get; private set; }

    private List<TierInfo> tiers;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        InitTiers();
    }

    void InitTiers()
    {
        tiers = new List<TierInfo>
        {
            new TierInfo { id = "T0", name = "专科线", icon = "🥉", minScore = 0, description = "首次完成游戏" },
            new TierInfo { id = "T1", name = "本科线", icon = "🥈", minScore = 35, description = "结局评分 > 35" },
            new TierInfo { id = "T2", name = "211线", icon = "🎖️", minScore = 55, description = "结局评分 > 55" },
            new TierInfo { id = "T3", name = "985线", icon = "🏅", minScore = 70, description = "结局评分 > 70" },
            new TierInfo { id = "T4", name = "清北线", icon = "👑", minScore = 85, description = "结局评分 > 85" },
            new TierInfo { id = "T5", name = "传说线", icon = "🌟", minScore = 95, description = "全结局解锁 + 全面调研" }
        };
    }

    public TierInfo CalculateTier(int satisfactionScore, int totalEndings, int seniorEvalCount)
    {
        if (totalEndings >= 4 && seniorEvalCount >= 10)
        {
            if (satisfactionScore >= 90)
                return tiers.Find(t => t.id == "T5");
        }

        var applicable = tiers.Where(t => satisfactionScore >= t.minScore).OrderByDescending(t => t.minScore);
        return applicable.FirstOrDefault() ?? tiers[0];
    }

    public string GetBestTierName()
    {
        int bestScore = PlayerPrefs.GetInt("tier_best_score", 0);
        return CalculateTier(bestScore, 4, 0).name;
    }

    public string GetBestTierIcon()
    {
        int bestScore = PlayerPrefs.GetInt("tier_best_score", 0);
        return CalculateTier(bestScore, 4, 0).icon;
    }

    public void RecordBestScore(int score)
    {
        int currentBest = PlayerPrefs.GetInt("tier_best_score", 0);
        if (score > currentBest)
        {
            PlayerPrefs.SetInt("tier_best_score", score);
            PlayerPrefs.Save();
            Debug.Log($"[TierEngine] 新纪录! 评分: {score}");
        }
    }

    public int GetBestScore()
    {
        return PlayerPrefs.GetInt("tier_best_score", 0);
    }

    public List<TierInfo> GetAllTiers()
    {
        return tiers ?? new List<TierInfo>();
    }

    public int GetProvinceRank(string province, int score)
    {
        var rankings = GetProvinceRankings(province);
        int count = rankings.Count(r => r.score > score);
        return count + 1;
    }

    List<(string name, int score)> GetProvinceRankings(string province)
    {
        var rankings = new List<(string name, int score)>();
        try
        {
            string json = PlayerPrefs.GetString($"province_rankings_{province}", "");
            if (!string.IsNullOrEmpty(json))
            {
                var wrapper = JsonUtility.FromJson<RankingListWrapper>("{\"entries\":" + json + "}");
                if (wrapper?.entries != null)
                {
                    foreach (var e in wrapper.entries)
                        rankings.Add((e.name, e.score));
                }
            }
        }
        catch
        {
            rankings.Add(("匿名", 80));
        }
        return rankings;
    }

    public void AddProvinceScore(string province, string playerName, int score)
    {
        var rankings = GetProvinceRankings(province);
        rankings.Add((playerName, score));
        rankings = rankings.OrderByDescending(r => r.score).Take(100).ToList();

        var entries = new List<RankingEntry>();
        foreach (var r in rankings)
            entries.Add(new RankingEntry { name = r.name, score = r.score });

        var wrapper = new RankingListWrapper { entries = entries };
        string json = JsonUtility.ToJson(wrapper);
        int startIdx = json.IndexOf('[');
        int endIdx = json.LastIndexOf(']');
        if (startIdx >= 0 && endIdx >= startIdx)
            json = json.Substring(startIdx, endIdx - startIdx + 1);

        PlayerPrefs.SetString($"province_rankings_{province}", json);
        PlayerPrefs.Save();
    }
}