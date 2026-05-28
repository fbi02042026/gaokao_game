using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    private Dictionary<string, float> lastShowTime = new Dictionary<string, float>();

    private Dictionary<string, float> cooldownConfig = new Dictionary<string, float>
    {
        {"modify_score", -1f},
        {"unlock_npc", -1f},
        {"refresh_item", 30f},
        {"skip_wait", 0f},
        {"daily_bonus", 86400f},
        {"talent_refresh", -1f},
        {"re_admit", -1f},
        {"show_probability", -1f}
    };

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public async Task<bool> ShowRewardedVideo(string type)
    {
        if (!CanShowAd(type))
        {
            Debug.Log($"[AdManager] 广告冷却中: {type}");
            return false;
        }

#if UNITY_WECHAT_GAME
        Debug.Log($"[AdManager] 微信广告展示: {type}（暂用模拟）");
#elif UNITY_BYTE_DANCE_MINI_GAME
        Debug.Log($"[AdManager] 抖音广告展示: {type}（暂用模拟）");
#else
        Debug.Log($"[AdManager] 模拟广告成功: {type}");
#endif

        lastShowTime[type] = Time.time;
        return true;
    }

    public bool CanShowAd(string type)
    {
        if (!lastShowTime.ContainsKey(type)) return true;
        float cd = cooldownConfig.GetValueOrDefault(type, 0f);
        if (cd < 0) return false;
        if (cd == 0) return true;
        return (Time.time - lastShowTime[type]) >= cd;
    }

    public void ResetPerRound()
    {
        var perRound = new List<string>();
        foreach (var kv in cooldownConfig) if (kv.Value < 0) perRound.Add(kv.Key);
        foreach (var key in perRound) lastShowTime.Remove(key);
    }
}